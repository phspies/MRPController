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
        public int Id { get; set; }
        [JsonProperty("workload_id")]
        [MaxLength(50)]
        public string workload_id { get; set; }
        [JsonProperty("category")]
        [MaxLength(255)]
        public string category { get; set; }
        [JsonProperty("counter")]
        [MaxLength(255)]
        public string counter { get; set; }
        [JsonProperty("instance")]
        [MaxLength(255)]
        public string instance { get; set; }
        [JsonProperty("sample")]
        [MaxLength(1024)]
        public String sample { get; set; }
    }
}
