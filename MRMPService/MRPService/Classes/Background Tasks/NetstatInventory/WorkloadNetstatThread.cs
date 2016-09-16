using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadNetstatThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_netstat_run = DateTime.UtcNow.AddMinutes(Global.os_netstat_interval);
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Netstat: Staring netstat collection process with {0} threads", Global.os_netstat_concurrency), Logger.Severity.Info);

                List<MRPWorkloadType> workloads;
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    workloads = _api.workload().listworkloads().workloads.Where(x => x.netstat_collection_enabled == true).ToList();
                }
                _processed_workloads = workloads.Count;

                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);
                foreach (var workload in workloads)
                {

                    while (lstThreads.Count(x => x.IsAlive) > Global.os_netstat_concurrency - 1)
                    {
                        Thread.Sleep(1000);
                    }

                    Thread _inventory_thread = new Thread(delegate ()
                    {
                        splashStart.Set();
                        try
                        {
                            switch (workload.ostype.ToUpper())
                            {
                                case "WINDOWS":
                                    WorkloadNetstat.WorkloadNetstatWindowsDo(workload);
                                    break;
                                case "UNIX":
                                    WorkloadNetstat.WorkloadNetstatUnixDo(workload);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Netstat: Error collecting netstat information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    splashStart.WaitOne();
                    Logger.log(String.Format("Netstat Collection Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }

                sw.Stop();

                Logger.log(String.Format("Netstat: Completed netstat collection for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_netstat_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_netstat_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}