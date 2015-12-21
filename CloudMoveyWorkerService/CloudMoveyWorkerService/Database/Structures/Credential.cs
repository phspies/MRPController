using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Database
{
    
    public class Credential
    {
        public string id { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public Nullable<byte> credential_type { get; set; }
        public string human_type { get; set; }
        public string hash_value { get; set; }
    }
}
