using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.FeeShareEngine.Domain.Models.Models;

namespace Service.FeeShareEngine.Grpc.Models
{
    [DataContract]
    public class GetAllStatsResponse
    {
        [DataMember(Order = 1)] public List<ShareStatEntity> Stats { get; set; }
    }
}