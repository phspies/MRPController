using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPDomainIPType
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
        [JsonProperty("begin_address")]
        public string begin_address { get; set; }
        [JsonProperty("end_address")]
        public string end_address { get; set; }
        [JsonProperty("ipversion")]
        public string ipversion { get; set; }
        [JsonProperty("prefix_size")]
        public int? prefix_size { get; set; }
        [JsonProperty("created_time")]
        public DateTime? created_time { get; set; }
        [JsonProperty("iptype")]
        public string iptype { get; set; }
        [JsonProperty("domainiplist")]
        public List<MRPDomainIPType> domainiplist { get; set; }
        [JsonProperty("domainiplistaddresses_attributes")]
        public List<MRPIPListAddressType> domainiplistaddresses { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
