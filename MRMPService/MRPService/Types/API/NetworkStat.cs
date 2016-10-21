using Newtonsoft.Json;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPNetworkStatsCRUDType : MRPManagerIDType
    {
        [JsonProperty("networkstat")]
        public MRPNetworkStatCRUDType networkstat { get; set; }
    }
    public class MRPNetworkStatsBulkCRUDType : MRPManagerIDType
    {
        [JsonProperty("networkstats")]
        public List<MRPNetworkStatCRUDType> networkstats { get; set; }
    }
    public class MRPNetworkStatCRUDType
    {
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("proto")]
        public string proto { get; set; }
        [JsonProperty("source_ip")]
        public string source_ip { get; set; }
        [JsonProperty("source_port")]
        public int source_port { get; set; }
        [JsonProperty("target_ip")]
        public string target_ip { get; set; }
        [JsonProperty("target_port")]
        public int target_port { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("pid")]
        public int pid { get; set; }
        [JsonProperty("process")]
        public string process { get; set; }

    }

}
