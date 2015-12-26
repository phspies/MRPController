using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.LocalDatabase
{
    
    public class Performance
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(50)]
        public string workload_id { get; set; }
        public DateTime timestamp { get; set; }
        [StringLength(100)]
        public string category_name { get; set; }
        [StringLength(100)]
        public string counter_name { get; set; }
        [StringLength(100)]
        public string instance { get; set; }
        public double value { get; set; }       
    }
}
