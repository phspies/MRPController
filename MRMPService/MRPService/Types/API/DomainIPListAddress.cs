using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
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
    }
}
