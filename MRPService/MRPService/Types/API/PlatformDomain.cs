using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPPlatformdomainsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MRPPlatformdomainCRUDType platformdomain { get; set; }
    }
    public class MRPPlatformdomainListType
    {
        public List<MRPPlatformdomainType> platformdomains { get; set; }
    }
    public class MRPPlatformdomainCRUDType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string domain { get; set; }
        public string moid { get; set; }
        public List<MRPPlatformnetworkCRUDType> platformnetworks_attributes { get; set; }

    }
    public class MRPPlatformdomainType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string domain { get; set; }
        public string moid { get; set; }

    }
}
