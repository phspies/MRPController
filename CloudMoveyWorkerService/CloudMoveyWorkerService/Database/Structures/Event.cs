using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    
    public class Event
    {
        public string id { get; set; }
        public string status { get; set; }
        public Nullable<int> severity { get; set; }
        public string component { get; set; }
        public string summary { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        public string entity { get; set; }
    }
}
