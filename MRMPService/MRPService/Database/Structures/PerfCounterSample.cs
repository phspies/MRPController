using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MRMPService.LocalDatabase
{
    public class PerfCounterSample
    {
        [JsonProperty("Id")]
        [Key]
        public int Id { get; set; }
        [JsonProperty("workload_id")]
        [StringLength(50)]
        public string workload_id { get; set; }
        [JsonProperty("category")]
        [StringLength(255)]
        public string category { get; set; }
        [JsonProperty("counter")]
        [StringLength(255)]
        public string counter { get; set; }
        [JsonProperty("instance")]
        [StringLength(255)]
        public string instance { get; set; }
        [JsonProperty("s0")]
        [StringLength(1024)]
        public String sample { get; set; }
    }
}
