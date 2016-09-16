using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MRMPService.TaskExecutioner;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadInventoryThread
    {
        MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();
        public void Start()
        {

            while (true)
            {
                DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(Global.os_inventory_interval);
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                Logger.log(String.Format("Staring operating system inventory process with {0} threads", Global.os_inventory_concurrency), Logger.Severity.Info);

                List<MRPWorkloadType> workloads;
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    workloads = _api.workload().listworkloads().workloads.Where(x => x.os_collection_enabled == true).ToList();
                }
                List<Thread> lstThreads = new List<Thread>();
                foreach (var workload in workloads)
                {

                    while (lstThreads.Count(x => x.IsAlive) > Global.os_inventory_concurrency - 1)
                    {
                        Thread.Sleep(1000);
                    }

                    Thread _inventory_thread = new Thread(delegate ()
                    {
                        try
                        {
                            switch (workload.ostype.ToUpper())
                            {
                                case "WINDOWS":
                                    WorkloadInventory.WorkloadInventoryWindowsDo(workload);
                                    break;
                                case "UNIX":
                                    WorkloadInventory.WorkloadInventoryLinuxDo(workload);
                                    break;
                            }

                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, "Success", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().InventoryUpdateStatus(workload, ex.Message, false);
                            }
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    Logger.log(String.Format("Workload Inventory Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }

                sw.Stop();

                Logger.log(String.Format("Completed operating system inventory for {0} workloads in {1} [next run at {2}]",
                    workloads.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_inventory_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_inventory_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}
