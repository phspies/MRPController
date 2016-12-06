using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRPDeploymentpolicyType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("policy")]
        public string policy { get; set; }
        [JsonProperty("dt_installpath")]
        public string dt_installpath { get; set; }
        [JsonProperty("dt_windows_temppath")]
        public string dt_windows_temppath { get; set; }
        [JsonProperty("dt_linux_temppath")]
        public string dt_linux_temppath { get; set; }
        [JsonProperty("dt_inifile")]
        public string dt_inifile { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("_default")]
        public bool _default { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("dt_max_memory")]
        public int dt_max_memory { get; set; }
        [JsonProperty("dt_windows_queue_folder")]
        public string dt_windows_queue_folder { get; set; }
        [JsonProperty("dt_linux_queue_folder")]
        public string dt_linux_queue_folder { get; set; }
        [JsonProperty("dt_queue_limit_disk_size")]
        public int dt_queue_limit_disk_size { get; set; }
        [JsonProperty("dt_queue_min_disk_free_size")]
        public int dt_queue_min_disk_free_size { get; set; }
        [JsonProperty("dt_queue_scheme")]
        public string dt_queue_scheme { get; set; }
        [JsonProperty("source_activation_code")]
        public string source_activation_code { get; set; }
        [JsonProperty("target_activation_code")]
        public string target_activation_code { get; set; }
    }
}
