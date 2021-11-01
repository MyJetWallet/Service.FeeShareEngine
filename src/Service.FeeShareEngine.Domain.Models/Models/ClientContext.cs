using System;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    public class ClientContext
    {
        public string ClientId { get; set; }
        public string WalletId { get; set; }
        public string BrokerId { get; set; }
        public string ReferrerClientId { get; set; }
        public DateTime TimeStamp { get; set; }
        public FeeShareGroup FeeShareGroup { get; set; }

        public ClientContext(string clientId, string walletId, string brokerId, string referrerClientId, FeeShareGroup feeShareGroup)
        {
            ClientId = clientId;
            WalletId = walletId;
            BrokerId = brokerId;
            ReferrerClientId = referrerClientId;
            TimeStamp = DateTime.UtcNow;
            FeeShareGroup = feeShareGroup;
        }
    }
}