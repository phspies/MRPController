using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
{
    public class MRPPlatformdatacenterCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPlatformdatacenterType platformdatacenter { get; set; }
    }
    public class MRPPlatformdatacenterListType
    {
        public List<MRPPlatformdatacenterType> platformdatacenters { get; set; }
    }
    public class MRPPlatformdatacenterType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string moid { get; set; }
        public int? maxcpu { get; set; }
        public int? maxmemory { get; set; }
        public int? maxdiskcount { get; set; }
        public int? maxdisksize { get; set; }
        public int? maxnetwork { get; set; }
        public int? mindisksize { get; set; }
        public int? minmemory { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string vpn_url { get; set; }
        public string diskspeeds { get; set; }
        public string cpuspeeds { get; set; }
        public bool? deleted { get; set; }
        public string displayname { get; set; }
    }
}
