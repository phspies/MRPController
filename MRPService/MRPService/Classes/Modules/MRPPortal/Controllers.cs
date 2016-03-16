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
        MRPControllerType worker = new MRPControllerType();
        public MRPWorker(MRP_ApiClient _MRP) : base(_MRP)
        {
            worker.controller_hostname = Environment.MachineName;
            worker.controller_version = Global.version_number;
            worker.controller_ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
        }
        public bool confirm_controller()
        {
            endpoint = ("/api/v1/controllers/confirm.json");
            MRPControllerConfirmType returnval = post<MRPControllerConfirmType>(worker);

            while (String.IsNullOrEmpty(((MRPControllerConfirmType)returnval).worker.organization_id))
            {
                Logger.log("Controller not registered!", Logger.Severity.Warn);
                Thread.Sleep(new TimeSpan(0, 0, 30));
                returnval = post<MRPControllerConfirmType>(worker) as MRPControllerConfirmType;
            }
            Global.organization_id = ((MRPControllerConfirmType)returnval).worker.organization_id;
            return true;
        }
    }
}
