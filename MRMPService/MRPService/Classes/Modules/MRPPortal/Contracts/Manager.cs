using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MRMPService.MRMPAPI.Contracts
{

    public class MRPManagerType
    {
        [JsonProperty("id")]
        public string id { get { return MRMPServiceBase.manager_id; } set { } }
        [JsonProperty("hostname")]
        public string hostname { get; set; }
        [JsonProperty("version")]
        public string version { get; set; }
        [JsonProperty("ipaddress")]
        public string ipaddress { get; set; }
        [JsonProperty("managerevents_attributes")]
        public List<MRPManagerEventType> managerevents { get; set; }
    }
    public class MRManagerCRUDType
    {
        [JsonProperty("manager")]
        public MRPManagerType workload { get; set; }
    }
}
