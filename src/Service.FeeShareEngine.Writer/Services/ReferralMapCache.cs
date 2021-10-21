using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.NoSql;
using Service.FeeShareEngine.Postgres;

namespace Service.FeeShareEngine.Writer.Services
{
    public class ReferralMapCache
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<ReferrerMapNoSqlEntity> _dataWriter;
        public ReferralMapCache(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, IMyNoSqlServerDataWriter<ReferrerMapNoSqlEntity> dataWriter)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _dataWriter = dataWriter;
        }

        public async Task<string> GetReferrerId(string clientId)
        {
            var mapEntity = await _dataWriter.GetAsync(ReferrerMapNoSqlEntity.GeneratePartitionKey(),
                ReferrerMapNoSqlEntity.GenerateRowKey(clientId));

            if (mapEntity != null)
                return mapEntity.MapEntity.ReferrerClientId;
            
            await using var ctx = DatabaseContext.Create(_dbContextOptionsBuilder);
            var map = await ctx.Referrals.FirstOrDefaultAsync(t => t.ClientId == clientId);

            if (map == null) 
                return null;
            
            await _dataWriter.InsertOrReplaceAsync(ReferrerMapNoSqlEntity.Create(map));
            await _dataWriter.CleanAndKeepLastRecordsAsync(ReferrerMapNoSqlEntity.GeneratePartitionKey(),
                Program.Settings.MaxCachedEntities);

            return map.ReferrerClientId;
        }
    }
}