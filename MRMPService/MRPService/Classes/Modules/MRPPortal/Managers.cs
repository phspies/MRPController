using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI
{
    class MRPManager : Core
    {
        MRPManagerType worker = new MRPManagerType();
        public MRPManager(MRMP_ApiClient _MRP) : base(_MRP)
        {
            worker.hostname = Environment.MachineName;
            worker.version = Global.manager_version;
            worker.ipaddress = String.Join(",", Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
        }
        public ResultType update_managerevents(List<MRPManagerEventType> _events)
        {
            MRManagerCRUDType _manager_type = new MRManagerCRUDType()
            {
                workload = new MRPManagerType()
                {
                    managerevents = _events
                }
            };
            endpoint = "/managers/update.json";
            return post<ResultType>(_manager_type);
        }
        public bool confirm_controller()
        {
            endpoint = ("/managers/confirm.json");
            ResultType returnval = post<ResultType>(worker);

            while (returnval.result.status == false)
            {
                Logger.log("Manager not registered with portal!", Logger.Severity.Warn);
                Thread.Sleep(new TimeSpan(0, 0, 30));
                returnval = post<ResultType>(worker) as ResultType;
            }
            Global.organization_id = returnval.result.organization_id;
            if (Global.organization_id == null)
            {
                Logger.log("No Organization ID Detected! - Exiting!!!", Logger.Severity.Fatal);
                System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("MRMP Service");
                svc.Stop();
            }
            return true;
        }
    }
}
