using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.API.Types.API
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
        public bool enablesnapshots { get; set; }
        public int snapshotincrement { get; set; }
        public string snapshotinterval { get; set; }
        public bool enablecompression { get; set; }
        public int compressionlevel { get; set; }
        public bool enablebandwidthlimit { get; set; }
        public int bandwidthlimit { get; set; }
        public string organization_id { get; set; }
        public bool delete_current_jobs { get; set; }
        public int snapshotcount { get; set; }
        public bool shutdown_source { get; set; }
        public bool change_target_ports { get; set; }
        public bool retain_network_configuration { get; set; }
    }
}
