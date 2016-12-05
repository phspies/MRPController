using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPService.Types.API
{

    public class MRPTaskListType
    {
        [JsonProperty("tasks")]
        public List<MRPTaskType> tasks { get; set; }
    }
    public class MRPTaskGetType
    {
        [JsonProperty("task_id")]
        public string task_id { get; set; }
    }
    public class MRPTaskType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("target_id")]
        public string target_id { get; set; }
        [JsonProperty("taskdetail")]
        public MRPTaskDetailType taskdetail { get; set; }
        [JsonProperty("target_type")]
        public string target_type { get; set; }
        [JsonProperty("task_type")]
        public string task_type { get; set; }
    }

    public class MRPTaskDetailType
    {
        [JsonProperty("managementobjects")]
        public List<MRPManagementobjectOrderType> managementobjects { get; set; }
        [JsonProperty("original")]
        public MRPWorkloadType original { get; set; }
        [JsonProperty("source_workload")]
        public MRPWorkloadType source_workload { get; set; }
        [JsonProperty("target_workload")]
        public MRPWorkloadType target_workload { get; set; }
        [JsonProperty("workloadpairs")]
        public List<MRPWorkloadPairType> workloadpairs { get; set; }
        [JsonProperty("repository")]
        public MRPWorkloadType repository { get; set; }
        [JsonProperty("protectiongroup")]
        public MRPProtectiongroupType protectiongroup { get; set; }
        [JsonProperty("source_platform")]
        public MRPPlatformType source_platform { get; set; }
        [JsonProperty("target_platform")]
        public MRPPlatformType target_platform { get; set; }
        [JsonProperty("managementobject")]
        public MRPManagementobjectType managementobject { get; set; }
        [JsonProperty("managementobjectsnapshot")]
        public MRPManagementobjectSnapshotType managementobjectsnapshot { get; set; }
        [JsonProperty("protectiongrouptree")]
        public MRPProtectiongrouptreeType protectiongrouptree { get; set; }
    }
    public class MRPManagementobjectOrderType
    {
        [JsonProperty("firedrill")]
        public bool? firedrill { get; set; }
        [JsonProperty("original")]
        public MRPWorkloadType original { get; set; }
        [JsonProperty("position")]
        public int position { get; set;}
        [JsonProperty("managementobject")]
        public MRPManagementobjectType managementobject { get; set; }
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
