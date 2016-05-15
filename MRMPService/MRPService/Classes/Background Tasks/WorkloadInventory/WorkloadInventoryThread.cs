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
    class WorkloadInventoryThread
    {
        MRP_ApiClient _cloud_movey = new MRP_ApiClient();
        public void Start()
        {

            while (true)
            {
                DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(Global.os_inventory_interval);
                Stopwatch sw = Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Staring operating system inventory process with {0} threads", Global.os_inventory_concurrency), Logger.Severity.Info);

                List<MRPWorkloadType> workloads;
                using (MRP_ApiClient _api = new MRP_ApiClient())
                {
                    workloads = _api.workload().listworkloads().workloads;
                }

                _processed_workloads = workloads.Count;
                Parallel.ForEach(workloads,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.os_inventory_concurrency },
                    (workload) =>
                    {
                        try
                        {
                            (new WorkloadInventory()).WorkloadInventoryDo(workload);
                            using (MRP_ApiClient _api = new MRP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, "Success", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            using (MRP_ApiClient _api = new MRP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, ex.Message, false);
                            }
                        }
                    });

                sw.Stop();

                Logger.log(String.Format("Completed operating system inventory for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_inventory_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_inventory_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}