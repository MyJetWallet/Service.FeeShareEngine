using System;
using System.Runtime.Serialization;
using Service.FeeShareEngine.Domain.Models;

namespace Service.FeeShareEngine.Postgres.Models
{
    [DataContract]
    public class FeePaymentEntity
    {
        public const string TopicName = "jet-wallet-fee-shares-payment";

        [DataMember(Order = 1)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 2)] public decimal Amount { get; set; }
        [DataMember(Order = 3)] public DateTime PeriodFrom { get; set; }
        [DataMember(Order = 4)] public DateTime PeriodTo { get; set; }
        [DataMember(Order = 5)] public DateTime CalculationTimestamp { get; set; }
        [DataMember(Order = 6)] public DateTime PaymentTimestamp { get; set; }
        [DataMember(Order = 7)] public PaymentStatus Status { get; set; }
        [DataMember(Order = 8)] public string PaymentOperationId { get; set; }
    }
}