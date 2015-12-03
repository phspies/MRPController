using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPlatformnetworksCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPlatformnetworkCRUDType platformnetwork { get; set; }
    }
    public class MoveyPlatformnetworkListType
    {
        public List<MoveyPlatformnetworkType> platformnetworks { get; set; }
    }
    public class MoveyPlatformnetworkCRUDType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4subnet { get; set; }
        public int ipv4netmask { get; set; }
        public string moid { get; set; }
        public bool provisioned { get; set; }
        public object ipv6subnet { get; set; }
        public object ipv6netmask { get; set; }
        public object networkdomain_moid { get; set; }
    }
    public class MoveyPlatformnetworkType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
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
    }
}
