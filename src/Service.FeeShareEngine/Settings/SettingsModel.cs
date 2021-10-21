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
    }
}
