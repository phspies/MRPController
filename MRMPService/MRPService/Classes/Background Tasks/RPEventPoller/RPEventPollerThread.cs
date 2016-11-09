using MRMPService.MRMPService.Log;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace MRMPService.RPEventPollerCollection
{
    class RPEventPollerThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(Global.rp4vm_group_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring RP4VM collection process with {0} threads", Global.os_performance_concurrency), Logger.Severity.Info);

                List<MRPManagementobjectType> _rp4vms;
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _rp4vms = _mrmp.managementobject().listmanagementobjects().managementobjects.Where(x => x.target_platform.enabled == true).ToList();
                }
                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);

                foreach (var _rp4vm in _rp4vms)
                {
                    while (lstThreads.Count(x => x.IsAlive) > Global.rp4vm_group_polling_concurrency)
                    {
                        Thread.Sleep(1000);
                    }

                    Thread _inventory_thread = new Thread(delegate ()
                    {
                        splashStart.Set();
                        try
                        {
                            RPEventPollerDo.PollerDo((MRPManagementobjectType)_rp4vm);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(string.Format("Error collecting RP4VMe information from {0} with error {1}", _rp4vm.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    splashStart.WaitOne();
                    Logger.log(String.Format("DT RP4VM Poller Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }
                sw.Stop();

                Logger.log(String.Format("Completed RP4VM Event collection for in {0} [next run at {1}]", TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

