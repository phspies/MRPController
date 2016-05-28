using DoubleTake.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobImageListType
    {
        public List<MRPJobImageType> jobimages { get; set; }
    }
    public class MRPJobImagesCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPJobImageType jobimage { get; set; }
    }
    public class MRPJobImageType
    {
        public string id { get; set; }
        public DateTime creation_timestamp { get; set; }
        public string description { get; set; }
        public Guid moid { get; set; }
        public string image_name { get; set; }
        public ImageType image_type { get; set; }
        public string protection_connection_id { get; set; }
        public string protection_job_name { get; set; }
        public string source_image_mount_location { get; set; }
        public string source_name { get; set; }
        public TargetStates state { get; set; }
        public List<MRPSnapshotType> jobimagesnapshots_attributes { get; set; }

    }
    public class MRPJobImageIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string image_id { get; set; }
    }
    public class MRPJobImageMOIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string moid_id { get; set; }
    }
}
