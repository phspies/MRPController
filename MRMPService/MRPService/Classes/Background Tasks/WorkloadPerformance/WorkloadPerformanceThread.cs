using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MRMPService.Utilities.SyncronizedList;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPAPI;

namespace MRMPService.PerformanceCollection
{
    class WorkloadPerformanceThread
    {
        //create syncronized lists to work in the threaded environment

        public void Start()
        {
            while (true)
            {
                DateTime _next_performance_run = DateTime.UtcNow.AddHours(1);
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int _processed_workloads = 0;


                MRPWorkloadListType _workload_paged;
                MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page = 1, deleted = false, enabled = true, perf_collection_enabled = true };
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _workload_paged = _api.workload().list_paged_filtered_brief(_filter);
                }
                _processed_workloads = (int)_workload_paged.pagination.total_entries;
                double _multiplyer = Math.Ceiling((double)_workload_paged.pagination.total_entries / 100.00);

                Logger.log(String.Format("Staring performance collection process with {0} threads", (MRMPServiceBase.os_performance_concurrency * _multiplyer)), Logger.Severity.Info);



                List<Thread> lstThreads = new List<Thread>();
                while (_workload_paged.pagination.page_size > 0)
                {
                    foreach (var workload in _workload_paged.workloads)
                    {
                        while (lstThreads.Count(x => x.IsAlive) > (MRMPServiceBase.os_performance_concurrency * _multiplyer))
                        {
                            Thread.Sleep(1000);
                        }
                        var splashStart = new ManualResetEvent(false);
                        Thread _inventory_thread = new Thread(delegate ()
                        {
                            splashStart.Set();
                            try
                            {
                                switch (workload.ostype.ToUpper())
                                {
                                    case "WINDOWS":
                                        WorkloadPerformance.WorkloadPerformanceWindowsDo(workload);
                                        break;
                                    case "UNIX":
                                        WorkloadPerformance.WorkloadPerformanceUnixDo(workload);
                                        break;
                                }
                                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                                {
                                    _api.workload().PeformanceUpdateStatus(workload, "Success", true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting performance information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                                {
                                    _api.workload().PeformanceUpdateStatus(workload, ex.Message, false);
                                }
                            }
                        });
                        lstThreads.Add(_inventory_thread);
                        _inventory_thread.Start();
                        splashStart.WaitOne();

                        Logger.log(String.Format("Workload Performance Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                    }
                    if (_workload_paged.pagination.next_page > 0)
                    {
                        _filter.page = _workload_paged.pagination.next_page;
                        using (MRMP_ApiClient _api = new MRMP_ApiClient())
                        {
                            _workload_paged = _api.workload().list_paged_filtered_brief(_filter);
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

                Logger.log(String.Format("Completed performance collection for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_performance_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_performance_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

