using System.ServiceModel;
using System.Threading.Tasks;
using Service.FeeShareEngine.Grpc.Models;

namespace Service.FeeShareEngine.Grpc
{
    [ServiceContract]
    public interface IFeesService
    {
        [OperationContract]
        Task<GetAllStatsResponse> GetAllStatsAsync();
        
        [OperationContract]
        Task<GetAllFeePaymentsResponse> GetAllFeePaymentsAsync();
        
        [OperationContract]
        Task<GetAllFeeSharesResponse> GetAllFeeSharesAsync();
    }
}