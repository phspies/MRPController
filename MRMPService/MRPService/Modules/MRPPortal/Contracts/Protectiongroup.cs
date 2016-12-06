using Newtonsoft.Json;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPProtectiongroupType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("group")]
        public string group { get; set; }
        [JsonProperty("supportservice_id")]
        public string supportservice_id { get; set; }
        [JsonProperty("position")]
        public int position { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("protectiongrouptype")]
        public string protectiongrouptype { get; set; }
        [JsonProperty("currentstep")]
        public int currentstep { get; set; }
        [JsonProperty("recoverypolicy_id")]
        public string recoverypolicy_id { get; set; }
        [JsonProperty("repository_workload_id")]
        public string repository_workload_id { get; set; }
        [JsonProperty("recoverypolicy")]
        public MRPRecoverypolicyType recoverypolicy { get; set; }
        [JsonProperty("workload")]
        public MRPWorkloadType workload { get; set; }
    }
}
