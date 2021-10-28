using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.NoSql;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Writer.Services
{
    public class ReferralMapCache
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        public ReferralMapCache(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
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
    }
}