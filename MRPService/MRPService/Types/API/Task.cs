using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.MRPService.Types.API
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
        public string worker_id { get; set; }
        public string task_type { get; set; }
        public bool hidden { get; set; }
        public bool locked { get; set; }
        public object locked_worker { get; set; }
        public bool internal_complete { get; set; }
        public bool blocking { get; set; }
    }

    public class MRPTaskSubmitpayloadType
    {
        public MRPTaskWorkloadType original { get; set; }
        public MRPTaskWorkloadType source { get; set; }
        public MRPTaskWorkloadType target { get; set; }
        public MRPTaskServicestackType servicestack { get; set; }
        public MRPTaskPlatformType platform { get; set; }
    }

    public class MRPTaskRecoverypolicyType
    {
        public string id { get; set; }
        public string policy { get; set; }
        public string policytype { get; set; }
        public string repositorypath { get; set; }
        public object mirrortype { get; set; }
        public object calculatesize { get; set; }
        public object deleteorphanedfiles { get; set; }
        public string networkroute { get; set; }
        public bool enablesnapshots { get; set; }
        public int snapshotincrement { get; set; }
        public string snapshotinterval { get; set; }
        public bool enablecompression { get; set; }
        public int compressionlevel { get; set; }
        public bool enablebandwidthlimit { get; set; }
        public int bandwidthlimit { get; set; }
        public string organization_id { get; set; }
        public bool delete_current_jobs { get; set; }
        public int snapshotcount { get; set; }
    }
    public class MRPTaskWorkloadType
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string iplist { get; set; }
        public string credential_id { get; set; }
        public int vcpu { get; set; }
        public int vcore { get; set; }
        public int vmemory { get; set; }
        public MRPTaskDeploymentpolicyType deploymentpolicy { get; set; }
        public List<MRPTaskDiskType> disks { get; set; }
        public List<MRPTaskVolumeType> volumes { get; set; }
        public List<MRPTaskInterfaceType> interfaces { get; set; }
        public MRPTaskTemplateType platform_template { get; set; }
    }
    public class MRPTaskDeploymentpolicyType
    {
        public string id { get; set; }
        public string policy { get; set; }
        public string dt_installpath { get; set; }
        public string dt_temppath { get; set; }
        public string dt_inifile { get; set; }
        public string organization_id { get; set; }
        public bool _default { get; set; }
        public bool enabled { get; set; }
        public int dt_max_memory { get; set; }
        public string dt_queue_folder { get; set; }
        public int dt_queue_limit_disk_size { get; set; }
        public int dt_queue_min_disk_free_size { get; set; }
        public string dt_queue_scheme { get; set; }
        public string activation_code { get; set; }
    }

    public class MRPTaskTemplateType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public object systemtemplate_id { get; set; }
        public string platform_moid { get; set; }
        public string organization_id { get; set; }
        public string image_moid { get; set; }
        public string image_name { get; set; }
        public string image_description { get; set; }
        public object image_type { get; set; }
        public string os_id { get; set; }
        public string os_type { get; set; }
        public string os_displayname { get; set; }
        public DateTime created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class MRPTaskDiskType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public int disksize { get; set; }
        public MRPTaskPlatformstoragetierType platformstoragetier { get; set; }
        public string moid { get; set; }
        public int position { get; set; }
        public bool provisioned { get; set; }
        public string deviceid { get; set; }
    }

    public class MRPTaskPlatformstoragetierType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string storagetier { get; set; }
        public string shortname { get; set; }
    }

    public class MRPTaskVolumeType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public string driveletter { get; set; }
        public string serialnumber { get; set; }
        public int blocksize { get; set; }
        public string deviceid { get; set; }
        public Int64 volumesize { get; set; }
        public Int64 volumefreespace { get; set; }
        public bool provisioned { get; set; }
        public string volumename { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class MRPTaskInterfaceType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int vnic { get; set; }
        public string ipaddress { get; set; }
        public string ipv6address { get; set; }
        public string netmask { get; set; }
        public string ipv6netmask { get; set; }
        public string platformnetwork_id { get; set; }
        public string ipassignment { get; set; }
        public string moid { get; set; }
        public int connection_index { get; set; }
        public string connection_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string macaddress { get; set; }
        public MRPTaskPlatformnetworkType platformnetwork { get; set; }
    }

    public class MRPTaskPlatformnetworkType
    {
        public string id { get; set; }
        public string platformdomain_id { get; set; }
        public string network { get; set; }
        public string description { get; set; }
        public string ipv4subnet { get; set; }
        public int ipv4netmask { get; set; }
        public object networktype { get; set; }
        public string moid { get; set; }
        public bool provisioned { get; set; }
        public string ipv6subnet { get; set; }
        public int ipv6netmask { get; set; }
        public string networkdomain_moid { get; set; }
        public string networkdomain_name { get; set; }
    }

    public class MRPTaskServicestackType
    {
        public string id { get; set; }
        public string service { get; set; }
        public string supportservice_id { get; set; }
        public int position { get; set; }
        public string organization_id { get; set; }
        public string stacktype { get; set; }
        public int currentstep { get; set; }
        public string recoverypolicy_id { get; set; }
        public string repository_workload_id { get; set; }
        public MRPTaskRecoverypolicyType recoverypolicy { get; set; }
    }

    public class MRPTaskPlatformType
    {
        public string id { get; set; }
        public string organization_id { get; set; }
        public string credential_id { get; set; }
        public string platform { get; set; }
        public string mapping { get; set; }
        public bool enabled { get; set; }
        public string platformtype { get; set; }
        public int maxcpu { get; set; }
        public int maxmemory { get; set; }
        public int maxdiskcount { get; set; }
        public int maxdisksize { get; set; }
        public int maxnetwork { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string moid { get; set; }
        public object mcpendpoint_id { get; set; }
        public string worker_id { get; set; }
        public string platform_version { get; set; }
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
        public string controller_id
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
        public string controller_id
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
