using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPWorkloadProcessType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("caption")]
        public string caption { get; set; }
        [JsonProperty("commandline")]
        public string commandline { get; set; }
        [JsonProperty("processid")]
        public int processid { get; set; }
        [JsonProperty("virtualsize")]
        public Int64 virtualsize { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("threadcount")]
        public int threadcount { get; set; }
        [JsonProperty("writeoperationcount")]
        public Int64 writeoperationcount { get; set; }
        [JsonProperty("writetransfercount")]
        public Int64 writetransfercount { get; set; }
        [JsonProperty("readoperationcount")]
        public Int64 readoperationcount { get; set; }
        [JsonProperty("readtransfercount")]
        public Int64 readtransfercount { get; set; }
        [JsonProperty("_destroy")]
        public bool? _destroy { get; set; }
    }
}
