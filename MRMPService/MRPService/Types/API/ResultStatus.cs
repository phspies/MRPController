﻿using Newtonsoft.Json;

namespace MRMPService.API.Types.API
{
    [JsonObject(Description = "result")]
    public class ResultType
    {
        public ResultDetailType result { get; set; }
    }
    public class ResultDetailType
    { 
        public bool status { get; set; }
        public string message { get; set; }
    }
}
