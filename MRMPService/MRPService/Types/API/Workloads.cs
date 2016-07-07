using MRMPService.MRMPService.Types.API;
using MRMPService.MRPService.Types.API;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPWorkloadsCRUDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPWorkloadType workload { get; set; }
    }
    public class MRPWorkloadGetIDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public String workload_id { get; set; }
    }
    public class MRPWorkloadGetMOIDType
    {
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public String moid_id { get; set; }
    }

    public class MRPWorkloadListType
    {
        public List<MRPWorkloadType> workloads { get; set; }
    }
    public class MRPWorkloadType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string hostname { get; set; }
        public string iplist { get; set; }
        public int? vcpu { get; set; }
        public int? vcpu_speed { get; set; }
        public int? vmemory { get; set; }
        public int? vcore { get; set; }
        public string ostype { get; set; }
        public string platform_ostype { get; set; }
        public string osedition { get; set; }
        public bool? failedover { get; set; }
        public bool? enabled { get; set; }
        public bool? active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string workloadtype { get; set; }
        public string configuration { get; set; }
        public bool? provisioned { get; set; }
        public bool? dt_installed { get; set; }
        public string dt_version { get; set; }
        public string deploymentpolicy_id { get; set; }
        public string ipaddress { get; set; }
        public string hash_value { get; set; }
        public bool? perf_collection_enabled { get; set; }
        public bool? os_collection_enabled { get; set; }
        public bool? dt_collection_enabled { get; set; }
        public bool? netstat_collection_enabled { get; set; }
        public bool? perf_collection_status { get; set; }
        public bool? netstat_collection_status { get; set; }
        public bool? os_collection_status { get; set; }
        public bool? dt_collection_status { get; set; }
        public string perf_collection_message { get; set; }
        public string netstat_collection_message { get; set; }
        public string os_collection_message { get; set; }
        public string dt_collection_message { get; set; }
        public DateTime? netstat_last_contact { get; set; }
        public DateTime? os_last_contact { get; set; }
        public DateTime? perf_last_contact { get; set; }
        public DateTime? dt_last_contact { get; set; }
        public int? os_contact_error_count { get; set; }
        public int? perf_contact_error_count { get; set; }
        public int? dt_contact_error_count { get; set; }
        public string platformtemplate_id { get; set; }
        public List<MRPWorkloadVolumeType> workloadvolumes_attributes { get; set; }
        public List<MRPWorkloadInterfaceType> workloadinterfaces_attributes { get; set; }
        public List<MRPWorkloadProcessType> workloadprocesses_attributes { get; set; }
        public List<MRPWorkloadSoftwareType> workloadsoftwares_attributes { get; set; }
        public MRPCredentialType credential { get; set; }
        public bool? deleted { get; set; }
        public int? last_dt_event_id { get; set; }
        public MRPDeploymentpolicyType deploymentpolicy { get; set; }
        public MRPPlatformtemplateType platformtemplate { get; set; }
        public MRPPlatformType platform { get; set; }
        public String primary_dns { get; set; }
        public String secondary_dns { get; set; }
        public string timezone { get; set; }
       
    }
}
