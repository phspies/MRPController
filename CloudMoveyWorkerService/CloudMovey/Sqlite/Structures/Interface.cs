using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    public class Interface
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public string moid { get; set; }
        public string index { get; set; }
        public string platform_network_id { get; set; }
        public string ipv4address { get; set; }
        public string ipv4netmask { get; set; }
        public string ipv4gateway { get; set; }
        public string ipv6address { get; set; }
        public string ipv6netmask { get; set; }
        public string ipv6gateway { get; set; }

    }
}
