using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobstatListType
    {
        [JsonProperty("jobstats")]
        public List<MRPJobstatType> jobstats { get; set; }
    }
    public class MRPJobstatsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("jobstat")]
        public MRPJobstatType jobstat { get; set; }
    }
    public class MRPJobstatType
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


    }
    public class MRPJobstatIDGETType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("worker_hostname")]
        public string worker_hostname { get; set; }
        [JsonProperty("jobstat_id")]
        public string jobstat_id { get; set; }
    }
    public class MRPJobstatDTIDGETType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("worker_hostname")]
        public string worker_hostname { get; set; }
        [JsonProperty("dt_jobstat_id")]
        public string dt_jobstat_id { get; set; }
    }
}
