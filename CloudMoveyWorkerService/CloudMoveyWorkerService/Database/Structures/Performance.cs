using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    [ProtoBuf.ProtoContract]
    public class Performance
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string workload_id { get; set; }
        [ProtoMember(3)]
        public DateTime timestamp { get; set; }
        [ProtoMember(4)]
        public string category_name { get; set; }
        [ProtoMember(5)]
        public string counter_name { get; set; }
        [ProtoMember(6)]
        public string instance { get; set; }
        [ProtoMember(7)]
        public double value { get; set; }       
    }
}
