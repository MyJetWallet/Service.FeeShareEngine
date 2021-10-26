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
        private readonly FeePaymentService _paymentService;
        private readonly IServiceBusPublisher<FeeShareEntity> _feeSharePublisher;
        private readonly Dictionary<string, ClientContext> _clientContexts = new();
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
                var clientContext = await GetClientContext(swap.WalletId1);
                if(string.IsNullOrEmpty(clientContext?.ReferrerClientId))
                    continue;

                var (feeShareAmount, feeShareInUsd) = _paymentService.CalculateFeeShare(swap);
                
                var feeShare = new FeeShareEntity
                {
                    ReferrerClientId = clientContext.ReferrerClientId,
                    FeeShareAmountInUsd = feeShareInUsd,
                    TimeStamp = DateTime.UtcNow,
                    OperationId = swap.MessageId,
                    FeeTransferOperationId = swap.MessageId + "|FeeTransferToService",
                    FeeAmount = swap.DifferenceVolumeAbs,
                    FeeAsset = swap.DifferenceAsset,
                    FeeShareAmount = feeShareAmount,
                    Status = PaymentStatus.New
                }; 
                await _paymentService.TransferToServiceWallet(feeShare);
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
                SearchText = walletId
            });
            var firstClient = client.Clients?.FirstOrDefault();
            if (firstClient == null)
            {
                _logger.LogError("Cannot find client for walletId {walletId}", walletId);
                return null;
            }

            var referrer = await _referralMap.GetReferrerId(firstClient.ClientId);
            context = new ClientContext(firstClient.ClientId, walletId, firstClient.BrokerId, referrer);
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

            public ClientContext(string clientId, string walletId, string brokerId, string referrerClientId)
            {
                ClientId = clientId;
                WalletId = walletId;
                BrokerId = brokerId;
                ReferrerClientId = referrerClientId;
                TimeStamp = DateTime.UtcNow;
            }
        }
    }
    
     
}