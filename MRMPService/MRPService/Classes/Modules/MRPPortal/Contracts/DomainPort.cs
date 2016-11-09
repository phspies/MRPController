using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPDomainPortType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdomain_id")]
        public string platformdomain_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("begin_port")]
        public int? begin_port { get; set; }
        [JsonProperty("end_port")]
        public int? end_port { get; set; }
        [JsonProperty("created_time")]
        public DateTime? created_time { get; set; }
        [JsonProperty("porttype")]
        public string porttype { get; set; }
        [JsonProperty("domainportlist")]
        public List<MRPDomainPortType> domainportlist { get; set; }
        [JsonProperty("domainportlistports_attributes")]
        public List<MRPPortListPortType> domainportlistports_attributes { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }

    }
}
