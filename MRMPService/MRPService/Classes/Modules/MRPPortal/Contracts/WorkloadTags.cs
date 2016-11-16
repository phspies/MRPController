using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPWorkloadTagType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("organizationtag_id")]
        public string organizationtag_id { get; set; }
        [JsonProperty("_destroy")]
        public bool? _destroy { get; set; }
        
    }
}
