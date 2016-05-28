using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{

    public class MRPJobListType
    {
        public List<MRPJobType> jobs { get; set; }
    }
    public class MRPJobsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPJobType job { get; set; }
    }
    public class MRPJobType
    {
        public string id { get; set; }
        public string target_workload_id { get; set; }
        public string source_workload_id { get; set; }
        public string job_type { get; set; }
        public string jobname { get; set; }
        public Guid? dt_job_id { get; set; }
        public string organization_id { get; set; }
        public string state { get; set; }
        public string servicestack_id { get; set; }
        public MRPWorkloadType source_workload { get; set; }
        public MRPWorkloadType target_workload { get; set; }
        public string internal_state { get; set; }
        public DateTime? last_contact { get; set; }
        public bool can_create_image_recovery { get; set; }
        public bool can_delete { get; set; }
        public bool can_edit { get; set; }
        public bool can_failback { get; set; }
        public bool can_pause { get; set; }
        public bool can_restore { get; set; }
        public bool can_reverse { get; set; }
        public bool can_start { get; set; }
        public bool can_stop { get; set; }
        public bool can_undo_failover { get; set; }
        public bool can_failover { get; set; }
        public List<MRPJobstatType> jobstats_attributes { get; set; }
        public List<MRPJobImageType> jobimages_attributes { get; set; }
    }
    public class MRPJobIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string job_id { get; set; }
    }
    public class MRPJobDTIDGETType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string worker_hostname { get; set; }
        public string dt_job_id { get; set; }
    }
}
