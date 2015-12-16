using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    [ProtoBuf.ProtoContract]
    public class Workload
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string hostname { get; set; }
        [ProtoMember(3)]
        public string platform_id { get; set; }
        [ProtoMember(4)]
        public string credential_id { get; set; }
        [ProtoMember(5)]
        public string hash_value { get; set; }
        [ProtoMember(6)]
        public string failovergroup_id { get; set; }
        [ProtoMember(7)]
        public string moid { get; set; }
        [ProtoMember(8)]
        public Nullable<bool> enabled { get; set; }
        [ProtoMember(9)]
        public Nullable<int> cpu_count { get; set; }
        [ProtoMember(10)]
        public Nullable<int> memory_count { get; set; }
        [ProtoMember(11)]
        public Nullable<long> storage_count { get; set; }
        [ProtoMember(12)]
        public bool credential_ok { get; set; }
        [ProtoMember(13)]
        public string application { get; set; }
        [ProtoMember(14)]
        public string osedition { get; set; }
        [ProtoMember(15)]
        public string ostype { get; set; }
        [ProtoMember(16)]
        public Nullable<System.DateTime> last_contact_attempt { get; set; }
        [ProtoMember(17)]
        public Nullable<int> last_contact_status { get; set; }
        [ProtoMember(18)]
        public string last_contact_message { get; set; }
        [ProtoMember(19)]
        public Nullable<int> failed_contact_attempts { get; set; }
        [ProtoMember(20)]
        public string iplist { get; set; }
        [ProtoMember(21)]
        public Nullable<int> cpu_coresPerSocket { get; set; }
        [ProtoMember(22)]
        public bool perf_collection { get; set; }
    }
}
