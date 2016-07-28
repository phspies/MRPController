using Newtonsoft.Json;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPerformanceCategoriesCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("performancecategory")]
        public MRPPerformanceCategoryCRUDType performancecategory { get; set; }
    }
    public class MRPPerformanceCategoryCRUDType
    {
        [JsonProperty("category_name")]
        public string category_name { get; set; }
        [JsonProperty("counter_name")]
        public string counter_name { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("instances")]
        public bool instances { get; set; }
    }
    public class MRPPerformanceCategoryListType
    {
        [JsonProperty("performancecategories")]
        public List<MRPPerformanceCategoryType> performancecategories { get; set; }
    }
    public class MRPPerformanceCategoryType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("category_name")]
        public string category_name { get; set; }
        [JsonProperty("counter_name")]
        public string counter_name { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("instances")]
        public bool instances { get; set; }

    }
}
