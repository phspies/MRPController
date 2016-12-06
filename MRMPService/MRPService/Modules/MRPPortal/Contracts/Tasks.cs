using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MRPService.Portal.Types.API;

namespace MRPService.Portal.Types.API
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
        public string endtimestamp { get; set; }
        public MRPTaskPayloadType submitpayload { get; set; }
        public string returnpayload { get; set; }
        public string target_type { get; set; }
        public string percentage { get; set; }
        public string step { get; set; }
        public object source { get; set; }
        public object name { get; set; }
        public string worker_id { get; set; }
        public string task_type { get; set; }
        public bool hidden { get; set; }
        public bool locked { get; set; }
        public object locked_worker { get; set; }
        public bool internal_complete { get; set; }
        public bool blocking { get; set; }
    }

    public class MRPTaskPayloadType
    {
        public MRPTaskDTType dt { get; set; }
        public string task_action { get; set; }
        public MRPTaskWindowsType windows { get; set; }
        public MRPTaskMcpType mcp { get; set; }
        public MRPPlatformType platform { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public object locationId { get; set; }
    }

    public class MRPTaskDTType
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public string domain { get; set; }
        public List<MRPWorkloadDiskType> volumes { get; set; }
        public MRPTaskFailovergroupType failovergroup { get; set; }
        public MRPTaskOriginalType original { get; set; }
        public MRPTaskSourceType source { get; set; }
        public MRPTaskTargetType target { get; set; }
        public MRPRecoverypolicyType recoverypolicy { get; set; }
        public bool reuse_dt_images { get; set; }
        public bool delete_current_dt_job { get; set; }
    }

    public class MRPTaskFailovergroupType
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public int position { get; set; }
        public string group { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string organization_id { get; set; }
        public int phase { get; set; }
        public string grouptype { get; set; }
        public bool enabled { get; set; }
        public string recoverypolicy_id { get; set; }
        public object failoversnapshot_id { get; set; }
        public string source_repository_id { get; set; }
        public string target_platform_id { get; set; }
        public string target_repository_id { get; set; }
        public string target_workload_suffix { get; set; }
        public bool dt_delete_current_job { get; set; }
    }

    public class MRPTaskOriginalType
    {
        public string id { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public string domain { get; set; }
        public List<MRPWorkloadDiskType> volumes { get; set; }
    }

    public class MRPTaskSourceType
    {
        public string username { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public string domain { get; set; }
        public List<MRPWorkloadDiskType> volumes { get; set; }
    }
    public class MRPTaskTargetType
    {
        public string username { get; set; }
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public string domain { get; set; }
    }



    public class MRPTaskMcpType
    {
        public MRPTaskOriginalType original { get; set; }
        public MRPWorkloadType target { get; set; }
        public MRPRecoverypolicyType recoverypolicy { get; set; }
        public string id { get; set; }
        public string endpoint { get; set; }
        public string url { get; set; }
    }
    public class MRPTaskMcpendpointType
    {
        public string id { get; set; }
        public string endpoint { get; set; }
        public string url { get; set; }
    }

    //Only used for talk updates
    public class TaskUpdateType
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string task_id { get; set; }

        public TaskUpdateAttributesType attributes { get; set; }
    }
    public class TaskUpdateAttributesType
    {
        public string returnpayload { set; get; }
        public int status { set; get; }
    }

    public class ProgressTaskUpdateType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public string task_id { get; set; }
        public ProgressTaskUpdateAttributesType attributes { get; set; }
    }
    public class ProgressTaskUpdateAttributesType
    {
        public string step { set; get; }
        public double percentage { set; get; }
    }
    public class CompleteTaskUpdateType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public string task_id { get; set; }
        public CompleteTaskUpdateAttributesType attributes { get; set; }
    }
    public class CompleteTaskUpdateAttributesType
    {
        public string step { set; get; }
        public decimal percentage { set; get; }
        public string returnpayload { set; get; }
        public int status { set; get; }
    }
}

