﻿using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MRMPService.Utilities.SyncronizedList;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPAPI;
using System.Threading.Tasks;

namespace MRMPService.PerformanceCollection
{
    class WorkloadPerformanceThread
    {
        //create syncronized lists to work in the threaded environment

        public async void Start()
        {
            while (true)
            {
                try
                {
                    DateTime _next_performance_run = DateTime.UtcNow.AddHours(1);
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                    MRPWorkloadListType _workload_paged;
                    MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page = 1, deleted = false, enabled = true, perf_collection_enabled = true };
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
                                    await WorkloadPerformance.WorkloadPerformanceWindowsDo(workload);
                                    break;
                                case "UNIX":
                                    await WorkloadPerformance.WorkloadPerformanceUnixDo(workload);
                                    break;
                            }

                            await MRMPServiceBase._mrmp_api.workload().PeformanceUpdateStatus(workload, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting performance information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            await MRMPServiceBase._mrmp_api.workload().PeformanceUpdateStatus(workload, ex.Message, false);
                        }
                    });

                    sw.Stop();

                    Logger.log(String.Format("Completed performance collection for {0} workloads in {1} [next run at {2}]",
                        _all_workloads.Count(), TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_performance_run), Logger.Severity.Info);

                    //Wait for next run
                    while (_next_performance_run > DateTime.UtcNow)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Performance: {0}", ex.ToString()), Logger.Severity.Fatal);
                }
            }
        }
    }
}

