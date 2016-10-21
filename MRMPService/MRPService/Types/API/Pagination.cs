using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPPaginationType
    {
        [JsonProperty("total_entries")]
        public int? total_entries { get; set; }
        [JsonProperty("total_pages")]
        public int? total_pages { get; set; }
        [JsonProperty("current_page")]
        public int? current_page { get; set; }
        [JsonProperty("page_size")]
        public int? page_size { get; set; }
        [JsonProperty("next_page")]
        public int? next_page { get; set; }
    }


}
