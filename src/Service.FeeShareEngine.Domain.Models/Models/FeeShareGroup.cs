using System;
using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class FeeShareGroup
    {
        [DataMember(Order = 1)]public string GroupId { get; set; }
        [DataMember(Order = 2)]public string AssetId { get; set; }
        [DataMember(Order = 3)]public decimal FeePercent { get; set; }
        [DataMember(Order = 4)]public bool IsDefault  { get; set; }
        [DataMember(Order = 5)]public DateTime LastTs  { get; set; }
    }
}