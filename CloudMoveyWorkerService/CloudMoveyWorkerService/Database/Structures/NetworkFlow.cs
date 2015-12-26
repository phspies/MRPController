using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.LocalDatabase
{
    
    public class NetworkFlow
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(50)]
        public string source_address { get; set; }
        [StringLength(50)]
        public string target_address { get; set; }
        [StringLength(50)]
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
