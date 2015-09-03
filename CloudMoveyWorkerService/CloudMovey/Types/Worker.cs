using CloudMoveyWorkerService.CloudMovey;
using CloudMoveyWorkerService.CloudMovey.Models;
using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Types
{
    class MoveyWorkerType
    {
        public string id { get; set; }
        public string hostname { get; set; }
        public string worker_version { get; set; }
        public string ipaddress { get; set; }
    }
}
