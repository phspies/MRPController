using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MRMPService.API.Classes
{
    class WorkloadNetstatThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_netstat_run = DateTime.Now.AddMinutes(Global.os_netstat_interval);
                Stopwatch sw = Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Netstat: Staring netstat collection process with {0} threads", Global.os_netstat_concurrency), Logger.Severity.Info);

                List<Workload> workloads;
                using (WorkloadSet workload_set = new WorkloadSet())
                {
                    workloads = workload_set.ModelRepository.Get(x => x.enabled == true && x.credential_id != null && x.iplist != null);
                }
                _processed_workloads = workloads.Count;
                Parallel.ForEach(workloads,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.os_netstat_concurrency },
                    (workload) =>
                    {
                        try
                        {
                            WorkloadNetstat.WorkloadNetstatDo(workload.id);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Netstat: Error collecting netstat information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                        }
                    });

                sw.Stop();

                Logger.log(String.Format("Netstat: Completed netstat collection for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_netstat_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_netstat_run > DateTime.Now)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}