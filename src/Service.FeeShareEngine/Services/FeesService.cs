using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.FeeShareEngine.Grpc;
using Service.FeeShareEngine.Grpc.Models;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Services
{
    public class FeesService : IFeesService
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public FeesService(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetAllStatsResponse> GetAllStatsAsync()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var stats = await ctx.ShareStatistics.ToListAsync();
            return new GetAllStatsResponse()
            {
                Stats = stats
            };
        }

        public async Task<GetAllFeePaymentsResponse> GetAllFeePaymentsAsync()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var payments = await ctx.FeePayments.ToListAsync();
            return new GetAllFeePaymentsResponse()
            {
                FeePayments = payments
            };
        }

        public async Task<GetAllFeeSharesResponse> GetAllFeeSharesAsync()
        {
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var shares = await ctx.FeeShares.ToListAsync();
            return new GetAllFeeSharesResponse()
            {
                FeeShares = shares
            };        
        }
    }
}