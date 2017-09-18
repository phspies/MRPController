using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPPlatformclusterType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdatacenter_id")]
        public string platformdatacenter_id { get; set; }
        [JsonProperty("cluster")]
        public string cluster { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("resourcepool_moid")]
        public string resourcepool_moid { get; set; }
        [JsonProperty("hostcount")]
        public int? hostcount { get; set; }
        [JsonProperty("networkcount")]
        public int? networkcount { get; set; }
        [JsonProperty("totalcpu")]
        public int? totalcpu { get; set; }
        [JsonProperty("totalmemory")]
        public long? totalmemory { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
