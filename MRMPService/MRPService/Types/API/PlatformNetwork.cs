using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformnetworksCRUDType : MRPManagerIDType
    {
        [JsonProperty("platformnetwork")]
        public MRPPlatformnetworkType platformnetwork { get; set; }
    }
    public class MRPPlatformnetworkListType
    {
        [JsonProperty("platformnetworks")]
        public List<MRPPlatformnetworkType> platformnetworks { get; set; }
    }
    public class MRPPlatformnetworkType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platformdomain_id")]
        public string platformdomain_id { get; set; }
        [JsonProperty("network")]
        public string network { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("ipv4subnet")]
        public string ipv4subnet { get; set; }
        [JsonProperty("ipv4netmask")]
        public int ipv4netmask { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("provisioned")]
        public bool provisioned { get; set; }
        [JsonProperty("ipv6subnet")]
        public string ipv6subnet { get; set; }
        [JsonProperty("ipv6netmask")]
        public int ipv6netmask { get; set; }
        [JsonProperty("networkdomain_moid")]
        public string networkdomain_moid { get; set; }
        [JsonProperty("networkdomain_name")]
        public string networkdomain_name { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
