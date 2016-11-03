using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPNetworkFlowsCRUDType
    {
        [JsonProperty("networkflows")]
        public List<MRPNetworkFlowCRUDType> networkflows { get; set; }
    }
    public class MRPNetworkFlowCRUDType
    {
        [JsonProperty("source_address")]
        public string source_address { get; set; }
        [JsonProperty("target_address")]
        public string target_address { get; set; }
        [JsonProperty("source_port")]
        public int source_port { get; set; }
        [JsonProperty("target_port")]
        public int target_port { get; set; }
        [JsonProperty("protocol")]
        public int protocol { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("start_timestamp")]
        public long start_timestamp { get; set; }
        [JsonProperty("stop_timestamp")]
        public long stop_timestamp { get; set; }
        [JsonProperty("packets")]
        public int packets { get; set; }
        [JsonProperty("kbyte")]
        public int kbyte { get; set; }
        [JsonProperty("process")]
        public string process { get; set; }
        [JsonProperty("pid")]
        public int pid { get; set; }
    }

}
