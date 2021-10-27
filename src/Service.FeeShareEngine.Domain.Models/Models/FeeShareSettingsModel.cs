using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Domain.Models.Models
{
    [DataContract]
    public class FeeShareSettingsModel
    {
        [DataMember(Order = 1)] public string FeeShareEngineWalletId { get; set; }
        [DataMember(Order = 2)] public string FeeShareEngineClientId { get; set; }
        [DataMember(Order = 3)] public string FeeShareEngineBrokerId { get; set; }
        [DataMember(Order = 4)] public string FeeShareEngineBrandId { get; set; }
        [DataMember(Order = 5)] public PeriodTypes CurrentPeriod { get; set; }
    }
}