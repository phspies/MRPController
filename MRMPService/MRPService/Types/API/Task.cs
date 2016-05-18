using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPService.Types.API
{

    public class MRPTaskListType
    {
        public List<MRPTaskType> tasks { get; set; }
    }

    public class MRPTaskType
    {
        public string id { get; set; }
        public string target { get; set; }
        public string target_id { get; set; }
        public object source_id { get; set; }
        public string user_id { get; set; }
        public string organization_id { get; set; }
        public string task { get; set; }
        public string status { get; set; }
        public string starttimestamp { get; set; }
        public object endtimestamp { get; set; }
        public MRPTaskSubmitpayloadType submitpayload { get; set; }
        public object returnpayload { get; set; }
        public string target_type { get; set; }
        public string percentage { get; set; }
        public string step { get; set; }
        public object source { get; set; }
        public object name { get; set; }
        public string manager_id { get; set; }
        public string task_type { get; set; }
        public bool hidden { get; set; }
        public bool locked { get; set; }
        public object locked_worker { get; set; }
        public bool internal_complete { get; set; }
        public bool blocking { get; set; }
    }

    public class MRPTaskSubmitpayloadType
    {
        public MRPWorkloadType original { get; set; }
        public MRPWorkloadType source { get; set; }
        public MRPWorkloadType target { get; set; }
        public MRPServicestackType servicestack { get; set; }
        public MRPPlatformType platform { get; set; }
        public MRPTaskJobType job { get; set; }
        public MRPStacktreeType stacktree { get; set; }
    }
    public class MRPTaskJobType
    {
        public string id { get; set; }
        public string target_workload_id { get; set; }
        public string source_workload_id { get; set; }
        public string job_type { get; set; }
        public string dt_job_id { get; set; }
        public Int64 dt_memory_replication_queue { get; set; }
        public Int64 dt_disk_replication_queue { get; set; }
        public Int64 dt_disk_queue_size { get; set; }
        public Int64 dt_memory_queue_size { get; set; }
        public DateTime? dt_queue_datetime_activation { get; set; }
        public string organization_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public Int64 dt_total_size { get; set; }
        public Int64 dt_remaining_size { get; set; }
        public string state { get; set; }
        public string servicestack_id { get; set; }
    }

    public class MRPTaskPlatformstoragetierType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string storagetier { get; set; }
        public string shortname { get; set; }
    }

   
    public class MRPTaskUpdateType
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string task_id { get; set; }

        public MRPTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPTaskUpdateAttributesType
    {
        public string returnpayload { set; get; }
        public int status { set; get; }
    }

    public class MRPProgressTaskUpdateType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string task_id { get; set; }
        public MRPProgressTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPProgressTaskUpdateAttributesType
    {
        public string step { set; get; }
        public double percentage { set; get; }
    }
    public class MRPCompleteTaskUpdateType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public string task_id { get; set; }
        public MRPCompleteTaskUpdateAttributesType attributes { get; set; }
    }
    public class MRPCompleteTaskUpdateAttributesType
    {
        public string step { set; get; }
        public decimal percentage { set; get; }
        public string returnpayload { set; get; }
        public int status { set; get; }
    }
}
