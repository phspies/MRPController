using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MRMPService.MRMPAPI.Types.API
{
    class MRPManagerType
    {
        [JsonProperty("manager_id")]
        public string manager_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("version")]
        public string version { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
    }
    public class MRPManagerConfirmType
    {
        [JsonProperty("manager")]
        public Manager manager { get; set; }
    }
    public class Manager
    {
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("status")]
        public bool status { get; set; }
    }


}
