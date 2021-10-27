using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.FeeShareEngine.NoSql;

namespace Service.FeeShareEngine.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterMyNoSqlWriter<FeeShareSettingsNoSqlEntity>(
                Program.ReloadedSettings(t => t.MyNoSqlWriterUrl), FeeShareSettingsNoSqlEntity.TableName);
        }
    }
}