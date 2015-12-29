using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.Portal.Types.API
{
    public class MoveyPlatformdomainsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyPlatformdomainCRUDType platformdomain { get; set; }
    }
    public class MoveyPlatformdomainListType
    {
        public List<MoveyPlatformdomainType> platformdomains { get; set; }
    }
    public class MoveyPlatformdomainCRUDType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string domain { get; set; }
        public string moid { get; set; }
    }
    public class MoveyPlatformdomainType
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string domain { get; set; }
        public string moid { get; set; }

    }
}
