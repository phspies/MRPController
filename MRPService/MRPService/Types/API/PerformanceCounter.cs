using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPPerformanceCountersCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MRPPerformanceCounterCRUDType performancecounter { get; set; }
    }
    public class MRPPerformanceCounterCRUDType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public DateTime timestamp { get; set; }
        public string performancecategory_id { get; set; }
        public string instance { get; set; }
        public double value { get; set; }       
    }
}
