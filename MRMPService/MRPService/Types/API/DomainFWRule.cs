using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPDomainFWRuleType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdomain_id")]
        public string platformdomain_id { get; set; }
        [JsonProperty("rulename")]
        public string rulename { get; set; }
        [JsonProperty("action")]
        public string action { get; set; }
        [JsonProperty("ipversion")]
        public string ipversion { get; set; }
        [JsonProperty("protocol")]
        public string protocol { get; set; }
        [JsonProperty("source_ip_type")]
        public string source_ip_type { get; set; }
        [JsonProperty("source_portlist_id")]
        public string source_portlist_id { get; set; }
        [JsonProperty("source_domainiplist_id")]
        public string source_domainiplist_id { get; set; }
        [JsonProperty("source_workload_id")]
        public string source_workload_id { get; set; }
        [JsonProperty("source_ips")]
        public string source_ips { get; set; }
        [JsonProperty("source_ips_prefix")]
        public int? source_ips_prefix { get; set; }
        [JsonProperty("source_begin_port")]
        public int? source_begin_port { get; set; }
        [JsonProperty("source_end_port")]
        public int? source_end_port { get; set; }

        [JsonProperty("target_ip_type")]
        public string target_ip_type { get; set; }
        [JsonProperty("target_workload_id")]
        public string target_workload_id { get; set; }
        [JsonProperty("target_ips")]
        public string target_ips { get; set; }
        [JsonProperty("target_domainiplist_id")]
        public string target_domainiplist_id { get; set; }
        [JsonProperty("target_portlist_id")]
        public string target_portlist_id { get; set; }
        [JsonProperty("target_ips_prefix")]
        public int? target_ips_prefix { get; set; }
        [JsonProperty("target_begin_port")]
        public int? target_begin_port { get; set; }
        [JsonProperty("target_end_port")]
        public int? target_end_port { get; set; }
        [JsonProperty("placement")]
        public string placement { get; set; }
        [JsonProperty("enabled")]
        public bool? enabled { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
    }
}
