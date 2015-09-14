using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Types.API
{
    public class MoveyWorkloadVolumeType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public string driveletter { get; set; }
        public int disksize { get; set; }
        public int diskfreesize { get; set; }
        public string platformstoragetier_id { get; set; }
        public string moid { get; set; }
        public int position { get; set; }
        public bool provisioned { get; set; }
        public MoveyPlatformStorageTierType platformstoragetier { get; set; }
    }
}
