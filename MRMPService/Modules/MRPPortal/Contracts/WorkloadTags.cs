using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPWorkloadTagType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("organizationtag_id")]
        public string organizationtag_id { get; set; }
        [JsonProperty("tagvalue")]
        public string tagvalue { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }

    }
}
