using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    [ProtoBuf.ProtoContract]
    public class Event
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string status { get; set; }
        [ProtoMember(3)]
        public Nullable<int> severity { get; set; }
        [ProtoMember(4)]
        public string component { get; set; }
        [ProtoMember(5)]
        public string summary { get; set; }
        [ProtoMember(6)]
        public Nullable<System.DateTime> timestamp { get; set; }
        [ProtoMember(7)]
        public string entity { get; set; }
    }
}
