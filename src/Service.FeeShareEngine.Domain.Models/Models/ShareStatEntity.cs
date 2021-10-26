using System;
using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class ShareStatEntity
    {
        [DataMember(Order = 1)]public decimal Amount { get; set; }
        [DataMember(Order = 2)]public DateTime PeriodFrom { get; set; }
        [DataMember(Order = 3)]public DateTime PeriodTo { get; set; }
        [DataMember(Order = 4)]public DateTime CalculationTimestamp { get; set; }
    }
}