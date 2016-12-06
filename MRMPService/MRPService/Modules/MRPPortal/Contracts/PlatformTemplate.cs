using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPPlatformtemplatesCRUDType
    {
        [JsonProperty("platformtemplate")]
        public MRPPlatformtemplateType platformtemplate { get; set; }
    }
    public class MRPPlatformtemplateListType
    {
        [JsonProperty("platformtemplates")]
        public List<MRPPlatformtemplateType> platformtemplates { get; set; }
    }
    public class MRPPlatformtemplateType
        {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("platform_moid")]
        public string platform_moid { get; set; }
        [JsonProperty("image_moid")]
        public string image_moid { get; set; }
        [JsonProperty("image_name")]
        public string image_name { get; set; }
        [JsonProperty("image_description")]
        public string image_description { get; set; }
        [JsonProperty("image_type")]
        public string image_type { get; set; }
        [JsonProperty("os_id")]
        public string os_id { get; set; }
        [JsonProperty("os_displayname")]
        public string os_displayname { get; set; }
        [JsonProperty("os_type")]
        public string os_type { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }

}
