using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPWorkloadInterfaceType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("vnic")]
        public int vnic { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
        [JsonProperty("ipv6address")]
        public string ipv6address { get; set; }
        [JsonProperty("netmask")]
        public string netmask { get; set; }
        [JsonProperty("ipv6netmask")]
        public string ipv6netmask { get; set; }
        [JsonProperty("platformnetwork_id")]
        public string platformnetwork_id { get; set; }
        [JsonProperty("ipassignment")]
        public string ipassignment { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("caption")]
        public string caption { get; set; }
        [JsonProperty("connection_index")]
        public int connection_index { get; set; }
        [JsonProperty("connection_id")]
        public string connection_id { get; set; }
        [JsonProperty("macaddress")]
        public string macaddress { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("platformnetwork")]
        public MRPPlatformnetworkType platformnetwork { get; set; }
    }
}
