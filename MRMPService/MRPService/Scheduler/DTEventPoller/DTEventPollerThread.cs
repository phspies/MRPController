using MRMPService.MRMPService.Log;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.Modules.MRMPPortal.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace MRMPService.Scheduler.DTEventPollerCollection
{
    class DTEventPollerThread
    {
        public async void Start()
        {
            while (true)
            {
                try
                {
                    DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(MRMPServiceBase.dt_event_polling_interval);
                    Stopwatch sw = Stopwatch.StartNew();

                    Logger.log(String.Format("Staring Double-Take Event collection process with {0} threads", MRMPServiceBase.dt_event_polling_concurrency), Logger.Severity.Info);
                    MRPWorkloadListType _workload_paged;
                    MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { page = 1, deleted = false, enabled = true, dt_installed = true, dt_collection_enabled = true };

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
                    Parallel.ForEach(_all_workloads, new ParallelOptions() { MaxDegreeOfParallelism = MRMPServiceBase.dt_event_polling_concurrency }, workload =>
                    {
                        try
                        {
                           DTEventPollerDo.PollerDo(workload);
                            MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(workload, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting Double-Take Event information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(workload, ex.Message, false);
                        }
                    });

                    sw.Stop();

                    Logger.log(String.Format("Completed Double-Take Event collection for {0} workloads in {1} [next run at {2}]",
                       _all_workloads.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalSeconds), _next_poller_run), Logger.Severity.Info);

                    //Wait for next run
                    while (_next_poller_run > DateTime.UtcNow)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("DTEventPoller: {0}", ex.ToString()), Logger.Severity.Fatal);
                }
            }
        }
    }
}

