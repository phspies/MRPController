using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Contracts
{
    public class MRPRecoverypolicyType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("policy")]
        public string policy { get; set; }
        [JsonProperty("policytype")]
        public string policytype { get; set; }
        [JsonProperty("repositorypath")]
        public string repositorypath { get; set; }
        [JsonProperty("calculatesize")]
        public bool? calculatesize { get; set; }
        [JsonProperty("deleteorphanedfiles")]
        public bool? deleteorphanedfiles { get; set; }
        [JsonProperty("networkroute")]
        public string networkroute { get; set; }
        [JsonProperty("enablesnapshots")]
        public bool? enablesnapshots { get; set; }
        [JsonProperty("snapshotincrement")]
        public int? snapshotincrement { get; set; }
        [JsonProperty("snapshotstarttimestamp")]
        public DateTime? snapshotstarttimestamp { get; set; }
        [JsonProperty("snapshotmaxcount")]
        public int? snapshotmaxcount { get; set; }
        [JsonProperty("snapshotinterval")]
        public string snapshotinterval { get; set; }
        [JsonProperty("enablecompression")]
        public bool? enablecompression { get; set; }
        [JsonProperty("compressionlevel")]
        public int? compressionlevel { get; set; }
        [JsonProperty("enablebandwidthlimit")]
        public bool enablebandwidthlimit { get; set; }
        [JsonProperty("bandwidthlimit")]
        public int? bandwidthlimit { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("delete_current_jobs")]
        public bool? delete_current_jobs { get; set; }
        [JsonProperty("snapshotcount")]
        public int snapshotcount { get; set; }
        [JsonProperty("shutdown_source")]
        public bool? shutdown_source { get; set; }
        [JsonProperty("change_target_ports")]
        public bool? change_target_ports { get; set; }
        [JsonProperty("retain_network_configuration")]
        public bool? retain_network_configuration { get; set; }
        [JsonProperty("dns_set_dns")]
        public bool? dns_set_dns { get; set; }
        [JsonProperty("dns_servers")]
        public string dns_servers {get; set;}
        [JsonProperty("dns_domain")]
        public string dns_domain { get; set; }
        [JsonProperty("dns_user_domain")]
        public string dns_user_domain { get; set; }
        [JsonProperty("dns_username")]
        public string dns_username { get; set; }
        [JsonProperty("dns_password")]
        public string dns_password { get; set; }
        [JsonProperty("dns_should_update_ttl")]
        public bool? dns_should_update_ttl { get; set; }
        [JsonProperty("dns_ttl")]
        public int? dns_ttl { get; set; }
        [JsonProperty("linux_staging_path")]
        public string linux_staging_path { get; set; }
        [JsonProperty("windows_staging_path")]
        public string windows_staging_path { get; set; }

        [JsonProperty("sourceplatform")]
        public MRPPlatformType sourceplatform { get; set; }
        [JsonProperty("targetplatform")]
        public MRPPlatformType targetplatform { get; set; }
        [JsonProperty("source_journal_datastore")]
        public MRPPlatformdatastoreType source_journal_datastore { get; set; }
        [JsonProperty("rp4vm_source_journal_size")]
        public int? rp4vm_source_journal_size { get; set; }
        [JsonProperty("target_journal_datastore")]
        public MRPPlatformdatastoreType target_journal_datastore { get; set; }
        [JsonProperty("target_datastore")]
        public MRPPlatformdatastoreType target_datastore { get; set; }
        [JsonProperty("rp4vm_target_journal_size")]
        public int? rp4vm_target_journal_size { get; set; }
        [JsonProperty("rp4vm_replicationmode")]
        public string rp4vm_replicationmode { get; set; }
        [JsonProperty("mcp_journal_size")]
        public int? mcp_journal_size { get; set; }
        [JsonProperty("firedrill_network")]
        public MRPPlatformnetworkType firedrill_network { get; set; }
        [JsonProperty("target_network")]
        public MRPPlatformnetworkType target_network { get; set; }
        [JsonProperty("target_cluster")]
        public MRPPlatformclusterType target_cluster { get; set; }

    }
}
