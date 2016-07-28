using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPWorkloadDiskType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("diskindex")]
        public int diskindex { get; set; }
        [JsonProperty("disksize")]
        public Int64 disksize { get; set; }
        [JsonProperty("platformstoragetier_id")]
        public string platformstoragetier_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("position")]
        public int position { get; set; }
        [JsonProperty("provisioned")]
        public bool provisioned { get; set; }
        [JsonProperty("deviceid")]
        public string deviceid { get; set; }
        [JsonProperty("_destroy")]
        public bool _destroy { get; set; }
        [JsonProperty("platformstoragetier")]
        public MRPPlatformStorageTierType platformstoragetier { get; set; }
    }
}
