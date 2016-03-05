using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MRPService.Utilities;
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

                                WorkloadInventory.UpdateWorkload(_mrp_workload.id);
                                workload_set.UpdateStatus(workload.id, "Success", 0);
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error connecting to WMI {0} with error {1}", workload.hostname, ex.Message), Logger.Severity.Error);
                                workload_set.UpdateStatus(workload.id, ex.Message, 1);
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