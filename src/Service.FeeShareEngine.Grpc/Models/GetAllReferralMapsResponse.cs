using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.FeeShareEngine.Domain.Models.Models;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class GetAllReferralMapsResponse
    {
        [DataMember(Order = 1)] public List<ReferralMapEntity> Referrals { get; set; }
    }
}