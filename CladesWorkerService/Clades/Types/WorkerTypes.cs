using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CladesWorkerService.Clades.Types
{
    public class Worker
    {
        public string id { get; set; }
        public string customer_id { get; set; }
        public string hostname { get; set; }
        public string ipaddress { get; set; }
        public bool status { get; set; }
    }
}

