using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.FeeShareEngine.Domain.Models;
using Service.FeeShareEngine.Postgres;
using Service.FeeShareEngine.Postgres.Models;
using Service.Liquidity.Converter.Domain.Models;

namespace Service.FeeShareEngine.Writer.Services
{
    public class FeeShareWriter
    {
        private readonly ILogger<FeeShareWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ReferralMapCache _referralMap;
        private readonly IClientWalletService _walletService;
        private readonly FeePaymentService _paymentService;
        private readonly IServiceBusPublisher<FeeShareEntity> _feeSharePublisher;

        public FeeShareWriter(
            ILogger<FeeShareWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ISubscriber<IReadOnlyList<SwapMessage>> subscriber, 
            ReferralMapCache referralMap, 
            IClientWalletService walletService,
            FeePaymentService paymentService, 
            IServiceBusPublisher<FeeShareEntity> feeSharePublisher)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _referralMap = referralMap;
            _walletService = walletService;
            _paymentService = paymentService;
            _feeSharePublisher = feeSharePublisher;
            subscriber.Subscribe(HandleEvents);

        }
        
        private async ValueTask HandleEvents(IReadOnlyList<SwapMessage> swaps)
        {
            var feeShares = new List<FeeShareEntity>();
            foreach (var swap in swaps)
            {
                var clientId = await _walletService.SearchClientsAsync(new SearchWalletsRequest()
                {
                    SearchText = swap.WalletId1
                });
                var referrer = await _referralMap.GetReferrerId(clientId.Clients.First().ClientId);
                if(string.IsNullOrEmpty(referrer))
                    continue;

                var (feeInUsd, feeShareAmount) = _paymentService.CalculateFeeShare(swap);
                
                var feeShare = new FeeShareEntity
                {
                    ReferrerClientId = referrer,
                    FeeShareAmountInUsd = feeShareAmount,
                    TimeStamp = DateTime.UtcNow,
                    OperationId = swap.MessageId,
                    FeeTransferOperationId = swap.MessageId + "|FeeTransferToService",
                    FeeAmount = swap.DifferenceVolumeAbs,
                    FeeAsset = swap.DifferenceAsset,
                    FeeAmountInUsd = feeInUsd,
                    Status = PaymentStatus.New
                };
                var isSuccess = await _paymentService.TransferToServiceWallet(feeShare);
                feeShare.Status = isSuccess ? PaymentStatus.Paid : PaymentStatus.FailedToPay;
                if(isSuccess)
                    feeShare.PaymentTimestamp = DateTime.UtcNow;
                
                feeShares.Add(feeShare);
            }
            
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            await ctx.UpsetAsync(feeShares);
            await ctx.SaveChangesAsync();

            await _feeSharePublisher.PublishAsync(feeShares);
        }

    }
}