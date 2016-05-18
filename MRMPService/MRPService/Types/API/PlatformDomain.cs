using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformdomainsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPlatformdomainType platformdomain { get; set; }
    }
    public class MRPPlatformdomainListType
    {
        public List<MRPPlatformdomainType> platformdomains { get; set; }
    }
    public class MRPPlatformdomainType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string domain { get; set; }
        public string moid { get; set; }
        public List<MRPPlatformnetworkType> platformnetworks_attributes { get; set; }

    }
}
