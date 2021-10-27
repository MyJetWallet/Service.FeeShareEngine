using System.Runtime.Serialization;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class DeleteGroupRequest
    {
        [DataMember(Order = 1)] public string FeeShareGroupId { get; set; }
    }
}