using CloudMoveyWorkerService.CloudMovey.Types.API;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyWorker : Core
    {
        MoveyWorkerType worker = new MoveyWorkerType();
        public MoveyWorker(CloudMovey _CloudMovey) : base(_CloudMovey)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            worker.worker_hostname = Environment.MachineName;
            worker.worker_version = Global.version_number;
            worker.worker_ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
            worker.worker_id = Global.agent_id;
        }
        public bool confirm_worker()
        {
            endpoint = ("/api/v1/workers/confirm.json");
            Object returnval = post<MoveyWorkerRegisterType>(worker);
            if (returnval is MoveyError)
            {
                return false;
            }
            else
            {
                Global.organization_id = ((MoveyWorkerRegisterType)returnval).worker.organization_id;
                return true;
            }
        }
        public bool register_worker()
        {
            endpoint = ("/api/v1/workers/register.json");
            Object returnval = post<MoveyWorkerType>(worker);
            if (returnval is MoveyError)
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
