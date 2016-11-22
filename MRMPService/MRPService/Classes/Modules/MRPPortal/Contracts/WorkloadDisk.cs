using Newtonsoft.Json;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPWorkloadDiskType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("diskindex")]
        public int? diskindex { get; set; }
        [JsonProperty("disksize")]
        public int? disksize { get; set; }
        [JsonProperty("platformstoragetier_id")]
        public string platformstoragetier_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("provisioned")]
        public bool? provisioned { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        

    }
}
