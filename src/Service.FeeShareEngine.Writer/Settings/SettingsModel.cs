using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.FeeShareEngine.Writer.Settings
{
    public class SettingsModel
    {
        [YamlProperty("FeeShareEngine.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("FeeShareEngine.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("FeeShareEngine.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("FeeShareEngine.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("FeeShareEngine.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("FeeShareEngine.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("FeeShareEngine.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
        
        [YamlProperty("FeeShareEngine.MaxCachedEntities")]
        public int MaxCachedEntities { get; set; }
        
        [YamlProperty("FeeShareEngine.ClientWalletsGrpcServiceUrl")]
        public string ClientWalletsGrpcServiceUrl { get; set; }
        
        [YamlProperty("FeeShareEngine.ChangeBalanceGatewayGrpcServiceUrl")]
        public string ChangeBalanceGatewayGrpcServiceUrl { get; set; }
        
        [YamlProperty("FeeShareEngine.LiquidityConverterGrpcServiceUrl")]
        public string LiquidityConverterGrpcServiceUrl { get; set; }

        [YamlProperty("FeeShareEngine.ServiceWalletClientId")]
        public string ServiceWalletClientId { get; set; }
        
        [YamlProperty("FeeShareEngine.ServiceWalletId")]
        public string ServiceWalletId { get; set; }
        
        [YamlProperty("FeeShareEngine.ServiceWalletBrokerId")]
        public string ServiceWalletBrokerId { get; set; }
        
        [YamlProperty("FeeShareEngine.ServiceWalletBrandId")]
        public string ServiceWalletBrandId { get; set; }
        
        [YamlProperty("FeeShareEngine.PeriodType")]
        public string PeriodType { get; set; }
        
        [YamlProperty("FeeShareEngine.DefaultBrand")]
        public string DefaultBrand { get; set; }
    }
}
