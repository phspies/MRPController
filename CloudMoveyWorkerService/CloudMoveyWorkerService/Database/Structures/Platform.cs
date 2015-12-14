using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    public class Platform
    {
        public string id { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string datacenter { get; set; }
        public Nullable<byte> vendor { get; set; }
        public string url { get; set; }
        public string credential_id { get; set; }
        public Nullable<int> passwordok { get; set; }
        public Nullable<System.DateTime> lastupdated { get; set; }
        public string platform_details { get; set; }
        public string human_vendor { get; set; }
        public Nullable<int> workload_count { get; set; }
        public Nullable<int> vlan_count { get; set; }
        public Nullable<int> networkdomain_count { get; set; }
        public string platform_version { get; set; }
        public string moid { get; set; }
        public string workload_sha1 { get; set; }
        public string vlan_sha1 { get; set; }
        public string networkdomain_sha1 { get; set; }
        public string hash_value { get; set; }
    }
}
