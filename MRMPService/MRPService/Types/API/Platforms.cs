using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string platform_id { get; set; }
    }
    public class MRPPlatformsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPlatformType platform { get; set; }
    }

    public class MRPPlatformListType
    {
        public List<MRPPlatformType> platforms { get; set; }
    }
    public class MRPPlatformType
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string credential_id { get; set; }
        public string platform { get; set; }
        public bool? enabled { get; set; }
        public string platformtype { get; set; }
        public string url { get; set; }
        public string moid { get; set; }
        public string manager_id { get; set; }
        public string platform_version { get; set; }
        public bool? deleted { get; set; }
        public MRPCredentialType credential;
        public List<MRPPlatformdomainType> platformdomains_attributes { get; set; }
        public List<MRPPlatformdatacenterType> platformdatacenters_attributes { get; set; }
    }
}

