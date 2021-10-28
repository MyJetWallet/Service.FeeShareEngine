using System.ServiceModel;
using System.Threading.Tasks;
using Service.FeeShareEngine.Domain.Models.Models;
using Service.FeeShareEngine.Grpc.Models;

namespace Service.FeeShareEngine.Grpc
{
    [ServiceContract]
    public interface IFeeShareEngineManager
    {
        [OperationContract]
        Task<OperationResponse> AddReferralLink(AddReferralRequest request);

        [OperationContract]
        Task<OperationResponse> DeleteReferralLink(DeleteReferralRequest request);
        
        [OperationContract]
        Task<GetAllReferralMapsResponse> GetAllReferralMaps(PaginationRequest request);
        
        [OperationContract]
        Task<AllFeeGroupsResponse> GetAllFeeShareGroups(PaginationRequest request);
        
        [OperationContract]
        Task<OperationResponse> AddOrUpdateFeeShareGroup(FeeShareGroup request);

        [OperationContract]
        Task<OperationResponse> DeleteFeeShareGroup(DeleteGroupRequest request);
        
        [OperationContract]
        Task<OperationResponse> UpdateFeeShareSettings(FeeShareSettingsModel request);

        [OperationContract]
        Task<FeeShareSettingsModel> GetFeeShareSettings();
    }
}