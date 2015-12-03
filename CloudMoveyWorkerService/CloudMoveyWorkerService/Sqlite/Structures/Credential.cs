using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Sqlite
{
    public class Credential
    {
        public string id { get; set; }
        public CREDENTIAL_TYPE type { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
    }
    public enum CREDENTIAL_TYPE { workload, platform }
}
