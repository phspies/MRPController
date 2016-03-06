using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MRPService.API.Classes
{
    class WorkloadInventoryThread
    {
        ApiClient _cloud_movey = new ApiClient();
        public void Start()
        {

            while (true)
            {
                DateTime _next_run = DateTime.Now.AddMinutes(Global.os_inventory_interval);
                Stopwatch sw = Stopwatch.StartNew();
                int _updated_workloads = 0;

                Logger.log(String.Format("Staring operating system inventory process with {0} threads", Global.os_inventory_concurrency), Logger.Severity.Info);

                using (WorkloadSet workload_set = new WorkloadSet())
                {
                    List<Workload> workloads = workload_set.ModelRepository.Get(x => x.enabled == true);
                    Parallel.ForEach(workloads,
                        new ParallelOptions { MaxDegreeOfParallelism = Global.os_inventory_concurrency },
                        (workload) => {

                            try
                            {
                                MRPWorkloadType _mrp_workload = _cloud_movey.workload().getworkload(workload.id);
                                WorkloadInventory.WorkloadInventoryDo(_mrp_workload.id);
                                workload_set.InventoryUpdateStatus(workload.id, "Success", true);
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.Message), Logger.Severity.Error);
                                workload_set.InventoryUpdateStatus(workload.id, ex.Message, false);
                            }
                            _updated_workloads += 1;

                        });
                }
                sw.Stop();

                Logger.log(String.Format("Completed operating system inventory. [updated workloads.{0}] = Total Execute Time: {1}",
                    _updated_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)), Logger.Severity.Info);

                //Wait for next run
                while(_next_run < DateTime.Now)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}