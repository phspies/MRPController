using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPerformancesCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPerformanceCRUDType performancecounter { get; set; }
    }
    public class MoveyPerformanceCRUDType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public DateTime timestamp { get; set; }
        public string category_name { get; set; }
        public string counter_name { get; set; }
        public string instance { get; set; }
        public double value { get; set; }       
    }
}
