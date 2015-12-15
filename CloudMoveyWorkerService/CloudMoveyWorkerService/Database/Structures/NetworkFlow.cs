using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    public class NetworkFlow
    {
        public string id { get; set; }
        public string source_address { get; set; }
        public string target_address { get; set; }
        public uint source_port { get; set; }
        public uint target_port { get; set; }
        public uint protocol { get; set; }
        public DateTime timestamp { get; set; }
        public uint start_timestamp { get; set; }
        public uint stop_timestamp { get; set; }
        public uint packets { get; set; }
        public uint kbyte { get; set; }
    }

}
