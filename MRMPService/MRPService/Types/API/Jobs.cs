using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
{

    public class MRPJobListType
    {
        public List<MRPJobType> jobs { get; set; }
    }
    public class MRPJobsCRUDType
    {
        public string controller_id
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
        public string workload_id { get; set; }
        public string source_workload_id { get; set; }
        public string job_type { get; set; }
        public string dt_job_id { get; set; }
        public Int64 dt_memory_replication_queue { get; set; }
        public Int64 dt_disk_replication_queue { get; set; }
        public Int64 dt_disk_queue_size { get; set; }
        public Int64 dt_memory_queue_size { get; set; }
        public DateTime dt_queue_datetime_activation { get; set; }
        public string organization_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Int64 dt_total_size { get; set; }
        public Int64 dt_remaining_size { get; set; }
        public string state { get; set; }
        public string servicestack_id { get; set; }
    }
    public class MRPJobIDGETType
    {
        public string controller_id
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
        public string controller_id
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
