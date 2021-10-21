using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.Postgres.Models;

namespace Service.FeeShareEngine.NoSql
{
    public class ReferrerMapNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-feeshare-referrermap";

        public static string GeneratePartitionKey() => "ReferrerMap";

        public static string GenerateRowKey(string clientId) => clientId;
        
        public ReferralMapEntity MapEntity { get; set; }

        public static ReferrerMapNoSqlEntity Create(ReferralMapEntity map) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(map.ClientId),
                MapEntity = map
            };
    }
}