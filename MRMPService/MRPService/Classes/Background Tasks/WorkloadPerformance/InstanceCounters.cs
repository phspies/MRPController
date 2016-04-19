using System;
using System.Diagnostics;

namespace MRMPService.PerformanceCollection
{


    public class InstanceCounters
    {
        public string instance { get; set; }
        public double value { get; set; }
        public CounterSample s0 { get; set; }
        public CounterSample s1 { get; set; }
        public DateTime timestamp { get; set; }
    }
}
