using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPWorkloadDiskType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public Int64 disksize { get; set; }
        public string platformstoragetier_id { get; set; }
        public string moid { get; set; }
        public int position { get; set; }
        public bool provisioned { get; set; }
        public string deviceid { get; set; }
        public int _destroy { get; set; }
        public MRPPlatformStorageTierType platformstoragetier { get; set; }
    }
}
