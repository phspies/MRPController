using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.API.Types.API
{
    public class MRPNetworkFlowsCRUDType
    {
        public string controller_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPNetworkFlowCRUDType networkflow { get; set; }
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
