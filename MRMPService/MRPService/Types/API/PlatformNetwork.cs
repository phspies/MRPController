using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformnetworksCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPlatformnetworkType platformnetwork { get; set; }
    }
    public class MRPPlatformnetworkListType
    {
        public List<MRPPlatformnetworkType> platformnetworks { get; set; }
    }
    public class MRPPlatformnetworkType
    {
        public string id { get; set; }
        public string platformdomain_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4subnet { get; set; }
        public int ipv4netmask { get; set; }
        public string moid { get; set; }
        public bool provisioned { get; set; }
        public string ipv6subnet { get; set; }
        public int ipv6netmask { get; set; }
        public string networkdomain_moid { get; set; }
        public string networkdomain_name { get; set; }
    }
}
