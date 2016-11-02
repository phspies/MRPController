using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPManagementobjectStatListType
    {
        [JsonProperty("ManagementobjectStats")]
        public List<MRPManagementobjectStatType> ManagementobjectStats { get; set; }
    }
    public class MRPManagementobjectStatsCRUDType
    {
        [JsonProperty("ManagementobjectStat")]
        public MRPManagementobjectStatType ManagementobjectStat { get; set; }
    }
    public class MRPManagementobjectStatType
    {
        [JsonProperty("job_id")]
        public string job_id { get; set; }
        [JsonProperty("replication_status")]
        public string replication_status { get; set; }
        [JsonProperty("replication_queue")]
        public Int64 replication_queue { get; set; }
        [JsonProperty("replication_disk_queue")]
        public Int64 replication_disk_queue { get; set; }
        [JsonProperty("replication_bytes_sent")]
        public Int64 replication_bytes_sent { get; set; }
        [JsonProperty("replication_bytes_sent_compressed")]
        public Int64 replication_bytes_sent_compressed { get; set; }
        [JsonProperty("mirror_status")]
        public string mirror_status { get; set; }
        [JsonProperty("mirror_skipped")]
        public Int64 mirror_skipped { get; set; }
        [JsonProperty("mirror_remaining")]
        public Int64 mirror_remaining { get; set; }
        [JsonProperty("mirror_percent_complete")]
        public int mirror_percent_complete { get; set; }
        [JsonProperty("connected_since")]
        public DateTime connected_since { get; set; }
        [JsonProperty("stransmit_mode")]
        public string stransmit_mode { get; set; }
        [JsonProperty("recovery_point_objective")]
        public DateTime recovery_point_objective { get; set; }
        [JsonProperty("recovery_point_latency")]
        public long recovery_point_latency { get; set; }
    }
    public class MRPManagementobjectStatIDGETType
    {
        [JsonProperty("worker_hostname")]
        public string worker_hostname { get; set; }
        [JsonProperty("ManagementobjectStat_id")]
        public string managementobjectstat_id { get; set; }
    }
    public class MRPManagementobjectStatDTIDGETType
    {
        [JsonProperty("worker_hostname")]
        public string worker_hostname { get; set; }
        [JsonProperty("dt_managementobjectstat_id")]
        public string dt_managementobjectstat_id { get; set; }
    }
}
