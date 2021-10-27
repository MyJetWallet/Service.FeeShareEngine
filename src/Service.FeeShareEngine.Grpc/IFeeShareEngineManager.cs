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
        Task<GetAllReferralMapsResponse> GetAllReferralMaps();
        
        [OperationContract]
        Task<AllFeeGroupsResponse> GetAllFeeShareGroups();
        
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