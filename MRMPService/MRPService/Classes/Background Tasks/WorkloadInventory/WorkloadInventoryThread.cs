using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
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

                MRPWorkloadListType _workload_paged;
                MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned=true, page = 1, deleted = false, enabled = true, os_collection_enabled = true };
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _workload_paged = _api.workload().list_paged_filtered(_filter);
                }
                int _processed_workloads = (int)_workload_paged.pagination.total_entries;

                List<Thread> lstThreads = new List<Thread>();
                while (_workload_paged.pagination.page_size > 0)
                {
                    foreach (var workload in _workload_paged.workloads)
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
                    if (_workload_paged.pagination.next_page > 0)
                    {
                        _filter.page = _workload_paged.pagination.next_page;
                        using (MRMP_ApiClient _api = new MRMP_ApiClient())
                        {
                            _workload_paged = _api.workload().list_paged_filtered(_filter);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                while (lstThreads.Any(x => x.IsAlive))
                {

                }

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
