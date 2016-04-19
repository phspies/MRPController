using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
{
    public class MRPWorkloadSoftwareType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        public string installlocation { get; set; }
        public int installstate { get; set; }
        public string name { get; set; }
        public string vendor { get; set; }
        public string version { get; set; }
    }
}
