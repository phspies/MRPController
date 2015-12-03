using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPlatformsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPlatformCRUDType platform { get; set; }
    }
    public class MoveyPlatformCRUDType
    {
        public string id { get; set; }
        public string platform { get; set; }
        public string mapping { get; set; }
        public bool? enabled { get; set; }
        public string credential_id { get; set; }
        public string worker_id { get;  set;}
        public string platformtype { get; set; }
        public string moid { get; set; }
        public string platform_version { get; set; }
        public string hash_value { get; set; }

    }
    public class MoveyPlatformListType
    {
        public List<MoveyPlatformType> platforms { get; set; }
    }
    public class MoveyPlatformType
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
        public string worker_id { get; set; }
        public string moid { get; set; }
        public string platform_version { get; set; }
        public List<MoveyPlatformnetworkType> platformnetworks { get; set; }
        public int? maxdiskcount { get; set; }
        public int? maxdisksize { get; set; }
        public int? maxnetwork { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string mcpendpoint_id { get; set; }
        public MoveyTaskMcpendpointType mcpendpoint { get; set; }
        public string organization_id { get; set; }
        public string hash_value { get; set; }

    }


}

