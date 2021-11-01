using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class ReferralMapChangeMessage
    {
        public const string TopicName = "jet-wallet-fee-shares-payment";
        
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string FeeShareGroupId { get; set; }

    }
}