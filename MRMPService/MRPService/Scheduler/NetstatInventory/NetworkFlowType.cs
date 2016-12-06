using System;
using System.ComponentModel.DataAnnotations;

namespace MRMPService.Scheduler.NetstatCollection
{

    public class NetworkFlowType
    {
        public string id { get; set; }
        public string source_address { get; set; }
        public string target_address { get; set; }
        public int source_port { get; set; }
        public int target_port { get; set; }
        public int protocol { get; set; }
        public DateTime timestamp { get; set; }
        public string process { get; set; }
        public int pid { get; set; }
    }

}
