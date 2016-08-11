using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPManagementobjectListType
    {
        [JsonProperty("managementobjects")]
        public List<MRPManagementobjectType> managementobjects { get; set; }
    }
    public class MRPManagementobjectsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("managementobject")]
        public MRPManagementobjectType managementobject { get; set; }
    }
    public class MRPManagementobjectType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("target_workload_id")]
        public string target_workload_id { get; set; }
        [JsonProperty("source_workload_id")]
        public string source_workload_id { get; set; }
        [JsonProperty("motype")]
        public string motype { get; set; }
        [JsonProperty("moname")]
        public string moname { get; set; }
        [JsonProperty("moid")]
        public Guid? moid { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("protectiongroup_id")]
        public string protectiongroup_id { get; set; }
        [JsonProperty("source_workload")]
        public MRPWorkloadType source_workload { get; set; }
        [JsonProperty("target_workload")]
        public MRPWorkloadType target_workload { get; set; }
        [JsonProperty("internal_state")]
        public string internal_state { get; set; }
        [JsonProperty("replication_status")]
        public string replication_status { get; set; }
        [JsonProperty("mirror_status")]
        public string mirror_status { get; set; }

        [JsonProperty("last_contact")]
        public DateTime? last_contact { get; set; }
        [JsonProperty("can_create_image_recovery")]
        public bool? can_create_image_recovery { get; set; }
        [JsonProperty("can_delete")]
        public bool? can_delete { get; set; }
        [JsonProperty("can_edit")]
        public bool? can_edit { get; set; }
        [JsonProperty("can_failback")]
        public bool? can_failback { get; set; }
        [JsonProperty("can_pause")]
        public bool? can_pause { get; set; }
        [JsonProperty("can_restore")]
        public bool? can_restore { get; set; }
        [JsonProperty("can_reverse")]
        public bool? can_reverse { get; set; }
        [JsonProperty("can_start")]
        public bool? can_start { get; set; }
        [JsonProperty("can_stop")]
        public bool? can_stop { get; set; }
        [JsonProperty("can_undo_failover")]
        public bool? can_undo_failover { get; set; }
        [JsonProperty("can_failover")]
        public bool? can_failover { get; set; }
        [JsonProperty("managementobjectstats_attributes")]
        public List<MRPManagementobjectStatType> managementobjectstats_attributes { get; set; }
        [JsonProperty("Managementobjectimages_attributes")]
        public List<MRPManagementobjectSnapshotType> managementobjectsnapshot_attributes { get; set; }
    }
    public class MRPManagementobjectIDGETType
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
        [JsonProperty("managementobject_id")]
        public string managementobject_id { get; set; }
    }
    public class MRPManagementobjectDTIDGETType
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
        [JsonProperty("moid")]
        public string moid { get; set; }
    }
}
