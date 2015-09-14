
using System;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    public class Workload
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string hostname { get; set; }
        public string cores { get; set; }
        public string memory { get; set; }
    }
}
