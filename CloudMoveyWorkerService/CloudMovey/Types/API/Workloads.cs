using System.Collections.Generic;

namespace CloudMoveyWorkerService.CloudMovey.Types.API
{
    public class MoveyWorkloadsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyWorkloadCRUDType server { get; set; }
    }
    public class MoveyWorkloadCRUDType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string failovergroup_id { get; set; }
        public string hostname { get; set; }
        public int vcpu { get; set; }
        public int vmemory { get; set; }
        public string ostype { get; set; }
        public object platform_ostype { get; set; }
        public string osedition { get; set; }
        public object failedover { get; set; }
        public bool enabled { get; set; }
        public bool active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string servertype { get; set; }
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
        public List<MoveyWorkloadVolumeType> serverdisks_attributes { get; set; }
        public List<MoveyWorkloadInterfaceType> serverinterfaces_attributes { get; set; }
    }
    public class MoveyWorkloadListType
    {
        public List<MoveyWorkloadType> servers { get; set; }
    }
    public class MoveyWorkloadType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string failovergroup_id { get; set; }
        public string hostname { get; set; }
        public int position { get; set; }
        public int vcpu { get; set; }
        public int vmemory { get; set; }
        public int cpu { get; set; }
        public int memory { get; set; }
        public string ostype { get; set; }
        public object platform_ostype { get; set; }
        public string osedition { get; set; }
        public object failedover { get; set; }
        public bool enabled { get; set; }
        public bool active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string servertype { get; set; }
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

        public List<MoveyWorkloadVolumeType> volumes { get; set; }
        public List<MoveyWorkloadInterfaceType> interfaces { get; set; }
    }
}
