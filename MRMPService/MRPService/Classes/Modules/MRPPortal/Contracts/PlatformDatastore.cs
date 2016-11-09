using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPPlatformdatastoreType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("datastore")]
        public string datastore { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("rp4vm_moid")]
        public string rp4vm_moid { get; set; }
        [JsonProperty("totalcapacity")]
        public long? totalcapacity { get; set; }
        [JsonProperty("freecapacity")]
        public long? freecapacity { get; set; }
        [JsonProperty("rp4vm_clusterid")]
        public long? rp4vm_clusterid { get; set; }
        [JsonProperty("rp4vm_arrayid")]
        public long? rp4vm_arrayid { get; set; }
        [JsonProperty("rp4vm_resourcepoolid")]
        public long? rp4vm_resourcepoolid { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
