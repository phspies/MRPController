using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPerformanceCountersCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public List<MRPPerformanceCounterCRUDType> performancecounters { get; set; }
    }
    public class MRPPerformanceCounterCRUDType
    {
        public string workload_id { get; set; }
        public DateTime timestamp { get; set; }
        public string performancecategory_id { get; set; }
        public string instance { get; set; }
        public double value { get; set; }       
    }
}
