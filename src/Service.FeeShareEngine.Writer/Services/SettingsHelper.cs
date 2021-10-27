using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.NoSql;
using Service.FeeShareEngine.Writer.Settings;

namespace Service.FeeShareEngine.Writer.Services
{
    public class SettingsHelper
    {
        private readonly IMyNoSqlServerDataWriter<FeeShareSettingsNoSqlEntity> _settingWriter;
        private readonly MyTaskTimer _timer;
        private readonly ILogger<SettingsHelper> _logger;
        
        public FeeShareSettingsModel SettingsModel { get; set; }

        public SettingsHelper(IMyNoSqlServerDataWriter<FeeShareSettingsNoSqlEntity> settingWriter, ILogger<SettingsHelper> logger)
        {
            _settingWriter = settingWriter;
            _logger = logger;
            _timer = new MyTaskTimer(nameof(SettingsHelper), TimeSpan.FromMinutes(1), logger, DoTimer);

        }
        
        private async Task DoTimer()
        {
            var entity = await _settingWriter.GetAsync(FeeShareSettingsNoSqlEntity.GeneratePartitionKey(),
                FeeShareSettingsNoSqlEntity.GenerateRowKey());
            if (entity != null)
            {
                SettingsModel = entity.Settings;
            }
            else
            {
                if (!Enum.TryParse(typeof(PeriodTypes), Program.Settings.PeriodType, true, out var type))
                {
                    _logger.LogError("Period {period} cannot be parsed", Program.Settings.PeriodType);
                    throw new Exception($"Period {Program.Settings.PeriodType} cannot be parsed");
                }

                var periodType = (PeriodTypes)type;
                SettingsModel = new FeeShareSettingsModel
                {
                    FeeShareEngineWalletId = Program.Settings.ServiceWalletId,
                    FeeShareEngineClientId = Program.Settings.ServiceWalletClientId,
                    FeeShareEngineBrokerId = Program.Settings.ServiceWalletBrokerId,
                    FeeShareEngineBrandId = Program.Settings.ServiceWalletBrandId,
                    CurrentPeriod = periodType
                };
                await _settingWriter.InsertOrReplaceAsync(FeeShareSettingsNoSqlEntity.Create(SettingsModel));
            }
        }
    }
}