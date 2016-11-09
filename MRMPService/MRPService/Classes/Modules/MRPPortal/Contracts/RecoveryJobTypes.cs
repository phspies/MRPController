using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class RecoveryJobListObject
    {
        [JsonProperty("recoveryjobs")]
        public List<RecoveryJobObject> recoveryjobs { get; set; }
    }

    public class RecoveryJobObject
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public object name { get; set; }
        [JsonProperty("jobtype")]
        public string jobtype { get; set; }
        [JsonProperty("source_workload_id")]
        public string source_workload_id { get; set; }
        [JsonProperty("target_workload_id")]
        public string target_workload_id { get; set; }
        [JsonProperty("target_platform_id")]
        public string target_platform_id { get; set; }
        [JsonProperty("target_platformnetwork_id")]
        public object target_platformnetwork_id { get; set; }
        [JsonProperty("repository_id")]
        public string repository_id { get; set; }
        [JsonProperty("activity")]
        public object activity { get; set; }
        [JsonProperty("mirrorstatus")]
        public object mirrorstatus { get; set; }
        [JsonProperty("replicationstatus")]
        public object replicationstatus { get; set; }
        [JsonProperty("transmitmode")]
        public object transmitmode { get; set; }
        [JsonProperty("recoverypolicy_id")]
        public object recoverypolicy_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("source_moid")]
        public string source_moid { get; set; }
        [JsonProperty("target_moid")]
        public string target_moid { get; set; }
    }
}
