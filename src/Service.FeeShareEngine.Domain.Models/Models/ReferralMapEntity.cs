using System;
using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class ReferralMapEntity
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 3)] public string FeeShareGroupId { get; set; }
        [DataMember(Order = 4)] public DateTime LastTs { get; set; }
    }
}