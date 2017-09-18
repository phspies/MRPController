using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPOrganizationCRUDType
    {
        [JsonProperty("organization")]
        public MRPOrganizationType organization { get; set; }
    }
    public class MRPOrganizationType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organizationtags_attributes")]
        public List<MRPOrganizationTagType> organizationtags { get; set; }
        public bool ShouldSerializeorganizationtags()
        {
            return (organizationtags == null || !organizationtags.Any()) ? false : true;
        }
    }
}
