using CloudMoveyWorkerService.CloudMoveyWorkerService.Log;
using CloudMoveyWorkerService.Portal.Types.API;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace CloudMoveyWorkerService.Portal
{
    class MoveyWorker : Core
    {
        MoveyWorkerType worker = new MoveyWorkerType();
        public MoveyWorker(CloudMoveyPortal _CloudMovey) : base(_CloudMovey)
        {
            worker.worker_hostname = Environment.MachineName;
            worker.worker_version = Global.version_number;
            worker.worker_ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
            worker.worker_id = Global.agent_id;
        }
        public bool confirm_worker()
        {
            endpoint = ("/api/v1/workers/confirm.json");
            MoveyWorkerRegisterType returnval = post<MoveyWorkerRegisterType>(worker);
            if (returnval.worker.message == "Registered")
            {
                while (String.IsNullOrEmpty(((MoveyWorkerRegisterType)returnval).worker.organization_id))
                {
                    Logger.log("Worker registered, but not associated to a organization", Logger.Severity.Warn);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    returnval = post<MoveyWorkerRegisterType>(worker) as MoveyWorkerRegisterType;
                }
                Global.organization_id = ((MoveyWorkerRegisterType)returnval).worker.organization_id;
                return true;
            }
            else
            {
                return false;
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
