using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MRMPService.LocalDatabase
{
    public class ManagerEvent
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
    }
}
