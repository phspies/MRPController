using Newtonsoft.Json;
using System;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPManagerEventType
    {
        [JsonProperty("manager_id")]
        public string manager_id { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
    }
}
