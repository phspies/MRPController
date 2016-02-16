using System.Collections.Generic;

namespace MRPService.Portal.Types.API
{

    public class MRPWorkloadGETType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public string workload_id { get; set; }
    }
    public class MRPWorkloadsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MRPWorkloadCRUDType workload { get; set; }
    }
    public class MRPWorkloadCRUDType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string failovergroup_id { get; set; }
        public string hostname { get; set; }
        public string iplist { get; set; }
        public int? vcpu { get; set; }
        public int? vcore { get; set; }
        public int? vmemory { get; set; }
        public string ostype { get; set; }
        public string platform_ostype { get; set; }
        public string osedition { get; set; }
        public bool? failedover { get; set; }
        public bool? enabled { get; set; }
        public bool? active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string workloadtype { get; set; }
        public object configuration { get; set; }
        public bool? provisioned { get; set; }
        public bool? dt_installed { get; set; }
        public string dt_machine_name { get; set; }
        public string dt_unique_id { get; set; }
        public string dt_status { get; set; }
        public string dt_version { get; set; }
        public object lasterror { get; set; }
        public object lastcontact { get; set; }
        public string deploymentpolicy_id { get; set; }
        public string password { get; set; }
        public string ipaddress { get; set; }
        public string hash_value { get; set; }
        public bool? perf_collection_status { get; set; }
        public bool? os_collection_status { get; set; }
        public bool? dt_collection_status { get; set; }
        public string perf_collection_message { get; set; }
        public string os_collection_message { get; set; }
        public string dt_collection_message { get; set; }
        public string platformtemplate_id { get; set; }
        public List<MRPWorkloadDiskType> workloaddisks_attributes { get; set; }
        public List<MRPWorkloadVolumeType> workloadvolumes_attributes { get; set; }
        public List<MRPWorkloadInterfaceType> workloadinterfaces_attributes { get; set; }
        public List<MRPWorkloadProcessType> workloadprocesses_attributes { get; set; }
        public List<MRPWorkloadSoftwareType> workloadsoftwares_attributes { get; set; }
    }
    public class MRPWorkloadListType
    {
        public List<MRPWorkloadType> workloads { get; set; }
    }
    public class MRPWorkloadType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string failovergroup_id { get; set; }
        public string hostname { get; set; }
        public string iplist { get; set; }
        public int vcpu { get; set; }
        public int vmemory { get; set; }
        public int vcore { get; set; }
        public string ostype { get; set; }
        public object platform_ostype { get; set; }
        public string osedition { get; set; }
        public object failedover { get; set; }
        public bool enabled { get; set; }
        public bool active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string workloadtype { get; set; }
        public object configuration { get; set; }
        public bool provisioned { get; set; }
        public bool dt_installed { get; set; }
        public object dt_machine_name { get; set; }
        public object dt_unique_id { get; set; }
        public object dt_status { get; set; }
        public string dt_version { get; set; }
        public object lasterror { get; set; }
        public object lastcontact { get; set; }
        public string deploymentpolicy_id { get; set; }
        public string password { get; set; }
        public string ipaddress { get; set; }
        public string hash_value { get; set; }
        public bool? perf_collection_status { get; set; }
        public bool? os_collection_status { get; set; }
        public bool? dt_collection_status { get; set; }
        public string perf_collection_message { get; set; }
        public string os_collection_message { get; set; }
        public string dt_collection_message { get; set; }
        public string platformtemplate_id { get; set; }
        public List<MRPWorkloadDiskType> disks { get; set; }
        public List<MRPWorkloadInterfaceType> interfaces { get; set; }
        public List<MRPWorkloadProcessType> processes { get; set; }
        public List<MRPWorkloadSoftwareType> softwares { get; set; }
        public List<MRPWorkloadVolumeType> volumes { get; set; }
    }
}
