using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPDomainNATRuleType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdomain_id")]
        public string platformdomain_id { get; set; }
        [JsonProperty("rulename")]
        public string rulename { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("internal_ip")]
        public string internal_ip { get; set; }
        [JsonProperty("external_ip")]
        public string external_ip { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("nattype")]
        public string nattype { get; set; }
    }
}
