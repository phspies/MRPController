using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPWorkloadSoftwareType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("caption")]
        public string caption { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("installlocation")]
        public string installlocation { get; set; }
        [JsonProperty("installstate")]
        public int installstate { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("vendor")]
        public string vendor { get; set; }
        [JsonProperty("version")]
        public string version { get; set; }
    }
}
