using MRMPService.MRMPService.Log;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadInventoryThread
    {
        public async void Start()
        {
            while (true)
            {
                try
                {
                    DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(MRMPServiceBase.os_inventory_interval);
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                    MRPWorkloadListType _workload_paged;
                    MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page = 1, deleted = false, enabled = true, os_collection_enabled = true };
                    _workload_paged = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(_filter);
                    List<MRPWorkloadType> _all_workloads = new List<MRPWorkloadType>();
                    while (_workload_paged.pagination.page_size > 0)
                    {
                        _all_workloads.AddRange(_workload_paged.workloads);
                        if (_workload_paged.pagination.next_page > 0)
                        {
                            _filter.page = _workload_paged.pagination.next_page;
                            _workload_paged = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(_filter);
                        }
                        else
                        {
                            break;
                        }
                    }
                    int _multiplyer = (_all_workloads.Count() > 75) ? (_all_workloads.Count()) / 75 : 1;

                    int _concurrency = (MRMPServiceBase.os_inventory_concurrency * _multiplyer);

                    Logger.log(String.Format("Inventory: Starting inventory collection process with {0} threads", _concurrency), Logger.Severity.Debug);
                    List<Thread> lstThreads = new List<Thread>();
                    foreach (var workload in _all_workloads)
                    {
                        while (lstThreads.Count(x => x.IsAlive) >= _concurrency)
                        {
                            await Task.Delay(500);
                        }

                        Thread _inventory_thread = new Thread(() =>
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
                                MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(workload, "Success", true);
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.GetBaseException().Message), Logger.Severity.Error);
                                MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(workload, ex.GetBaseException().Message, false);
                            }
                        });
                        _inventory_thread.Name = workload.hostname;
                        lstThreads.Add(_inventory_thread);
                        _inventory_thread.Start();
                        Logger.log(String.Format("Workload Inventory Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                    }
                    Logger.log(String.Format("Completed inventory collection for {0} workloads in {1} [next run at {2}]", _all_workloads.Count(), TimeSpan.FromMilliseconds(sw.Elapsed.TotalSeconds), _next_inventory_run), Logger.Severity.Info);
                    //Wait for next run
                    while (_next_inventory_run > DateTime.UtcNow)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Inventory: {0}", ex.ToString()), Logger.Severity.Fatal);
                }
            }
        }

    }
}
