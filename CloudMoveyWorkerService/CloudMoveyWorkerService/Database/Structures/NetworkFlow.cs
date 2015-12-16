using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    [ProtoBuf.ProtoContract]
    public class NetworkFlow
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string source_address { get; set; }
        [ProtoMember(3)]
        public string target_address { get; set; }
        [ProtoMember(4)]
        public uint source_port { get; set; }
        [ProtoMember(5)]
        public uint target_port { get; set; }
        [ProtoMember(6)]
        public uint protocol { get; set; }
        [ProtoMember(7)]
        public DateTime timestamp { get; set; }
        [ProtoMember(8)]
        public uint start_timestamp { get; set; }
        [ProtoMember(9)]
        public uint stop_timestamp { get; set; }
        [ProtoMember(10)]
        public uint packets { get; set; }
        [ProtoMember(11)]
        public uint kbyte { get; set; }
        
    }

}
