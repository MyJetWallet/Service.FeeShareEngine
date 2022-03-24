using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyJetWallet.Sdk.ServiceBus;
using Service.AssetsDictionary.Client;
using Service.ChangeBalanceGateway.Grpc;
using Service.ChangeBalanceGateway.Grpc.Models;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.IndexPrices.Client;
using Service.Liquidity.Converter.Domain.Models;
using Service.Liquidity.Converter.Domain.Models.Settings;
using Service.Liquidity.Converter.Grpc;

namespace Service.FeeShareEngine.Writer.Services
{
    public class SharesPaymentService : IStartable
    {
        private readonly IConvertIndexPricesClient _convertPricesClient;
        private readonly ISpotChangeBalanceService _changeBalanceService;
        private readonly ILiquidityConverterSettingsManager _liquidityConverterSettings;
        private readonly IClientWalletService _walletService;
        private LiquidityConverterSettings _converterSettings;
        private DateTime _converterSettingsTimeStamp = DateTime.MinValue;
        private readonly IServiceBusPublisher<FeePaymentEntity> _feePaymentPublisher;
        private readonly ILogger<SharesPaymentService> _logger;
        private readonly SettingsHelper _settingsHelper;
        private readonly IAssetsDictionaryClient _assetsDictionary;

        public SharesPaymentService(IConvertIndexPricesClient convertPricesClient, ISpotChangeBalanceService changeBalanceService, ILiquidityConverterSettingsManager liquidityConverterSettings, IClientWalletService walletService, ILogger<SharesPaymentService> logger, IServiceBusPublisher<FeePaymentEntity> feePaymentPublisher, SettingsHelper settingsHelper, IAssetsDictionaryClient assetsDictionary)
        {
            _convertPricesClient = convertPricesClient;
            _changeBalanceService = changeBalanceService;
            _liquidityConverterSettings = liquidityConverterSettings;
            _walletService = walletService;
            _logger = logger;
            _feePaymentPublisher = feePaymentPublisher;
            _settingsHelper = settingsHelper;
            _assetsDictionary = assetsDictionary;
        }

        public (decimal feeShareInNative, decimal feeShareInTarget) CalculateFeeShare(SwapMessage swap, FeeShareGroup feeShareGroup)
        {
            var conversionRate = _convertPricesClient.GetConvertIndexPriceByPairAsync(swap.FeeAsset, feeShareGroup.AssetId);
            var sharePercent = feeShareGroup.FeePercent;
            var feeShareInNative = swap.FeeAmount * (sharePercent / 100);
            var feeShareInTarget = conversionRate.Price * feeShareInNative;
            return (feeShareInNative, feeShareInTarget);
        }

        public async Task TransferSettlementPayment(ShareStatEntity entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.SettlementOperationId))
                    entity.SettlementOperationId = Guid.NewGuid().ToString("N") + "|FeeShareSettlement";

                var converterSettings = GetConverterSettings();

                var asset = _assetsDictionary.GetAssetById(new AssetIdentity
                {
                    BrokerId = _settingsHelper.SettingsModel.FeeShareEngineBrokerId,
                    Symbol = entity.AssetId
                });

                if (asset == null)
                {
                    _logger.LogError("Unable to find asset {asset}", entity.AssetId);
                    entity.Status = SettlementStatus.FailedToPay;
                    entity.ErrorMessage = $"Unable to find asset {entity.AssetId}";
                    return;
                }

                var amount = Math.Round(entity.Amount, asset.Accuracy, MidpointRounding.ToZero);

                if (amount <= 0)
                {
                    entity.Status = SettlementStatus.FailedToPay;
                    entity.ErrorMessage = "Amount too small";
                    return;
                }

                var request = new FeeTransferRequest
                {
                    TransactionId = entity.SettlementOperationId,
                    ClientId = converterSettings.BrokerAccountId,
                    FromWalletId = converterSettings.BrokerWalletId,
                    ToWalletId = _settingsHelper.SettingsModel.FeeShareEngineWalletId,
                    Amount = amount,
                    AssetSymbol = entity.AssetId,
                    Comment = "FeeShare settlement transfer to service wallet",
                    BrokerId = converterSettings.BrokerId,
                    RequestSource = "FeeShareEngine"
                };
                var result = await _changeBalanceService.TransferFeeShareToServiceWalletAsync(request);
                if (result.Result)
                {
                    entity.Status = SettlementStatus.Settled;
                    entity.PaymentTimestamp = DateTime.UtcNow;
                }
                else
                {
                    entity.Status = SettlementStatus.FailedToPay;
                    entity.ErrorMessage = result.ErrorMessage;
                    _logger.LogError(
                        "When transferring fee settlement to service wallet. Transfer request {requestJson} Transfer error {error}",
                        JsonSerializer.Serialize(request), result.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When transferring settlement payment id {operationId}", entity.SettlementOperationId);
                entity.Status = SettlementStatus.FailedToPay;
                entity.ErrorMessage = e.Message;
            }
        }
        
        public async Task PayFeeToReferrers(FeePaymentEntity payment)
        {
            try
            {
                if (string.IsNullOrEmpty(payment.PaymentOperationId))
                    payment.PaymentOperationId = Guid.NewGuid().ToString("N") + "|FeeSharePayment";

                var response = await _walletService.SearchClientsAsync(new SearchWalletsRequest()
                {
                    SearchText = payment.ReferrerClientId,
                    Take = 1
                });

                var asset = _assetsDictionary.GetAssetById(new AssetIdentity
                {
                    BrokerId = _settingsHelper.SettingsModel.FeeShareEngineBrokerId,
                    Symbol = payment.AssetId
                });

                if (asset == null)
                {
                    _logger.LogError("Unable to find asset {asset}", payment.AssetId);
                    payment.Status = PaymentStatus.FailedToPay;
                    payment.ErrorMessage = $"Unable to find asset {payment.AssetId}";
                    return;
                }

                var referrer = response.Clients?.FirstOrDefault();
                if (referrer == null)
                {
                    _logger.LogError("Cannot find wallet for user {userId}", payment.ReferrerClientId);
                    payment.Status = PaymentStatus.FailedToPay;
                    payment.ErrorMessage = $"Cannot find wallet for user {payment.ReferrerClientId}";
                    return;
                }

                var walletId = (await _walletService.GetWalletsByClient(new JetClientIdentity()
                {
                    BrokerId = referrer.BrokerId,
                    BrandId = Program.Settings.DefaultBrand,
                    ClientId = referrer.ClientId
                })).Wallets.First();

                var amount = Math.Round(payment.Amount, asset.Accuracy, MidpointRounding.ToZero);

                if (amount <= 0)
                {
                    payment.Status = PaymentStatus.FailedToPay;
                    payment.ErrorMessage = "Amount too small";
                    return;
                }

                var request = new FeeTransferRequest
                {
                    TransactionId = payment.PaymentOperationId,
                    ClientId = _settingsHelper.SettingsModel.FeeShareEngineClientId,
                    FromWalletId = _settingsHelper.SettingsModel.FeeShareEngineWalletId,
                    ToWalletId = walletId.WalletId,
                    Amount = amount,
                    AssetSymbol = payment.AssetId,
                    Comment = "FeeShares payment to referrer",
                    BrokerId = referrer.BrokerId,
                    RequestSource = "FeeShareEngine"
                };
                var result = await _changeBalanceService.PayFeeSharesToReferrerAsync(request);
                payment.ReferrerWalletId = walletId.WalletId;

                if (result.Result)
                {
                    payment.Status = PaymentStatus.Paid;
                    payment.PaymentTimestamp = DateTime.UtcNow;

                    await _feePaymentPublisher.PublishAsync(payment);
                }
                else
                {
                    payment.Status = PaymentStatus.FailedToPay;
                    payment.ErrorMessage = result.ErrorMessage;
                    _logger.LogError(
                        "When transferring fee payment to referrer. Transfer request {requestJson} Transfer error {error}",
                        JsonSerializer.Serialize(request), result.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When transferring fee payment to referrer id {operationId}", payment.PaymentOperationId);
                payment.Status = PaymentStatus.FailedToPay;
                payment.ErrorMessage = e.Message;
            }
        }

        private async Task EnsureServiceWalletIsCreated()
        {
            await _walletService.GetWalletsByClient(new JetClientIdentity(_settingsHelper.SettingsModel.FeeShareEngineBrokerId,
                _settingsHelper.SettingsModel.FeeShareEngineBrandId, _settingsHelper.SettingsModel.FeeShareEngineClientId));
        }

        private LiquidityConverterSettings GetConverterSettings()
        {
            if ((DateTime.UtcNow - _converterSettingsTimeStamp).TotalMinutes > 10 || _converterSettings == null)
            {
                _converterSettings = _liquidityConverterSettings.GetLiquidityConverterSettingsAsync().Settings.First();
                _converterSettingsTimeStamp = DateTime.UtcNow;
            }

            return _converterSettings;
        }

        public void Start()
        {
            EnsureServiceWalletIsCreated();
        }
    }
}