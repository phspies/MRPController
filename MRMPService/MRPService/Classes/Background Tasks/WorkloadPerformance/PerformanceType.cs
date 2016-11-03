using System;

namespace MRMPService.PerformanceCollection
{

    public class PerformanceType
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
