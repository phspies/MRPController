using Newtonsoft.Json;

namespace MRMPService.MRMPService.Types.API
{

    public class MRPProtectiongrouptreeCRUDType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
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
