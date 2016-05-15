using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
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
        public MRPPlatformCRUDType platform { get; set; }
    }
    public class MRPPlatformCRUDType
    {
        public string id { get; set; }
        public string platform { get; set; }
        public string mapping { get; set; }
        public bool? enabled { get; set; }
        public string credential_id { get; set; }
        public string manager_id { get;  set;}
        public string platformtype { get; set; }
        public string moid { get; set; }
        public string platform_version { get; set; }
        public string hash_value { get; set; }
        public bool deleted { get; set; }
        public string dc_discovery_list { get; set; }
        public bool dc_discovery_status { get; set; }
        public string dc_discovery_message { get; set; }

    }
    public class MRPPlatformListType
    {
        public List<MRPPlatformType> platforms { get; set; }
    }
    public class MRPPlatformType
    {
        public string id { get; set; }
        public string platform { get; set; }
        public string mapping { get; set; }
        public bool? enabled { get; set; }
        public string credential_id { get; set; }
        public string platformtype { get; set; }
        public int? maxcpu { get; set; }
        public int? maxmemory { get; set; }
        public string url { get; set; }
        public string manager_id { get; set; }
        public string moid { get; set; }
        public string platform_version { get; set; }
        public List<MRPPlatformnetworkType> platformnetworks { get; set; }
        public int? maxdiskcount { get; set; }
        public int? maxdisksize { get; set; }
        public int? maxnetwork { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string mcpendpoint_id { get; set; }
        public string organization_id { get; set; }
        public string hash_value { get; set; }
        public bool deleted { get; set; }
        public string dc_discovery_list { get; set; }
        public bool dc_discovery_status { get; set; }
        public string dc_discovery_message { get; set; }

        public MRPCredentialType credential;

    }


}

