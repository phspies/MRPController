using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPWorkloadVolumeType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("diskindex")]
        public int? diskindex { get; set; }
        [JsonProperty("driveletter")]
        public string driveletter { get; set; }
        [JsonProperty("volumesize")]
        public Int64? volumesize { get; set; }
        [JsonProperty("volumefreespace")]
        public Int64? volumefreespace { get; set; }
        [JsonProperty("provisioned")]
        public bool? provisioned { get; set; }
        [JsonProperty("serialnumber")]
        public string serialnumber { get; set; }
        [JsonProperty("volumename")]
        public string volumename { get; set; }
        [JsonProperty("deviceid")]
        public string deviceid { get; set; }
        [JsonProperty("blocksize")]
        public Int64? blocksize { get; set; }
        [JsonProperty("filesystem_type")]
        public string filesystem_type { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("platformstoragetier_id")]
        public String platformstoragetier_id { get; set; }
    }
}
