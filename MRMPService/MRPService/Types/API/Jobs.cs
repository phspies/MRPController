using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobListType
    {
        [JsonProperty("jobs")]
        public List<MRPJobType> jobs { get; set; }
    }
    public class MRPJobsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("job")]
        public MRPJobType job { get; set; }
    }
    public class MRPJobType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("target_workload_id")]
        public string target_workload_id { get; set; }
        [JsonProperty("source_workload_id")]
        public string source_workload_id { get; set; }
        [JsonProperty("job_type")]
        public string job_type { get; set; }
        [JsonProperty("jobname")]
        public string jobname { get; set; }
        [JsonProperty("dt_job_id")]
        public Guid? dt_job_id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("servicestack_id")]
        public string servicestack_id { get; set; }
        [JsonProperty("source_workload")]
        public MRPWorkloadType source_workload { get; set; }
        [JsonProperty("target_workload")]
        public MRPWorkloadType target_workload { get; set; }
        [JsonProperty("internal_state")]
        public string internal_state { get; set; }
        [JsonProperty("last_contact")]
        public DateTime? last_contact { get; set; }
        [JsonProperty("can_create_image_recovery")]
        public bool can_create_image_recovery { get; set; }
        [JsonProperty("can_delete")]
        public bool can_delete { get; set; }
        [JsonProperty("can_edit")]
        public bool can_edit { get; set; }
        [JsonProperty("can_failback")]
        public bool can_failback { get; set; }
        [JsonProperty("can_pause")]
        public bool can_pause { get; set; }
        [JsonProperty("can_restore")]
        public bool can_restore { get; set; }
        [JsonProperty("can_reverse")]
        public bool can_reverse { get; set; }
        [JsonProperty("can_start")]
        public bool can_start { get; set; }
        [JsonProperty("can_stop")]
        public bool can_stop { get; set; }
        [JsonProperty("can_undo_failover")]
        public bool can_undo_failover { get; set; }
        [JsonProperty("can_failover")]
        public bool can_failover { get; set; }
        [JsonProperty("jobstats_attributes")]
        public List<MRPJobstatType> jobstats_attributes { get; set; }
        [JsonProperty("jobimages_attributes")]
        public List<MRPJobImageType> jobimages_attributes { get; set; }
    }
    public class MRPJobIDGETType
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
        [JsonProperty("job_id")]
        public string job_id { get; set; }
    }
    public class MRPJobDTIDGETType
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
        [JsonProperty("dt_job_id")]
        public string dt_job_id { get; set; }
    }
}
