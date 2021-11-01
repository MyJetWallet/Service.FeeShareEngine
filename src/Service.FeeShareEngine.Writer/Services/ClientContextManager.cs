using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.ClientWallets.Grpc;
using Service.ClientWallets.Grpc.Models;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Writer.Services
{
    public class ClientContextManager
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IClientWalletService _walletService;
        private readonly Dictionary<string, ClientContext> _clientContexts = new();
        private readonly ILogger<FeeShareWriter> _logger;

        public ClientContextManager(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IClientWalletService walletService, ILogger<FeeShareWriter> logger, ISubscriber<ReferralMapChangeMessage> subscriber)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _walletService = walletService;
            _logger = logger;
            subscriber.Subscribe(HandleEvent);
        }

        public async Task<(string referrerClientId, FeeShareGroup feeShareGroup)> GetReferrerId(string clientId)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var map = await ctx.Referrals.FirstOrDefaultAsync(t => t.ClientId == clientId);
            if (map != null)
            {
                var feeShareGroup = await ctx.FeeShareGroups.FirstOrDefaultAsync(t => t.GroupId == map.FeeShareGroupId);
                return (map?.ReferrerClientId, feeShareGroup);
            }
            return (null, null);
        }
        
        public async ValueTask<ClientContext> GetClientContext(string walletId)
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

            var (referrer, feeGroup) = await GetReferrerId(firstClient.ClientId);
            context = new ClientContext(firstClient.ClientId, walletId, firstClient.BrokerId, referrer, feeGroup);
            _clientContexts[walletId] = context;

            while (_clientContexts.Count > Program.Settings.MaxCachedEntities)
            {
                _clientContexts.Remove(_clientContexts.OrderBy(t => t.Value.TimeStamp).First().Key);
            }
            
            return context;
        }

        private async ValueTask HandleEvent(ReferralMapChangeMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message.ClientId))
            {
                var (key, value) = _clientContexts.FirstOrDefault(t => t.Value.ClientId == message.ClientId);
                if (value != null)
                    _clientContexts.Remove(key);
            }

            if (!string.IsNullOrWhiteSpace(message.FeeShareGroupId))
            {
                var (key, value) = _clientContexts.FirstOrDefault(t => t.Value.FeeShareGroup.GroupId == message.FeeShareGroupId);
                if (value != null)
                    _clientContexts.Remove(key);
            }
        }
    }
}