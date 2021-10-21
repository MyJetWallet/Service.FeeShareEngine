using System;
using System.Runtime.Serialization;
using Service.FeeShareEngine.Domain.Models;

namespace Service.FeeShareEngine.Postgres.Models
{
    [DataContract]
    public class FeeShareEntity
    {
        public const string TopicName = "jet-wallet-fee-share-transfer";

        [DataMember(Order = 1)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 2)] public decimal FeeShareAmountInUsd { get; set; }
        [DataMember(Order = 3)] public DateTime TimeStamp { get; set; }
        [DataMember(Order = 4)] public string OperationId { get; set; }
        [DataMember(Order = 5)] public string FeeTransferOperationId { get; set; }
        [DataMember(Order = 6)] public decimal FeeAmount { get; set; }
        [DataMember(Order = 7)] public string FeeAsset { get; set; }
        [DataMember(Order = 8)] public decimal FeeAmountInUsd { get; set; }
        [DataMember(Order = 9)] public DateTime PaymentTimestamp { get; set; }
        [DataMember(Order = 10)] public PaymentStatus Status { get; set; }
    }
}