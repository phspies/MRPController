﻿using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MRPService.API.Classes
{
    class WorkloadNetstatThread
    {
        ApiClient _cloud_movey = new ApiClient();
        public void Start()
        {

            while (true)
            {
                DateTime _next_netstat_run = DateTime.Now.AddMinutes(Global.os_netstat_interval);
                Stopwatch sw = Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Staring operating system netstat process with {0} threads", Global.os_netstat_concurrency), Logger.Severity.Info);

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
                            MRPWorkloadType _mrp_workload = _cloud_movey.workload().getworkload(workload.id);
                            WorkloadNetstat.WorkloadNetstatDo(_mrp_workload.id);
                            Workloads_Update.InventoryUpdateStatus(workload.id, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting netstat information from {0} with error {1}", workload.hostname, ex.Message), Logger.Severity.Error);
                            Workloads_Update.InventoryUpdateStatus(workload.id, ex.Message, false);
                        }
                    });

                sw.Stop();

                Logger.log(String.Format("Completed operating system netstat for {0} workloads in {1} [next run at {2}]",
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