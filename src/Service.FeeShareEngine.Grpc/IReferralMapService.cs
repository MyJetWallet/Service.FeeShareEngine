using System.ServiceModel;
using System.Threading.Tasks;
using Service.FeeShareEngine.Grpc.Models;

namespace Service.FeeShareEngine.Grpc
{
    [ServiceContract]
    public interface IReferralMapService
    {
        [OperationContract]
        Task AddReferralLink(AddReferralRequest request);

        [OperationContract]
        Task<GetAllReferralMapsResponse> GetAllReferralMaps();
    }
}