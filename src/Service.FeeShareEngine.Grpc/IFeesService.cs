using System.ServiceModel;
using System.Threading.Tasks;
using Service.FeeShareEngine.Grpc.Models;

namespace Service.FeeShareEngine.Grpc
{
    [ServiceContract]
    public interface IFeesService
    {
        [OperationContract]
        Task<GetAllStatsResponse> GetAllStatsAsync(PaginationRequest request);
        
        [OperationContract]
        Task<GetAllFeePaymentsResponse> GetAllFeePaymentsAsync(PaginationRequest request);
        
        [OperationContract]
        Task<GetAllFeeSharesResponse> GetAllFeeSharesAsync(PaginationRequest request);
        
        [OperationContract]
        Task<OperationResponse> RetryFailedFeePayments();
        
        [OperationContract]
        Task<OperationResponse> RetryFailedFeeSettlements();
    }
}