using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformGETType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
    }
    public class MRPPlatformsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("platform")]
        public MRPPlatformType platform { get; set; }
    }
    public class MRPPlatformListType
    {
        [JsonProperty("platforms")]
        public List<MRPPlatformType> platforms { get; set; }
    }
    public class MRPPlatformType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("credential_id")]
        public string credential_id { get; set; }
        [JsonProperty("platform")]
        public string platform { get; set; }
        [JsonProperty("enabled")]
        public bool? enabled { get; set; }
        [JsonProperty("platformtype")]
        public string platformtype { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("manager_id")]
        public string manager_id { get; set; }
        [JsonProperty("platform_version")]
        public string platform_version { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("default_credential_id")]
        public string default_credential_id { get; set; }
        [JsonProperty("credential")]
        public MRPCredentialType credential { get; set; }
        [JsonProperty("platformdomains_attributes")]
        public List<MRPPlatformdomainType> platformdomains_attributes { get; set; }
        [JsonProperty("platformdatacenters_attributes")]
        public List<MRPPlatformdatacenterType> platformdatacenters_attributes { get; set; }
    }
}

