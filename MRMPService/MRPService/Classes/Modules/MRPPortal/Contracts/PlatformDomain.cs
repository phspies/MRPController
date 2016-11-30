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
        [JsonProperty("domaintype")]
        public string domaintype { get; set; }
        [JsonProperty("snatip4address")]
        public string snatip4address { get; set; }
        [JsonProperty("platformnetworks_attributes")]
        public List<MRPPlatformnetworkType> platformnetworks { get; set; }
        [JsonProperty("domainnatrules_attributes")]
        public List<MRPDomainNATRuleType> domainnatrules { get; set; }
        [JsonProperty("domainfwrules_attributes")]
        public List<MRPDomainFWRuleType> domainfwrules { get; set; }
        [JsonProperty("domainportlists_attributes")]
        public List<MRPDomainPortType> domainportlists { get; set; }
        [JsonProperty("domainiplists_attributes")]
        public List<MRPDomainIPType> domainiplists { get; set; }
        [JsonProperty("domainaffinityrules_attributes")]
        public List<MRPDomainAffinityRuleType> domainaffinityrules { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
    }
}
