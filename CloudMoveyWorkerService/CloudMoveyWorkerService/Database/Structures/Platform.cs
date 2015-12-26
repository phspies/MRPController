using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.LocalDatabase
{
    
    public class Platform
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(100)]
        public string description { get; set; }
        [StringLength(50)]
        public string username { get; set; }
        [StringLength(50)]
        public string password { get; set; }
        [StringLength(50)]
        public string datacenter { get; set; }
        public int vendor { get; set; }
        [StringLength(50)]
        public string url { get; set; }
        [StringLength(50)]
        public string credential_id { get; set; }
        public Nullable<int> passwordok { get; set; }
        public Nullable<System.DateTime> lastupdated { get; set; }
        [StringLength(50)]
        public string platform_details { get; set; }
        [StringLength(50)]
        public string human_vendor { get; set; }
        public Nullable<int> workload_count { get; set; }
        public Nullable<int> vlan_count { get; set; }
        public Nullable<int> networkdomain_count { get; set; }
        [StringLength(50)]
        public string platform_version { get; set; }
        [StringLength(50)]
        public string moid { get; set; }
        [StringLength(50)]
        public string workload_sha1 { get; set; }
        [StringLength(50)]
        public string vlan_sha1 { get; set; }
        [StringLength(50)]
        public string networkdomain_sha1 { get; set; }
        [StringLength(50)]
        public string hash_value { get; set; }
    }
}
