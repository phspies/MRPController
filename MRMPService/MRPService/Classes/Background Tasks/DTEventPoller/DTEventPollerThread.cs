using MRMPService.MRMPService.Log;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace MRMPService.DTEventPollerCollection
{
    class DTEventPollerThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(Global.dt_event_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring Double-Take Event collection process with {0} threads", Global.dt_event_polling_concurrency), Logger.Severity.Info);
                MRPWorkloadListType _workload_paged;
                MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { page = 1, deleted = false, enabled = true, dt_installed = true, dt_collection_enabled = true };
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _workload_paged = _api.workload().list_paged_filtered_brief(_filter);
                }
                int _processed_workloads = (int)_workload_paged.pagination.total_entries;
                double _multiplyer = Math.Ceiling((double)_workload_paged.pagination.total_entries / 200.00);


                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);
                while (_workload_paged.pagination.page_size > 0)
                {
                    foreach (var workload in _workload_paged.workloads)
                    {

                        while (lstThreads.Count(x => x.IsAlive) > (Global.dt_job_polling_concurrency * _multiplyer))
                        {
                            Thread.Sleep(1000);
                        }

                        Thread _inventory_thread = new Thread(delegate ()
                        {
                            splashStart.Set();
                            try
                            {
                                DTEventPollerDo.PollerDo(workload);
                                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                                {
                                    _api.workload().DoubleTakeUpdateStatus(workload, "Success", true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting Double-Take Event information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                                {
                                    _api.workload().DoubleTakeUpdateStatus(workload, ex.Message, false);
                                }
                            }
                        });
                        lstThreads.Add(_inventory_thread);
                        _inventory_thread.Start();
                        splashStart.WaitOne();
                        Logger.log(String.Format("DT Event Poller Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
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

                Logger.log(String.Format("Completed Double-Take Event collection for {0} workloads in {1} [next run at {2}]",
                   _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

