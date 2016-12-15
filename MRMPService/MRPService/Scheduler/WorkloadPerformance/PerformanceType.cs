using System;

namespace MRMPService.Scheduler.PerformanceCollection
{

    public class PerformanceType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public DateTime timestamp { get; set; }
        public string category { get; set; }
        public string counter { get; set; }
        public string instance { get; set; }
        public double value { get; set; }       
    }
}
