using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MRMPService.LocalDatabase
{
    public class PerfCounterSample
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string workload_id { get; set; }
        [MaxLength(255)]
        public string category { get; set; }
        [MaxLength(255)]
        public string counter { get; set; }
        [MaxLength(255)]
        public string instance { get; set; }
        [MaxLength(1024)]
        public String sample { get; set; }
    }
}
