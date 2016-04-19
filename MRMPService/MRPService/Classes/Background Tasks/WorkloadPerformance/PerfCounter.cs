using System.Collections.Generic;

namespace MRMPService.PerformanceCollection
{
    public class PerfCounter
    {
        public string category { get; set; }
        public string counter { get; set; }
        public List<InstanceCounters> instances { get; set; }
    }
}
