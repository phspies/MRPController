using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MRMPService.MRMPAPI.Classes
{
    class WorkloadNetstatThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_netstat_run = DateTime.UtcNow.AddMinutes(Global.os_netstat_interval);
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                int _processed_workloads = 0;

                Logger.log(String.Format("Netstat: Staring netstat collection process with {0} threads", Global.os_netstat_concurrency), Logger.Severity.Info);

                MRPWorkloadListType _workload_paged;
                MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { page=1, deleted=false, enabled=true, netstat_collection_enabled=true };
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _workload_paged = _api.workload().list_paged_filtered_brief(_filter);
                }
                _processed_workloads = (int)_workload_paged.pagination.total_entries;

                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);
                while (_workload_paged.pagination.page_size > 0)
                {
                    foreach (var workload in _workload_paged.workloads)
                    {
                        while (lstThreads.Count(x => x.IsAlive) > Global.os_netstat_concurrency - 1)
                        {
                            Thread.Sleep(1000);
                        }

                        Thread _inventory_thread = new Thread(delegate ()
                        {
                            splashStart.Set();
                            try
                            {
                                switch (workload.ostype.ToUpper())
                                {
                                    case "WINDOWS":
                                        WorkloadNetstat.WorkloadNetstatWindowsDo(workload);
                                        break;
                                    case "UNIX":
                                        WorkloadNetstat.WorkloadNetstatUnixDo(workload);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Netstat: Error collecting netstat information from {0} with error {1}", workload.hostname, ex.ToString()), Logger.Severity.Error);
                            }
                        });
                        lstThreads.Add(_inventory_thread);
                        _inventory_thread.Start();
                        splashStart.WaitOne();
                        Logger.log(String.Format("Netstat Collection Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
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

                Logger.log(String.Format("Netstat: Completed netstat collection for {0} workloads in {1} [next run at {2}]",
                    _processed_workloads, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_netstat_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_netstat_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }

    }
}