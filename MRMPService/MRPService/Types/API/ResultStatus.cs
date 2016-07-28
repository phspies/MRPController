using Newtonsoft.Json;

namespace MRMPService.MRMPAPI.Types.API
{
    public class ResultType
    {
        [JsonProperty("result")]
        public ResultDetailType result { get; set; }
    }
    public class ResultDetailType
    {
        [JsonProperty("status")]
        public bool status { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
    }
}
