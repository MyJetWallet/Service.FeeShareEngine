using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.FeeShareEngine.Settings
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
        
        [YamlProperty("FeeShareEngine.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("FeeShareEngine.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
    }
}
