using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Portal.Types.API
{
    public class MRPRecoverypolicyType
    {
        public string id { get; set; }
        public string policy { get; set; }
        public string policytype { get; set; }
        public string repositorypath { get; set; }
        public object mirrortype { get; set; }
        public object calculatesize { get; set; }
        public object deleteorphanedfiles { get; set; }
        public string networkroute { get; set; }
        public object enablesnapshots { get; set; }
        public object snapshotinterval { get; set; }
        public object snapshottimestamp { get; set; }
        public bool enablecompression { get; set; }
        public int compressionlevel { get; set; }
        public bool enablebandwidthrottle { get; set; }
        public int bandwidththrottlelimit { get; set; }
        public string organization_id { get; set; }
    }
}
