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
        [DataMember(Order = 5)]public string SettlementOperationId { get; set; }
        [DataMember(Order = 6)]public string AssetId { get; set; }
        [DataMember(Order = 7)]public SettlementStatus Status { get; set; }
        [DataMember(Order = 8)]public DateTime PaymentTimestamp { get; set; }
        [DataMember(Order = 9)]public string ErrorMessage { get; set; }



    }
}