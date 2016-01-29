using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPWorkloadVolumeType
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public string driveletter { get; set; }
        public Int64 volumesize { get; set; }
        public Int64 volumefreespace { get; set; }
        public bool provisioned { get; set; }
        public string serialnumber { get; set; }
        public string volumename { get; set; }
        public string deviceid { get; set; }
        public Int64 blocksize { get; set; }
        public int _destroy { get; set; }
        public MRPPlatformStorageTierType platformstoragetier { get; set; }
    }
}
