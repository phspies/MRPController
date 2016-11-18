using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPOrganizationTagType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("tagkeyid")]
        public string tagkeyid { get; set; }
        [JsonProperty("tagkeyname")]
        public string tagkeyname { get; set; }
        [JsonProperty("tagdisplayreport")]
        public bool? tagdisplayreport { get; set; }
        [JsonProperty("tagvaluerequired")]
        public bool? tagvaluerequired { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("tagtype")]
        public string tagtype { get; set; }
        

    }
}
