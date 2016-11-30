using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MRMPService.MRMPAPI;

namespace MRMPService.NetstatCollection
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


                MRPWorkloadListType _workload_paged;
                MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page =1, deleted=false, enabled=true, netstat_collection_enabled=true };
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _workload_paged = _api.workload().list_paged_filtered_brief(_filter);
                }
                _processed_workloads = (int)_workload_paged.pagination.total_entries;
                double _multiplyer = Math.Ceiling((double)_workload_paged.pagination.total_entries / 200.00);

                Logger.log(String.Format("Netstat: Staring netstat collection process with {0} threads", (_multiplyer * Global.os_netstat_concurrency)), Logger.Severity.Info);

                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);
                while (_workload_paged.pagination.page_size > 0)
                {
                    foreach (var workload in _workload_paged.workloads)
                    {
                        while (lstThreads.Count(x => x.IsAlive) > (Global.os_netstat_concurrency * _multiplyer))
                        {
                            Thread.Sleep(1000);
                        }

                        Thread _inventory_thread = new Thread(async delegate ()
                        {
                            splashStart.Set();
                            try
                            {
                                switch (workload.ostype.ToUpper())
                                {
                                    case "WINDOWS":
                                        await WorkloadNetstat.WorkloadNetstatWindowsDo(workload);
                                        break;
                                    case "UNIX":
                                        await WorkloadNetstat.WorkloadNetstatUnixDo(workload);
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