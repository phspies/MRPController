using Newtonsoft.Json;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPTaskWindowsType
    {
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
        [JsonProperty("domain")]
        public string domain { get; set; }
    }
}
