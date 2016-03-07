using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.LocalDatabase
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
        public Nullable<int> vcpu_speed { get; set; }
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
        [StringLength(1024)]
        public string iplist { get; set; }
        public Nullable<int> cpu_coresPerSocket { get; set; }
        public DateTime? perf_last_contact { get; set;}
        public bool perf_collection_status { get; set; }
        public DateTime? os_last_contact { get; set; }
        public Nullable<int> os_contact_error_count { get; set; }
        public Nullable<int> perf_contact_error_count { get; set; }
        public Nullable<int> dt_contact_error_count { get; set; }
        public bool os_collection_status { get; set; }
        public DateTime? dt_last_contact { get; set; }
        public bool dt_collection_status { get; set; }
        [StringLength(255)]
        public string perf_collection_message { get; set; }
        [StringLength(255)]
        public string os_collection_message { get; set; }
        [StringLength(255)]
        public string dt_collection_message { get; set; }
    }
}
