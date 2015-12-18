using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyNetworkFlowsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyNetworkFlowCRUDType networkflow { get; set; }
    }
    public class MoveyNetworkFlowCRUDType
    {
        public string id { get; set; }
        public string source_address { get; set; }
        public string target_address { get; set; }
        public uint source_port { get; set; }
        public uint target_port { get; set; }
        public uint protocol { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime start_timestamp { get; set; }
        public DateTime stop_timestamp { get; set; }
        public uint packets { get; set; }
        public uint kbyte { get; set; }
        
    }

}
