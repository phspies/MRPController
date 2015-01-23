using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.Models
{
    public class ServerListObject
    {
        public List<ServerObject> servers { get; set; }
    }

    public class ServerObject
    {
        public string id { get; set; }
        public string platform_id { get; set; }
        public string failovergroup_id { get; set; }
        public string hostname { get; set; }
        public int position { get; set; }
        public int vcpu { get; set; }
        public int vmemory { get; set; }
        public string ostype { get; set; }
        public string osversion { get; set; }
        public object failedover { get; set; }
        public bool enabled { get; set; }
        public bool active { get; set; }
        public string moid { get; set; }
        public string credential_id { get; set; }
        public string servertype { get; set; }
        public bool provisioned { get; set; }
        public string dt_version { get; set; }
        public string dt_status { get; set; }
        public string dt_product { get; set; }
        public string dt_machine_name { get; set; }
        public string dt_unique_id { get; set; }
        public string lasterror { get; set; }
    }
    
}
