using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.MRPService.Types.API
{
    public class MRPDeploymentpolicyType
    {
        public string id { get; set; }
        public string policy { get; set; }
        public string dt_installpath { get; set; }
        public string dt_temppath { get; set; }
        public string dt_inifile { get; set; }
        public string organization_id { get; set; }
        public bool _default { get; set; }
        public bool enabled { get; set; }
        public int dt_max_memory { get; set; }
        public string dt_queue_folder { get; set; }
        public int dt_queue_limit_disk_size { get; set; }
        public int dt_queue_min_disk_free_size { get; set; }
        public string dt_queue_scheme { get; set; }
        public string source_activation_code { get; set; }
        public string target_activation_code { get; set; }

    }
}
