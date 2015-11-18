using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPlatformStorageTierType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string storagetier { get; set; }
        public string shortname { get; set; }
    }
}
