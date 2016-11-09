using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPPlatformdomainsCRUDType
    {
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
        [JsonProperty("domainnatrules_attributes")]
        public List<MRPDomainNATRuleType> domainnatrules_attributes { get; set; }
        [JsonProperty("domainfwrules_attributes")]
        public List<MRPDomainFWRuleType> domainfwrules_attributes { get; set; }
        [JsonProperty("domainportlists_attributes")]
        public List<MRPDomainPortType> domainportlists_attributes { get; set; }
        [JsonProperty("domainiplists_attributes")]
        public List<MRPDomainIPType> domainiplists_attributes { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }

        public bool ShouldSerializeplatformnetworks_attributes()
        {
            return (platformnetworks_attributes == null || !platformnetworks_attributes.Any()) ? false : true;
        }
        public bool ShouldSerializedomainnatrules_attributes()
        {
            return (domainnatrules_attributes == null || !domainnatrules_attributes.Any()) ? false : true;
        }
        public bool ShouldSerializedomainportlists_attributes()
        {
            return (domainportlists_attributes == null || !domainportlists_attributes.Any()) ? false : true;
        }
        public bool ShouldSerializedomainiplists_attributes()
        {
            return (domainiplists_attributes == null || !domainiplists_attributes.Any()) ? false : true;
        }
        public bool ShouldSerializedomainfwrules_attributes()
        {
            return (domainfwrules_attributes == null || !domainfwrules_attributes.Any()) ? false : true;
        }
    }
}
