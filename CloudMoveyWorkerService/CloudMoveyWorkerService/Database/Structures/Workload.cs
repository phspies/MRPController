using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.LocalDatabase
{
    
    public class Workload
    {
        [Key, StringLength(50)]
        public string id { get; set; }
        [StringLength(50)]
        public string hostname { get; set; }
        [StringLength(50)]
        public string platform_id { get; set; }
        [StringLength(50)]
        public string credential_id { get; set; }
        [StringLength(50)]
        public string hash_value { get; set; }
        [StringLength(50)]
        public string failovergroup_id { get; set; }
        [StringLength(50)]
        public string moid { get; set; }
        public Nullable<bool> enabled { get; set; }
        public Nullable<int> vcpu { get; set; }
        public Nullable<int> vcore { get; set; }
        public Nullable<int> vmemory { get; set; }
        public Nullable<long> storage_count { get; set; }
        public bool credential_ok { get; set; }
        [StringLength(50)]
        public string application { get; set; }
        [StringLength(50)]
        public string osedition { get; set; }
        [StringLength(50)]
        public string ostype { get; set; }
        public Nullable<System.DateTime> last_contact_attempt { get; set; }
        public Nullable<int> last_contact_status { get; set; }
        [StringLength(50)]
        public string last_contact_message { get; set; }
        public Nullable<int> failed_contact_attempts { get; set; }
        [StringLength(255)]
        public string iplist { get; set; }
        public Nullable<int> cpu_coresPerSocket { get; set; }
        public bool perf_collection { get; set; }
    }
}
