using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRPService.Types.API
{
    public class MRPProtectiongroupType
    {
        public string id { get; set; }
        public string service { get; set; }
        public string supportservice_id { get; set; }
        public int position { get; set; }
        public string organization_id { get; set; }
        public string protectiongrouptype { get; set; }
        public int currentstep { get; set; }
        public string recoverypolicy_id { get; set; }
        public string repository_workload_id { get; set; }
        public MRPRecoverypolicyType recoverypolicy { get; set; }
    }
}
