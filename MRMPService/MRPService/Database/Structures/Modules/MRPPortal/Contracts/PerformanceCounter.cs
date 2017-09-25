using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPPerformanceCountersCRUDType
    {
        [JsonProperty("performancecounters")]
        public List<MRPPerformanceCounterCRUDType> performancecounters { get; set; }
    }
    public class MRPPerformanceCounterCRUDType
    {
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("category")]
        public string category { get; set; }
        [JsonProperty("counter")]
        public string counter { get; set; }
        [JsonProperty("instance")]
        public string instance { get; set; }
        [JsonProperty("value")]
        public double value { get; set; }       
    }
}
