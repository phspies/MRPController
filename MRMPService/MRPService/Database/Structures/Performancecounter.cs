using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRMPService.LocalDatabase
{
    public class Performancecounter
    {
        [Key, StringLength(45)]
        public string id { get; set; }
        [StringLength(45)]
        [Index]
        public string workload_id { get; set; }
        [StringLength(128)]
        [Index]
        public string category { get; set; }
        [StringLength(128)]
        [Index]
        public string counter { get; set; }
        [StringLength(256)]
        [Index]
        public string instance { get; set; }
        [Index]
        public DateTime timestamp { get; set; }
        public double value { get; set; }
    }
}
