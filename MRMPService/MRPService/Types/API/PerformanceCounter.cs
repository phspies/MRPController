using System;

namespace MRMPService.API.Types.API
{
    public class MRPPerformanceCountersCRUDType
    {
        public string controller_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPPerformanceCounterCRUDType performancecounter { get; set; }
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
