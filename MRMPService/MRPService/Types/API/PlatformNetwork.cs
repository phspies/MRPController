using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
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
        public MRPPlatformnetworkCRUDType platformnetwork { get; set; }
    }
    public class MRPPlatformnetworkListType
    {
        public List<MRPPlatformnetworkType> platformnetworks { get; set; }
    }
    public class MRPPlatformnetworkCRUDType
    {
        public string id { get; set; }
        public string platformdomain_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4subnet { get; set; }
        public int ipv4netmask { get; set; }
        public string moid { get; set; }
        public bool provisioned { get; set; }
        public object ipv6subnet { get; set; }
        public object ipv6netmask { get; set; }
        public object networkdomain_moid { get; set; }
        public object networkdomain_name { get; set; }
    }
    public class MRPPlatformnetworkType
    {
        public string id { get; set; }
        public string platformdomain_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4subnet { get; set; }
        public int ipv4netmask { get; set; }
        public string networktype { get; set; }
        public string moid { get; set; }
        public bool provisioned { get; set; }
        public object ipv6subnet { get; set; }
        public object ipv6netmask { get; set; }
        public object networkdomain_moid { get; set; }
        public object networkdomain_name { get; set; }
    }
}
