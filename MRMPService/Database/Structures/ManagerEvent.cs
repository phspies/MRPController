using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRMPService.LocalDatabase
{
    public class ManagerEvent
    {
        [JsonProperty("Id")]
        [Key]
        public int Id { get; set; }
        [JsonProperty("message")]
        [Column(TypeName = "ntext")]
        public string message { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
    }
}
