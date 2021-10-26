using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Grpc;
using Service.FeeShareEngine.Grpc.Models;
using Service.FeeShareEngine.Postgres;
using Service.FeeShareEngine.Settings;

namespace Service.FeeShareEngine.Services
{
    public class ReferralMapService: IReferralMapService
    {
        private readonly ILogger<ReferralMapService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public ReferralMapService(ILogger<ReferralMapService> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task AddReferralLink(AddReferralRequest request)
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            await ctx.UpsetAsync(new[]
            {
                new ReferralMapEntity()
                {
                    ReferrerClientId = request.ReferrerClientId,
                    ClientId = request.ClientId
                }
            });
        }

        public async Task<GetAllReferralMapsResponse> GetAllReferralMaps()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var maps = await ctx.Referrals.ToListAsync();
            return new GetAllReferralMapsResponse()
            {
                Referrals = maps
            };
        }
    }
}
