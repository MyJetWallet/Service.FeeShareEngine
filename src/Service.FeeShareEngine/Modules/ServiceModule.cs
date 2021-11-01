using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.NoSql;

namespace Service.FeeShareEngine.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterMyNoSqlWriter<FeeShareSettingsNoSqlEntity>(
                Program.ReloadedSettings(t => t.MyNoSqlWriterUrl), FeeShareSettingsNoSqlEntity.TableName);
            
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<ReferralMapChangeMessage>(serviceBusClient, ReferralMapChangeMessage.TopicName, true);
            
        }
    }
}