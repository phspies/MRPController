using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
{
    public class RecoveryJobListObject
    {
        public List<RecoveryJobObject> recoveryjobs { get; set; }
    }

    public class RecoveryJobObject
    {
        public string id { get; set; }
        public object name { get; set; }
        public string jobtype { get; set; }
        public string source_workload_id { get; set; }
        public string target_workload_id { get; set; }
        public string target_platform_id { get; set; }
        public object target_platformnetwork_id { get; set; }
        public string repository_id { get; set; }
        public object activity { get; set; }
        public object mirrorstatus { get; set; }
        public object replicationstatus { get; set; }
        public object transmitmode { get; set; }
        public object recoverypolicy_id { get; set; }
        public string moid { get; set; }
        public string source_moid { get; set; }
        public string target_moid { get; set; }
    }
}
