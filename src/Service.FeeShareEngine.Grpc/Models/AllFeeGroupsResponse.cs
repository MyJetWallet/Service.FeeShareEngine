using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.FeeShareEngine.Domain.Models.Models;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class AllFeeGroupsResponse
    {
        [DataMember(Order = 1)] public List<FeeShareGroup> Groups;
    }
}