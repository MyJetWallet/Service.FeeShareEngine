using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class PaginationRequest
    {
        [DataMember(Order = 1)] public int Take { get; set; }
        [DataMember(Order = 2)] public int Skip { get; set; }
        [DataMember(Order = 3)] public string SearchText { get; set; }

    }
}