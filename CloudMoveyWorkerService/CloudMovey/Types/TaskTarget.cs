using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Types
{

    public class Target
    {
        public string password { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public int cpu { get; set; }
        public int memory { get; set; }
        public List<Volume> volumes { get; set; }
        public List<Interface> interfaces { get; set; }
        public string ostype { get; set; }
        public string osedition { get; set; }
    }

    public class Volume
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int diskindex { get; set; }
        public string driveletter { get; set; }
        public int disksize { get; set; }
        public int? diskfreesize { get; set; }
        public string platformstoragetier_id { get; set; }
        public string moid { get; set; }
        public int position { get; set; }
        public bool provisioned { get; set; }
        public Platformstoragetier platformstoragetier { get; set; }
    }

    public class Platformstoragetier
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string storagetier { get; set; }
        public string shortname { get; set; }
    }

    public class Interface
    {
        public string id { get; set; }
        public string workload_id { get; set; }
        public int vnic { get; set; }
        public string ipaddress { get; set; }
        public string ipv6address { get; set; }
        public string netmask { get; set; }
        public string ipv6netmask { get; set; }
        public string platformnetwork_id { get; set; }
        public string ipassignment { get; set; }
        public object moid { get; set; }
        public MoveyPlatformnetworkType platformnetwork { get; set; }
    }

}
