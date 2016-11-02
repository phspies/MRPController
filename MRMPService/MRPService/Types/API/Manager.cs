using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPManagerType
    {
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("version")]
        public string version { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
    }
}
