using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    class Disk
    {
        public string id { get; set; }
        public string moid { get; set; }
        public string workload_id { get; set; }
        public int index { get; set; }
        public string tier { get; set; }
        public long total_capacity { get; set; }
        public long total_free { get; set; }
        public string driveletter { get; set; }
        public string mountpoint { get; set; }
    }
}
