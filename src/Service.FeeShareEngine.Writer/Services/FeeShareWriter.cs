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
using Service.Liquidity.Converter.Domain.Models;

namespace Service.FeeShareEngine.Writer.Services
{
    public class FeeShareWriter
    {
        private readonly ILogger<FeeShareWriter> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ReferralMapCache _referralMap;
        private readonly IClientWalletService _walletService;
        private readonly SharesPaymentService _paymentService;
        private readonly IServiceBusPublisher<FeeShareEntity> _feeSharePublisher;
        private readonly Dictionary<string, ClientContext> _clientContexts = new();
        public FeeShareWriter(
            ILogger<FeeShareWriter> logger, 
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            ISubscriber<IReadOnlyList<SwapMessage>> subscriber, 
            ReferralMapCache referralMap, 
            IClientWalletService walletService,
            SharesPaymentService paymentService, 
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
                var clientContext = await GetClientContext(swap.WalletId1);
                if(string.IsNullOrEmpty(clientContext?.ReferrerClientId))
                    continue;

                var (feeShareAmountInNative, feeShareInTarget) = _paymentService.CalculateFeeShare(swap, clientContext.FeeShareGroup);
                
                var feeShare = new FeeShareEntity
                {
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
                    FeeToTargetConversionRate = feeShareInTarget/feeShareAmountInNative
                }; 
                feeShares.Add(feeShare);
            }

            if (feeShares.Any())
            {
                await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
                await ctx.UpsetAsync(feeShares);
            }
        }

        private async ValueTask<ClientContext> GetClientContext(string walletId)
        {
            if (_clientContexts.TryGetValue(walletId, out var context))
                return context;
            
            
            var client = await _walletService.SearchClientsAsync(new SearchWalletsRequest()
            {
                SearchText = walletId,
                Take = 1
            });
            var firstClient = client.Clients?.FirstOrDefault();
            if (firstClient == null)
            {
                _logger.LogError("Cannot find client for walletId {walletId}", walletId);
                return null;
            }

            var (referrer, feeGroup) = await _referralMap.GetReferrerId(firstClient.ClientId);
            context = new ClientContext(firstClient.ClientId, walletId, firstClient.BrokerId, referrer, feeGroup);
            _clientContexts[walletId] = context;

            while (_clientContexts.Count > Program.Settings.MaxCachedEntities)
            {
                _clientContexts.Remove(_clientContexts.OrderBy(t => t.Value.TimeStamp).First().Key);
            }
            
            return context;
        }

        private class ClientContext
        {
            public string ClientId { get; set; }
            public string WalletId { get; set; }
            public string BrokerId { get; set; }
            public string ReferrerClientId { get; set; }
            public DateTime TimeStamp { get; set; }
            public FeeShareGroup FeeShareGroup { get; set; }

            public ClientContext(string clientId, string walletId, string brokerId, string referrerClientId, FeeShareGroup feeShareGroup)
            {
                ClientId = clientId;
                WalletId = walletId;
                BrokerId = brokerId;
                ReferrerClientId = referrerClientId;
                TimeStamp = DateTime.UtcNow;
                FeeShareGroup = feeShareGroup;
            }
        }
    }
    
     
}