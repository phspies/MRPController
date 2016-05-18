using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPNetworkFlowsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public List<MRPNetworkFlowCRUDType> networkflows { get; set; }
    }
    public class MRPNetworkFlowCRUDType
    {
        public string source_address { get; set; }
        public string target_address { get; set; }
        public int source_port { get; set; }
        public int target_port { get; set; }
        public int protocol { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime start_timestamp { get; set; }
        public DateTime stop_timestamp { get; set; }
        public int packets { get; set; }
        public int kbyte { get; set; }
        public string source_workload_id { get; set; }
        public string target_workload_id { get; set; }
    }

}
