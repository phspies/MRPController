using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    
    public class Performance
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
