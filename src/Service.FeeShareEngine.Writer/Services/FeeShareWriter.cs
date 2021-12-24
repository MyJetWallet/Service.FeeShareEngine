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
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Postgres;
using Service.IndexPrices.Client;
using Service.Liquidity.Converter.Domain.Models;

namespace Service.FeeShareEngine.Writer.Services
{
    public class FeeShareWriter
    {
        private readonly ILogger<FeeShareWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ClientContextManager _contextManager;
        private readonly SharesPaymentService _paymentService;
        private readonly IIndexPricesClient _indexPrices;
        public FeeShareWriter(
            ILogger<FeeShareWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ISubscriber<IReadOnlyList<SwapMessage>> subscriber, 
            ClientContextManager contextManager, 
            SharesPaymentService paymentService, IIndexPricesClient indexPrices)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _contextManager = contextManager;
            _paymentService = paymentService;
            _indexPrices = indexPrices;
            subscriber.Subscribe(HandleEvents);

        }
        
        private async ValueTask HandleEvents(IReadOnlyList<SwapMessage> swaps)
        {
            var feeShares = new List<FeeShareEntity>();
            foreach (var swap in swaps)
            {
                var clientContext = await _contextManager.GetClientContext(swap.WalletId1);
                if(string.IsNullOrEmpty(clientContext?.ReferrerClientId))
                    continue;

                var (feeShareAmountInNative, feeShareInTarget) = _paymentService.CalculateFeeShare(swap, clientContext.FeeShareGroup);
                
                var feeShare = new FeeShareEntity
                {
                    ReferralClientId = swap.AccountId1,
                    ReferrerClientId = clientContext.ReferrerClientId,
                    FeeShareAmountInTargetAsset = feeShareInTarget,
                    TimeStamp = DateTime.UtcNow,
                    OperationId = swap.MessageId,
                    FeeTransferOperationId = swap.MessageId + "|FeeTransferToService",
                    FeeAmount = swap.DifferenceVolumeAbs,
                    FeeAsset = swap.DifferenceAsset,
                    FeeShareAmountInFeeAsset = feeShareAmountInNative,
                    Status = PaymentStatus.New,
                    FeeShareAsset = clientContext.FeeShareGroup.AssetId,
                    FeeToTargetConversionRate = feeShareInTarget/feeShareAmountInNative,
                    FeeAssetIndexPrice = _indexPrices.GetIndexPriceByAssetAsync(swap.DifferenceAsset).UsdPrice,
                    TargetAssetIndexPrice = _indexPrices.GetIndexPriceByAssetAsync(clientContext.FeeShareGroup.AssetId).UsdPrice
                }; 
                feeShares.Add(feeShare);
            }

            if (feeShares.Any())
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                await ctx.UpsetAsync(feeShares);
            }
        }
    }
}