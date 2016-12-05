using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadInventoryThread
    {
        MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();
        public async void Start()
        {
            try
            {

                while (true)
                {
                    DateTime _next_inventory_run = DateTime.UtcNow.AddMinutes(MRMPServiceBase.os_inventory_interval);
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                    Logger.log(String.Format("Staring operating system inventory process with {0} threads", MRMPServiceBase.os_inventory_concurrency), Logger.Severity.Info);

                    MRPWorkloadListType _workload_paged;
                    MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page = 1, deleted = false, enabled = true, os_collection_enabled = true };
                    _workload_paged = await MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(_filter);
                    List<MRPWorkloadType> _all_workloads = new List<MRPWorkloadType>();
                    while (_workload_paged.pagination.page_size > 0)
                    {
                        _all_workloads.AddRange(_workload_paged.workloads);
                        if (_workload_paged.pagination.next_page > 0)
                        {
                            _filter.page = _workload_paged.pagination.next_page;
                            _workload_paged = await MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(_filter);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Parallel.ForEach(_all_workloads, new ParallelOptions() { MaxDegreeOfParallelism = MRMPServiceBase.os_inventory_concurrency }, async workload =>
                    {
                        try
                        {
                            switch (workload.ostype.ToUpper())
                            {
                                case "WINDOWS":
                                    await WorkloadInventory.WorkloadInventoryWindowsDo(workload);
                                    break;
                                case "UNIX":
                                    await WorkloadInventory.WorkloadInventoryLinuxDo(workload);
                                    break;
                            }
                            await MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(workload, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting inventory information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            await MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(workload, ex.Message, false);
                        }
                    });

                    //Wait for next run
                    while (_next_inventory_run > DateTime.UtcNow)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Inventory: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }
    }
}
