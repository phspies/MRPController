using CladesWorkerService.Clades.Models;
using CladesWorkerService.Clades.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades
{
    [JsonObject(MemberSerialization.OptIn)]
    class Worker : Core 
    {
        public Worker(Clades _clades) : base(_clades) { }

        [JsonProperty]
        public string id { get; set; }
        [JsonProperty]
        public string hostname { get; set; }
        [JsonProperty]
        public string worker_version { get; set; }
        [JsonProperty]
        public string ipaddress { get; set; }
        public bool confirm_worker()
        {
            endpoint = ("/api/v1/workers/confirm.json");
            Object returnval = post(this);
            if (returnval is Error)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool register_worker()
        {
            endpoint = ("/api/v1/workers/register.json");
            Object returnval = post(this);
            if (returnval is Error)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }


}


