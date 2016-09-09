using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformdomainsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("platformdomain")]
        public MRPPlatformdomainType platformdomain { get; set; }
    }
    public class MRPPlatformdomainListType
    {
        [JsonProperty("platformdomains")]
        public List<MRPPlatformdomainType> platformdomains { get; set; }
    }
    public class MRPPlatformdomainType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("domain")]
        public string domain { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("platformnetworks_attributes")]
        public List<MRPPlatformnetworkType> platformnetworks_attributes { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }

    }
}
