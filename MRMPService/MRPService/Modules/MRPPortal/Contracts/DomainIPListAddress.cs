using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPIPListAddressType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("domainiplist_id")]
        public string domainiplist_id { get; set; }
        [JsonProperty("begin_address")]
        public string begin_address { get; set; }
        [JsonProperty("end_address")]
        public string end_address { get; set; }
        [JsonProperty("prefix_size")]
        public int? prefix_size { get; set; }
        [JsonProperty("ipversion")]
        public string ipversion { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("iptype")]
        public string iptype { get; set; }
    }
}
