using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.FeeShareEngine.Client
{
    public static class AutofacHelper
    {
        public static void RegisterFeeShareEngineClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new FeeShareEngineClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetReferralMapService()).As<IReferralMapService>().SingleInstance();
            builder.RegisterInstance(factory.GetFeesService()).As<IFeesService>().SingleInstance();

        }

        public static void RegisterFeeShareSubscriber(this ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient, string queue)
        {
            builder.RegisterMyServiceBusSubscriberBatch<FeeShareEntity>(serviceBusClient, FeeShareEntity.TopicName, queue,
                TopicQueueType.Permanent);
        } 
        
        public static void RegisterFeePaymentSubscriber(this ContainerBuilder builder, MyServiceBusTcpClient serviceBusClient, string queue)
        {
            builder.RegisterMyServiceBusSubscriberBatch<FeePaymentEntity>(serviceBusClient, FeePaymentEntity.TopicName, queue,
                TopicQueueType.Permanent);
        } 
    }
}
