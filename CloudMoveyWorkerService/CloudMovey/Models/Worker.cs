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

namespace CloudMoveyWorkerService.CloudMovey
{
    [JsonObject(MemberSerialization.OptIn)]
    class Worker : Core 
    {
        public Worker(CloudMovey _CloudMovey) : base(_CloudMovey) {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

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




   [JsonObject(MemberSerialization.OptIn)]
    class CommandWorker : Core 
    {
        public CommandWorker(CloudMovey _CloudMovey) : base(_CloudMovey) { }

        [JsonProperty]
        public string id { get; set; }
        [JsonProperty]
        public string hostname { get; set; }
    }


}
