using Newtonsoft.Json;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPDomainAffinityRuleType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdomain_id")]
        public string platformdomain_id { get; set; }
        [JsonProperty("workload1_id")]
        public string workload1_id { get; set; }
        [JsonProperty("workload2_id")]
        public string workload2_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("affinitytype")]
        public string affinitytype { get; set; }
   
    }
}
