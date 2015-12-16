using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    [ProtoBuf.ProtoContract]
    public class Platform
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string description { get; set; }
        [ProtoMember(3)]
        public string username { get; set; }
        [ProtoMember(4)]
        public string password { get; set; }
        [ProtoMember(5)]
        public string datacenter { get; set; }
        [ProtoMember(6)]
        public Nullable<byte> vendor { get; set; }
        [ProtoMember(7)]
        public string url { get; set; }
        [ProtoMember(8)]
        public string credential_id { get; set; }
        [ProtoMember(9)]
        public Nullable<int> passwordok { get; set; }
        [ProtoMember(10)]
        public Nullable<System.DateTime> lastupdated { get; set; }
        [ProtoMember(11)]
        public string platform_details { get; set; }
        [ProtoMember(12)]
        public string human_vendor { get; set; }
        [ProtoMember(13)]
        public Nullable<int> workload_count { get; set; }
        [ProtoMember(14)]
        public Nullable<int> vlan_count { get; set; }
        [ProtoMember(15)]
        public Nullable<int> networkdomain_count { get; set; }
        [ProtoMember(16)]
        public string platform_version { get; set; }
        [ProtoMember(17)]
        public string moid { get; set; }
        [ProtoMember(18)]
        public string workload_sha1 { get; set; }
        [ProtoMember(19)]
        public string vlan_sha1 { get; set; }
        [ProtoMember(20)]
        public string networkdomain_sha1 { get; set; }
        [ProtoMember(21)]
        public string hash_value { get; set; }
    }
}
