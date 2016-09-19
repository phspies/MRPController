using DD.CBU.Compute.Api.Contracts.Network20;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPlatformdatacenterCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("platformdatacenter")]
        public MRPPlatformdatacenterType platformdatacenter { get; set; }
    }
    public class MRPPlatformdatacenterListType
    {
        [JsonProperty("platformdatacenters")]
        public List<MRPPlatformdatacenterType> platformdatacenters { get; set; }
    }
    public class MRPPlatformdatacenterType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("maxcpu")]
        public int? maxcpu { get; set; }
        [JsonProperty("maxmemory")]
        public int? maxmemory { get; set; }
        [JsonProperty("maxdiskcount")]
        public int? maxdiskcount { get; set; }
        [JsonProperty("maxdisksize")]
        public int? maxdisksize { get; set; }
        [JsonProperty("maxnetwork")]
        public int? maxnetwork { get; set; }
        [JsonProperty("mindisksize")]
        public int? mindisksize { get; set; }
        [JsonProperty("minmemory")]
        public int? minmemory { get; set; }
        [JsonProperty("city")]
        public string city { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("country")]
        public string country { get; set; }
        [JsonProperty("vpn_url")]
        public string vpn_url { get; set; }
        [JsonProperty("diskspeeds")]
        public DiskSpeedType[] diskspeeds { get; set; }
        [JsonProperty("cpuspeeds")]
        public CpuSpeedType[] cpuspeeds { get; set; }
        [JsonProperty("deleted")]
        public bool? deleted { get; set; }
        [JsonProperty("displayname")]
        public string displayname { get; set; }
        [JsonProperty("virtualcenter_uid")]
        public string virtualcenter_uid { get; set; }
        [JsonProperty("target_drs_moid_list")]
        public string target_drs_moid_list { get; set; }
        [JsonProperty("platformclusters_attributes")]
        public List<MRPPlatformclusterType> platformclusters_attributes { get; set; }
    }
}
