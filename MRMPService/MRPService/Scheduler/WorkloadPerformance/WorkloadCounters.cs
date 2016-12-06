using System.Collections.Generic;

namespace MRMPService.Scheduler.PerformanceCollection
{
    public class WorkloadCounters
    {
        public string workload_id { get; set; }
        public List<PerfCounter> counters { get; set; }
    }
}
