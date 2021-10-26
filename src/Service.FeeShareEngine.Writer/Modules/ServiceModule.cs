using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.ChangeBalanceGateway.Client;
using Service.ClientWallets.Client;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.NoSql;
using Service.FeeShareEngine.Writer.Services;
using Service.IndexPrices.Client;
using Service.Liquidity.Converter.Client;
using Service.Liquidity.Converter.Domain.Models;
using Service.MatchingEngine.EventBridge.ServiceBus;

namespace Service.FeeShareEngine.Writer.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), Program.LogFactory);
            builder.RegisterMyServiceBusSubscriberBatch<SwapMessage>(serviceBusClient, SwapMessage.TopicName, "FeeShareEngine-Writer", TopicQueueType.Permanent);
            builder.RegisterMyServiceBusPublisher<FeePaymentEntity>(serviceBusClient, FeePaymentEntity.TopicName, true);
            builder.RegisterMyServiceBusPublisher<FeeShareEntity>(serviceBusClient, FeeShareEntity.TopicName, true);

            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(t => t.MyNoSqlReaderHostPort));
            builder.RegisterLiquidityConverterManagerClient(Program.Settings.LiquidityConverterGrpcServiceUrl);
            builder.RegisterSpotChangeBalanceGatewayClient(Program.Settings.ChangeBalanceGatewayGrpcServiceUrl);
            builder.RegisterClientWalletsClientsWithoutCache(Program.Settings.ClientWalletsGrpcServiceUrl);
            builder.RegisterConvertIndexPricesClient(myNoSqlClient);
            
            builder
                .RegisterType<ReferralMapCache>()
                .AsSelf()
                .SingleInstance();
            
            builder
                .RegisterType<FeePaymentWriter>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<FeeShareWriter>()
                .AsSelf()
                .AutoActivate()
                .SingleInstance();
            
            builder
                .RegisterType<FeePaymentService>()
                .AsSelf()
                .SingleInstance();
            

        }
    }
}