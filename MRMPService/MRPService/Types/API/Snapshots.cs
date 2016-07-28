using DoubleTake.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPSnapshotType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("comment")]
        public string comment { get; set; }
        [JsonProperty("moid")]
        public Guid moid { get; set; }
        [JsonProperty("reason")]
        public SnapshotCreationReason reason { get; set; }
        [JsonProperty("states")]
        public TargetStates states { get; set; }
        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }
    }
}
