using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPService.Types.API
{

    public class MRPTaskListType
    {
        [JsonProperty("tasks")]
        public List<MRPTaskType> tasks { get; set; }
    }

    public class MRPTaskType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("target")]
        public string target { get; set; }
        [JsonProperty("target_id")]
        public string target_id { get; set; }
        [JsonProperty("source_id")]
        public object source_id { get; set; }
        [JsonProperty("user_id")]
        public string user_id { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("task")]
        public string task { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("starttimestamp")]
        public string starttimestamp { get; set; }
        [JsonProperty("endtimestamp")]
        public object endtimestamp { get; set; }
        [JsonProperty("submitpayload")]
        public MRPTaskSubmitpayloadType submitpayload { get; set; }
        [JsonProperty("returnpayload")]
        public object returnpayload { get; set; }
        [JsonProperty("target_type")]
        public string target_type { get; set; }
        [JsonProperty("percentage")]
        public string percentage { get; set; }
        [JsonProperty("step")]
        public string step { get; set; }
        [JsonProperty("source")]
        public object source { get; set; }
        [JsonProperty("name")]
        public object name { get; set; }
        [JsonProperty("manager_id")]
        public string manager_id { get; set; }
        [JsonProperty("task_type")]
        public string task_type { get; set; }
        [JsonProperty("hidden")]
        public bool hidden { get; set; }
        [JsonProperty("locked")]
        public bool locked { get; set; }
        [JsonProperty("locked_worker")]
        public object locked_worker { get; set; }
        [JsonProperty("internal_complete")]
        public bool internal_complete { get; set; }
        [JsonProperty("blocking")]
        public bool blocking { get; set; }
    }

    public class MRPTaskSubmitpayloadType
    {
        [JsonProperty("original")]
        public MRPWorkloadType original { get; set; }
        [JsonProperty("source")]
        public MRPWorkloadType source { get; set; }
        [JsonProperty("target")]
        public MRPWorkloadType target { get; set; }
        [JsonProperty("workload_pairs")]
        public MRPWorkloadPairType workload_pairs { get; set; }
        [JsonProperty("repository")]
        public MRPWorkloadType repository { get; set; }
        [JsonProperty("protectiongroup")]
        public MRPProtectiongroupType protectiongroup { get; set; }
        [JsonProperty("platform")]
        public MRPPlatformType platform { get; set; }
        [JsonProperty("managementobject")]
        public MRPManagementobjectType managementobject { get; set; }
        [JsonProperty("protectiongrouptree")]
        public MRPProtectiongrouptreeType protectiongrouptree { get; set; }
    }
    public class MRPTaskJobType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("target_workload_id")]
        public string target_workload_id { get; set; }
        [JsonProperty("source_workload_id")]
        public string source_workload_id { get; set; }
        [JsonProperty("job_type")]
        public string job_type { get; set; }
        [JsonProperty("dt_job_id")]
        public string dt_job_id { get; set; }
        [JsonProperty("dt_memory_replication_queue")]
        public Int64 dt_memory_replication_queue { get; set; }
        [JsonProperty("dt_disk_replication_queue")]
        public Int64 dt_disk_replication_queue { get; set; }
        [JsonProperty("dt_disk_queue_size")]
        public Int64 dt_disk_queue_size { get; set; }
        [JsonProperty("dt_memory_queue_size")]
        public Int64 dt_memory_queue_size { get; set; }
        [JsonProperty("dt_queue_datetime_activation")]
        public DateTime? dt_queue_datetime_activation { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("created_at")]
        public DateTime? created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? updated_at { get; set; }
        [JsonProperty("dt_total_size")]
        public Int64 dt_total_size { get; set; }
        [JsonProperty("dt_remaining_size")]
        public Int64 dt_remaining_size { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("servicestack_id")]
        public string servicestack_id { get; set; }
    }

    public class MRPTaskPlatformstoragetierType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("storagetier")]
        public string storagetier { get; set; }
        [JsonProperty("shortname")]
        public string shortname { get; set; }
    }
    public class MRPTaskUpdateType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("task_id")]
        public string task_id { get; set; }
        [JsonProperty("attributes")]
        public MRPTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPTaskUpdateAttributesType
    {
        [JsonProperty("returnpayload")]
        public string returnpayload { set; get; }
        [JsonProperty("status")]
        public int status { set; get; }
    }

    public class MRPProgressTaskUpdateType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("task_id")]
        public string task_id { get; set; }
        [JsonProperty("attributes")]
        public MRPProgressTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPProgressTaskUpdateAttributesType
    {
        [JsonProperty("step")]
        public string step { set; get; }
        [JsonProperty("percentage")]
        public double percentage { set; get; }
    }
    public class MRPCompleteTaskUpdateType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("task_id")]
        public string task_id { get; set; }
        [JsonProperty("attributes")]
        public MRPCompleteTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPCompleteTaskUpdateAttributesType
    {
        [JsonProperty("step")]
        public string step { set; get; }
        [JsonProperty("percentage")]
        public decimal percentage { set; get; }
        [JsonProperty("returnpayload")]
        public string returnpayload { set; get; }
        [JsonProperty("status")]
        public int status { set; get; }
    }
}
