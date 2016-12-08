using MRMPService.MRMPService.Log;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MRMPService.MRMPAPI;
using System.Threading.Tasks;

namespace MRMPService.Scheduler.NetstatCollection
{
    class WorkloadNetstatThread
    {
        public async void Start()
        {
            try
            {
                while (true)
                {
                    DateTime _next_netstat_run = DateTime.UtcNow.AddMinutes(MRMPServiceBase.os_netstat_interval);

                    MRPWorkloadListType _workload_paged;
                    MRPWorkloadFilterPagedType _filter = new MRPWorkloadFilterPagedType() { provisioned = true, page = 1, deleted = false, enabled = true, netstat_collection_enabled = true };
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
                    int _multiplyer = (_all_workloads.Count() > 100) ? (_all_workloads.Count()) / 100 : 1;

                    int _connurrency = (MRMPServiceBase.os_netstat_concurrency * _multiplyer);

                    Logger.log(String.Format("Netstat: Staring netstat collection process with {0} threads", _connurrency), Logger.Severity.Info);

                    Parallel.ForEach(_all_workloads, new ParallelOptions() { MaxDegreeOfParallelism = _connurrency }, async workload =>
                    {
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

                    //Wait for next run
                    while (_next_netstat_run > DateTime.UtcNow)
                    {
                        await Task.Delay(new TimeSpan(0, 0, 5));
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.log(String.Format("Netstat: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }

    }
}