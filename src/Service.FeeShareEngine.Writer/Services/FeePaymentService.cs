using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.ServiceBus;
using Service.ChangeBalanceGateway.Grpc;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.FeeShareEngine.Postgres.Models;
using Service.IndexPrices.Client;
using Service.Liquidity.Converter.Domain.Models;
using Service.Liquidity.Converter.Grpc;

namespace Service.FeeShareEngine.Writer.Services
{
    public class FeePaymentService : IStartable
    {
        private readonly IConvertIndexPricesClient _convertPricesClient;
        private readonly ISpotChangeBalanceService _changeBalanceService;
        private readonly ILiquidityConverterSettingsManager _liquidityConverterSettings;
        private readonly IClientWalletService _walletService;


        public FeePaymentService(IConvertIndexPricesClient convertPricesClient, ISpotChangeBalanceService changeBalanceService, ILiquidityConverterSettingsManager liquidityConverterSettings, IClientWalletService walletService)
        {
            _convertPricesClient = convertPricesClient;
            _changeBalanceService = changeBalanceService;
            _liquidityConverterSettings = liquidityConverterSettings;
            _walletService = walletService;
        }

        public (decimal feeInUsd, decimal feeShare) CalculateFeeShare(SwapMessage swap)
        {
            var conversionRate = _convertPricesClient.GetConvertIndexPriceByPairAsync(swap.DifferenceAsset, "USD");
            var sharePercent = Program.ReloadedSettings(t => t.FeeSharePercent).Invoke();
            var feeInUsd = conversionRate.Price * swap.DifferenceVolumeAbs;
            var feeShare = feeInUsd * (sharePercent / 100);
            return (feeInUsd, feeShare);
        }
        
        public async Task<bool> TransferToServiceWallet(FeeShareEntity share)
        {
            var converterSettings = _liquidityConverterSettings.GetLiquidityConverterSettingsAsync().Settings.First();
            var result = await _changeBalanceService.TransferFeeShareToServiceWalletAsync(new()
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
            });

            if (!result.Result)
                return false;

            return true;
        }

        public async Task<bool> PayFeeToReferrers(FeePaymentEntity payment)
        {
            var referrer = (await _walletService.SearchClientsAsync(new SearchWalletsRequest()
            {
                SearchText = payment.ReferrerClientId
            })).Clients.First();
            
            var walletId = (await _walletService.GetWalletsByClient(new JetClientIdentity()
            {
                BrokerId = referrer.BrokerId,
                BrandId = referrer.BrandId,
                ClientId = referrer.ClientId
            })).Wallets.First();
            
            var result = await _changeBalanceService.PayFeeSharesToReferrerAsync(new()
            {
                TransactionId = payment.PaymentOperationId,
                ClientId = Program.Settings.ServiceWalletClientId,
                FromWalletId = Program.Settings.ServiceWalletBrokerId,
                ToWalletId = walletId.WalletId,
                Amount = (double)payment.Amount,
                AssetSymbol = "USD",
                Comment = "FeeShares payment to referrer",
                BrokerId = referrer.BrokerId,
                RequestSource = "FeeShareEngine"
            });

            if (!result.Result)
                return false;

            return true;   
        }

        private async Task EnsureServiceWalletIsCreated()
        {
            await _walletService.GetWalletsByClient(new JetClientIdentity(Program.Settings.ServiceWalletBrokerId,
                    Program.Settings.ServiceWalletBrandId, Program.Settings.ServiceWalletClientId));
        }

        public void Start()
        {
            EnsureServiceWalletIsCreated();
        }
    }
}