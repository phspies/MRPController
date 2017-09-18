using Newtonsoft.Json;

namespace MRMPService.Modules.MRMPPortal.Contracts
{

    public class MRPProtectiongrouptreeCRUDType
    {
        [JsonProperty("protectiongrouptree")]
        public MRPProtectiongrouptreeType protectiongrouptree { get; set; }
    }
    public class MRPProtectiongrouptreeType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("dt_job_id")]
        public string dt_job_id { get; set; }
    }
}
