using System;
using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class FeeShareEntity
    {
        public const string TopicName = "jet-wallet-fee-share-transfer";

        [DataMember(Order = 1)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 2)] public decimal FeeAmount { get; set; }
        [DataMember(Order = 3)] public string FeeAsset { get; set; }
        [DataMember(Order = 4)] public decimal FeeShareAmountInFeeAsset { get; set; }
        [DataMember(Order = 5)] public decimal FeeShareAmountInTargetAsset { get; set; }
        [DataMember(Order = 6)] public DateTime TimeStamp { get; set; }
        [DataMember(Order = 7)] public string OperationId { get; set; }
        [DataMember(Order = 8)] public string FeeTransferOperationId { get; set; }
        [DataMember(Order = 9)] public DateTime PaymentTimestamp { get; set; }
        [DataMember(Order = 10)] public PaymentStatus Status { get; set; }
        [DataMember(Order = 11)] public string ConverterWalletId { get; set; }
        [DataMember(Order = 12)] public string FeeShareWalletId { get; set; }
        [DataMember(Order = 13)] public string BrokerId { get; set; }
        [DataMember(Order = 14)] public string ErrorMessage { get; set; }
        [DataMember(Order = 15)] public string FeeShareAsset { get; set; }
        [DataMember(Order = 16)] public decimal FeeToTargetConversionRate { get; set; }
    }
}