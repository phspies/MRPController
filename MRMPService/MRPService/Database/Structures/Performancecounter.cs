using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRMPService.LocalDatabase
{
    public class Performancecounter
    {
        [MaxLength(45)]
        public string id { get; set; }
        [MaxLength(45)]
        public string workload_id { get; set; }
        [MaxLength(128)]
        public string category { get; set; }
        [MaxLength(128)]
        public string counter { get; set; }
        [MaxLength(256)]
        public string instance { get; set; }
        public DateTime timestamp { get; set; }
        public double value { get; set; }
    }
}
