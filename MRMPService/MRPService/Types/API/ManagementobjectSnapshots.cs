using DoubleTake.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPManagementobjectSnapshotListType
    {
        [JsonProperty("ManagementobjectSnapshots")]
        public List<MRPManagementobjectSnapshotType> ManagementobjectSnapshots { get; set; }
    }
    public class MRPManagementobjectSnapshotsCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("managementobjectsnapshot")]
        public MRPManagementobjectSnapshotType ManagementobjectSnapshot { get; set; }
    }

    public class MRPManagementobjectSnapshotType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("creation_timestamp")]
        public DateTime? creation_timestamp { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("snapshotmoid")]
        public String snapshotmoid { get; set; }
        [JsonProperty("imagemoid")]
        public String imagemoid { get; set; }
        [JsonProperty("imagename")]
        public string imagename { get; set; }
        [JsonProperty("imagetype")]
        public string imagetype { get; set; }
        [JsonProperty("protection_connection_id")]
        public string protection_connection_id { get; set; }
        [JsonProperty("protection_job_name")]
        public string protection_job_name { get; set; }
        [JsonProperty("source_image_mount_location")]
        public string source_image_mount_location { get; set; }
        [JsonProperty("source_name")]
        public string source_name { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("comment")]
        public string comment { get; set; }
        [JsonProperty("reason")]
        public string reason { get; set; }
        [JsonProperty("timestamp")]
        public DateTime? timestamp { get; set; }
        [JsonProperty("connection_id")]
        public Guid? connection_id{ get; set; }
        [JsonProperty("_destroy")]
        public bool? _destroy { get; set; }
    }
    public class MRPManagementobjectSnapshotIDGETType
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
        [JsonProperty("image_id")]
        public string image_id { get; set; }
    }
    public class MRPManagementobjectSnapshotMOIDGETType
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
        [JsonProperty("moid_id")]
        public string moid_id { get; set; }
    }
}
