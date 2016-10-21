using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPWorkloadsCRUDType : MRPManagerIDType
    {
        [JsonProperty("workload")]
        public MRPWorkloadType workload { get; set; }
    }

    public class MRPWorkloadFilterPagedType : MRPManagerIDType
    {
        [JsonProperty("enabled")]
        public bool? enabled { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("perf_collection_enabled")]
        public bool? perf_collection_enabled { get; set; }
        [JsonProperty("netstat_collection_enabled")]
        public bool? netstat_collection_enabled { get; set; }
        [JsonProperty("os_collection_enabled")]
        public bool? os_collection_enabled { get; set; }
        [JsonProperty("dt_installed")]
        public bool? dt_installed { get; set; }
        [JsonProperty("page")]
        public int? page { get; set; }
    }
    public class MRPWorkloadGetIDType : MRPManagerIDType
    {
        [JsonProperty("workload_id")]
        public String workload_id { get; set; }
    }
    public class MRPWorkloadGetMOIDType : MRPManagerIDType
    {
        [JsonProperty("moid_id")]
        public String moid_id { get; set; }
    }

    public class MRPWorkloadListType
    {
        [JsonProperty("workloads")]
        public List<MRPWorkloadType> workloads { get; set; }
        [JsonProperty("pagination")]
        public MRPPaginationType pagination { get; set; }
    }
    public class MRPWorkloadPairType
    {
        [JsonProperty("source")]
        public MRPWorkloadType source { get; set; }
        [JsonProperty("target")]
        public MRPWorkloadType target { get; set; }
    }
    public class MRPWorkloadType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("iplist")]
        public string iplist { get; set; }
        [JsonProperty("vcpu")]
        public int? vcpu { get; set; }
        [JsonProperty("vcpu_speed")]
        public int? vcpu_speed { get; set; }
        [JsonProperty("vmemory")]
        public int? vmemory { get; set; }
        [JsonProperty("vcore")]
        public int? vcore { get; set; }
        [JsonProperty("ostype")]
        public string ostype { get; set; }
        [JsonProperty("platform_ostype")]
        public string platform_ostype { get; set; }
        [JsonProperty("osedition")]
        public string osedition { get; set; }
        [JsonProperty("serialnumber")]
        public string serialnumber { get; set; }
        [JsonProperty("model")]
        public string model { get; set; }
        [JsonProperty("failedover")]
        public bool? failedover { get; set; }
        [JsonProperty("enabled")]
        public bool? enabled { get; set; }
        [JsonProperty("active")]
        public bool? active { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("vcenter_uuid")]
        public string vcenter_uuid { get; set; }      
        [JsonProperty("credential_id")]
        public string credential_id { get; set; }
        [JsonProperty("workloadtype")]
        public string workloadtype { get; set; }
        [JsonProperty("configuration")]
        public string configuration { get; set; }
        [JsonProperty("provisioned")]
        public bool? provisioned { get; set; }
        [JsonProperty("dt_installed")]
        public bool? dt_installed { get; set; }
        [JsonProperty("dt_version")]
        public string dt_version { get; set; }
        [JsonProperty("deploymentpolicy_id")]
        public string deploymentpolicy_id { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
        [JsonProperty("hash_value")]
        public string hash_value { get; set; }
        [JsonProperty("perf_collection_enabled")]
        public bool? perf_collection_enabled { get; set; }
        [JsonProperty("os_collection_enabled")]
        public bool? os_collection_enabled { get; set; }
        [JsonProperty("dt_collection_enabled")]
        public bool? dt_collection_enabled { get; set; }
        [JsonProperty("netstat_collection_enabled")]
        public bool? netstat_collection_enabled { get; set; }
        [JsonProperty("perf_collection_status")]
        public bool? perf_collection_status { get; set; }
        [JsonProperty("netstat_collection_status")]
        public bool? netstat_collection_status { get; set; }
        [JsonProperty("os_collection_status")]
        public bool? os_collection_status { get; set; }
        [JsonProperty("dt_collection_status")]
        public bool? dt_collection_status { get; set; }
        [JsonProperty("perf_collection_message")]
        public string perf_collection_message { get; set; }
        [JsonProperty("netstat_collection_message")]
        public string netstat_collection_message { get; set; }
        [JsonProperty("os_collection_message")]
        public string os_collection_message { get; set; }
        [JsonProperty("dt_collection_message")]
        public string dt_collection_message { get; set; }
        [JsonProperty("netstat_last_contact")]
        public DateTime? netstat_last_contact { get; set; }
        [JsonProperty("os_last_contact")]
        public DateTime? os_last_contact { get; set; }
        [JsonProperty("perf_last_contact")]
        public DateTime? perf_last_contact { get; set; }
        [JsonProperty("dt_last_contact")]
        public DateTime? dt_last_contact { get; set; }
        [JsonProperty("os_contact_error_count")]
        public int? os_contact_error_count { get; set; }
        [JsonProperty("perf_contact_error_count")]
        public int? perf_contact_error_count { get; set; }
        [JsonProperty("dt_contact_error_count")]
        public int? dt_contact_error_count { get; set; }
        [JsonProperty("platformtemplate_id")]
        public string platformtemplate_id { get; set; }
        [JsonProperty("workloadvolumes_attributes")]
        public List<MRPWorkloadVolumeType> workloadvolumes_attributes { get; set; }
        [JsonProperty("workloadinterfaces_attributes")]
        public List<MRPWorkloadInterfaceType> workloadinterfaces_attributes { get; set; }
        [JsonProperty("workloadprocesses_attributes")]
        public List<MRPWorkloadProcessType> workloadprocesses_attributes { get; set; }
        [JsonProperty("workloadsoftwares_attributes")]
        public List<MRPWorkloadSoftwareType> workloadsoftwares_attributes { get; set; }
        [JsonProperty("credential")]
        public MRPCredentialType credential { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("last_dt_event_id")]
        public int? last_dt_event_id { get; set; }
        [JsonProperty("deploymentpolicy")]
        public MRPDeploymentpolicyType deploymentpolicy { get; set; }
        [JsonProperty("platformtemplate")]
        public MRPPlatformtemplateType platformtemplate { get; set; }
        [JsonProperty("platform")]
        public MRPPlatformType platform { get; set; }
        [JsonProperty("primary_dns")]
        public String primary_dns { get; set; }
        [JsonProperty("secondary_dns")]
        public String secondary_dns { get; set; }
        [JsonProperty("timezone")]
        public string timezone { get; set; }
       
    }
}
