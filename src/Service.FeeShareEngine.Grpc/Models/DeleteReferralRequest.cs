using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class DeleteReferralRequest
    {
        [DataMember(Order = 1)]
        public string ClientId { get; set; }
    }
}