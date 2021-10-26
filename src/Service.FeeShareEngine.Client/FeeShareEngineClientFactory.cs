using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.FeeShareEngine.Grpc;

namespace Service.FeeShareEngine.Client
{
    [UsedImplicitly]
    public class FeeShareEngineClientFactory: MyGrpcClientFactory
    {
        public FeeShareEngineClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IReferralMapService GetReferralMapService() => CreateGrpcService<IReferralMapService>();
        
        public IFeesService GetFeesService() => CreateGrpcService<IFeesService>();

    }
}
