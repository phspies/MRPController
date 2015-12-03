using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    class Network
    {
        public string id { get; set; }
        public string moid { get; set; }
        public string platform_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4range { get; set; }
        public string ipv4mask { get; set; }
        public string ipv6range { get; set; }
        public string ipv6mask { get; set; }
    }
}
