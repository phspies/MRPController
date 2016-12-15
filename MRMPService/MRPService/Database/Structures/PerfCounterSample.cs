using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MRMPService.LocalDatabase
{
    public class PerfCounterSample
    {
        [JsonProperty("Id")]
        [Key]
        public int Id { get; set; }
        [Index]
        [JsonProperty("workload_id")]
        [StringLength(50)]
        [Index]
        public string workload_id { get; set; }
        [JsonProperty("category")]
        [StringLength(255)]
        [Index]
        public string category { get; set; }
        [JsonProperty("counter")]
        [StringLength(255)]
        [Index]
        public string counter { get; set; }
        [JsonProperty("instance")]
        [StringLength(255)]
        [Index]
        public string instance { get; set; }
        [JsonProperty("sample")]
        [StringLength(1024)]
        public String sample { get; set; }
    }
}
