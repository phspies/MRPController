using MRPService.MRPService.Log;
using MRPService.API.Types.API;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace MRPService.API
{
    class MRPWorker : Core
    {
        MRPWorkerType worker = new MRPWorkerType();
        public MRPWorker(MRP_ApiClient _MRP) : base(_MRP)
        {
            worker.worker_hostname = Environment.MachineName;
            worker.worker_version = Global.version_number;
            worker.worker_ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
            worker.worker_id = Global.agent_id;
        }
        public bool confirm_worker()
        {
            endpoint = ("/api/v1/workers/confirm.json");
            MRPWorkerRegisterType returnval = post<MRPWorkerRegisterType>(worker);
            if (returnval.worker.message == "Registered")
            {
                while (String.IsNullOrEmpty(((MRPWorkerRegisterType)returnval).worker.organization_id))
                {
                    Logger.log("Worker registered, but not associated to a organization", Logger.Severity.Warn);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    returnval = post<MRPWorkerRegisterType>(worker) as MRPWorkerRegisterType;
                }
                Global.organization_id = ((MRPWorkerRegisterType)returnval).worker.organization_id;
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
            Object returnval = post<MRPWorkerType>(worker);
            if (returnval is MRPError)
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
