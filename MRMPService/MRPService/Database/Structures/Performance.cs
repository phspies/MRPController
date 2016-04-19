using System;
using System.ComponentModel.DataAnnotations;

namespace MRMPService.LocalDatabase
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
