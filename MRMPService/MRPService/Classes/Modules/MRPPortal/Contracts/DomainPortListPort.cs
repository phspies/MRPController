using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPPortListPortType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("domainportlist_id")]
        public string domainportlist_id { get; set; }
        [JsonProperty("begin_port")]
        public int? begin_port { get; set; }
        [JsonProperty("end_port")]
        public int? end_port { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
