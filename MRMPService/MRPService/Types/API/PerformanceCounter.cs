using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPerformanceCountersCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("performancecounters")]
        public List<MRPPerformanceCounterCRUDType> performancecounters { get; set; }
    }
    public class MRPPerformanceCounterCRUDType
    {
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("performancecategory_id")]
        public string performancecategory_id { get; set; }
        [JsonProperty("instance")]
        public string instance { get; set; }
        [JsonProperty("value")]
        public double value { get; set; }       
    }
}
