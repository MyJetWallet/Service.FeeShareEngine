using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.ServiceBus;
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
    public class FeePaymentService : IStartable
    {
        private readonly IConvertIndexPricesClient _convertPricesClient;
        private readonly ISpotChangeBalanceService _changeBalanceService;
        private readonly ILiquidityConverterSettingsManager _liquidityConverterSettings;
        private readonly IClientWalletService _walletService;
        private LiquidityConverterSettings _converterSettings;
        private DateTime _converterSettingsTimeStamp = DateTime.MinValue;
        private readonly IServiceBusPublisher<FeeShareEntity> _feeSharePublisher;
        private readonly IServiceBusPublisher<FeePaymentEntity> _feePaymentPublisher;
        private readonly ILogger<FeePaymentService> _logger;


        public FeePaymentService(IConvertIndexPricesClient convertPricesClient, ISpotChangeBalanceService changeBalanceService, ILiquidityConverterSettingsManager liquidityConverterSettings, IClientWalletService walletService, IServiceBusPublisher<FeeShareEntity> feeSharePublisher, ILogger<FeePaymentService> logger, IServiceBusPublisher<FeePaymentEntity> feePaymentPublisher)
        {
            _convertPricesClient = convertPricesClient;
            _changeBalanceService = changeBalanceService;
            _liquidityConverterSettings = liquidityConverterSettings;
            _walletService = walletService;
            _feeSharePublisher = feeSharePublisher;
            _logger = logger;
            _feePaymentPublisher = feePaymentPublisher;
        }

        public (decimal feeShare, decimal feeShareInUsd) CalculateFeeShare(SwapMessage swap)
        {
            var conversionRate = _convertPricesClient.GetConvertIndexPriceByPairAsync(swap.DifferenceAsset, "USD");
            var sharePercent = Program.ReloadedSettings(t => t.FeeSharePercent).Invoke();
            var feeShare = swap.DifferenceVolumeAbs * (sharePercent / 100);
            var feeShareInUsd = conversionRate.Price * feeShare;
            return (feeShare, feeShareInUsd);
        }
        
        public async Task TransferToServiceWallet(FeeShareEntity share)
        {
            var converterSettings = GetConverterSettings();
            var request = new FeeTransferRequest
            {
                TransactionId = share.FeeTransferOperationId,
                ClientId = converterSettings.BrokerAccountId,
                FromWalletId = converterSettings.BrokerWalletId,
                ToWalletId = Program.Settings.ServiceWalletId,
                Amount = (double)share.FeeShareAmountInUsd,
                AssetSymbol = "USD",
                Comment = "FeeShare transfer to service wallet",
                BrokerId = converterSettings.BrokerId,
                RequestSource = "FeeShareEngine"
            };
            var result = await _changeBalanceService.TransferFeeShareToServiceWalletAsync(request);

            share.FeeShareWalletId = Program.Settings.ServiceWalletId;
            share.ConverterWalletId = converterSettings.BrokerWalletId;
            share.BrokerId = converterSettings.BrokerId;

            if (result.Result)
            {
                share.Status = PaymentStatus.Reserved;
                share.PaymentTimestamp = DateTime.UtcNow;
                await _feeSharePublisher.PublishAsync(share);
            }
            else
            {
                share.Status = PaymentStatus.FailedToReserve;
                share.ErrorMessage = result.ErrorMessage;
                _logger.LogError("When transferring fee share to service wallet. Transfer request {requestJson} Transfer error {error}", JsonSerializer.Serialize(request), result.ErrorMessage);
            }
        }

        public async Task PayFeeToReferrers(FeePaymentEntity payment)
        {
            var response = await _walletService.SearchClientsAsync(new SearchWalletsRequest()
            {
                SearchText = payment.ReferrerClientId,
                Take = 1
            });
            
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
                BrandId = referrer.BrandId,
                ClientId = referrer.ClientId
            })).Wallets.First();

            var request = new FeeTransferRequest
            {
                TransactionId = payment.PaymentOperationId,
                ClientId = Program.Settings.ServiceWalletClientId,
                FromWalletId = Program.Settings.ServiceWalletId,
                ToWalletId = walletId.WalletId,
                Amount = (double)payment.Amount,
                AssetSymbol = "USD",
                Comment = "FeeShares payment to referrer",
                BrokerId = referrer.BrokerId,
                RequestSource = "FeeShareEngine"
            };
            var result = await _changeBalanceService.PayFeeSharesToReferrerAsync(request);
            
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
                _logger.LogError("When transferring fee payment to referrer. Transfer request {requestJson} Transfer error {error}", JsonSerializer.Serialize(request), result.ErrorMessage);
            }
        }

        private async Task EnsureServiceWalletIsCreated()
        {
            await _walletService.GetWalletsByClient(new JetClientIdentity(Program.Settings.ServiceWalletBrokerId,
                    Program.Settings.ServiceWalletBrandId, Program.Settings.ServiceWalletClientId));
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