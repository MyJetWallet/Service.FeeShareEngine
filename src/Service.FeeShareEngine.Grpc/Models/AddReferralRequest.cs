using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class AddReferralRequest
    {
        [DataMember(Order = 1)]
        public string ClientId { get; set; }
        
        [DataMember(Order = 2)]
        public string ReferrerClientId { get; set; }
        
        [DataMember(Order = 3)]
        public string FeeShareGroupId { get; set; } 
    }
}