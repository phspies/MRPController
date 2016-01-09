using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyWorkloadInterfaceType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int vnic { get; set; }
        public string ipaddress { get; set; }
        public string ipv6address { get; set; }
        public string netmask { get; set; }
        public string ipv6netmask { get; set; }
        public string platformnetwork_id { get; set; }
        public string ipassignment { get; set; }
        public string moid { get; set; }
        public int connection_index { get; set; }
        public string connection_id { get; set; }
        public MoveyPlatformnetworkType platformnetwork { get; set; }
    }
}
