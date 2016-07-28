using DoubleTake.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobImageListType
    {
        [JsonProperty("jobimages")]
        public List<MRPJobImageType> jobimages { get; set; }
    }
    public class MRPJobImagesCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("jobimage")]
        public MRPJobImageType jobimage { get; set; }
    }
    public class MRPJobImageType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("creation_timestamp")]
        public DateTime creation_timestamp { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("moid")]
        public Guid moid { get; set; }
        [JsonProperty("image_name")]
        public string image_name { get; set; }
        [JsonProperty("image_type")]
        public ImageType image_type { get; set; }
        [JsonProperty("protection_connection_id")]
        public string protection_connection_id { get; set; }
        [JsonProperty("protection_job_name")]
        public string protection_job_name { get; set; }
        [JsonProperty("source_image_mount_location")]
        public string source_image_mount_location { get; set; }
        [JsonProperty("source_name")]
        public string source_name { get; set; }
        [JsonProperty("state")]
        public TargetStates state { get; set; }
        [JsonProperty("jobimagesnapshots_attributes")]
        public List<MRPSnapshotType> jobimagesnapshots_attributes { get; set; }

    }
    public class MRPJobImageIDGETType
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
    public class MRPJobImageMOIDGETType
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
