using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.Domain.Models.Models;

namespace Service.FeeShareEngine.NoSql
{
    public class FeeShareSettingsNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-feeshare-settings";

        public static string GeneratePartitionKey() => "FeeShareSettings";

        public static string GenerateRowKey() => "FeeShareSettings";
        
        public FeeShareSettingsModel Settings { get; set; }

        public static FeeShareSettingsNoSqlEntity Create(FeeShareSettingsModel settings) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Settings = settings
            };
    }
}