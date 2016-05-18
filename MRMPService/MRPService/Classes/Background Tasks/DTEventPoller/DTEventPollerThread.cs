using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;

namespace MRMPService.DTEventPollerCollection
{
    class DTEventPollerThread
    { 
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(Global.dt_job_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring Double-Take Event collection process with {0} threads", Global.os_performance_concurrency), Logger.Severity.Info);

                MRPWorkloadListType _dt_workloads;
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _dt_workloads = _mrmp.workload().list_dt_installed();
                }
                Parallel.ForEach(_dt_workloads.workloads,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.dt_job_polling_concurrency },
                    (workload) =>
                    {
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

                sw.Stop();

                Logger.log(String.Format("Completed Double-Take Event collection for {0} jobs in {1} [next run at {2}]",
                    _dt_workloads.workloads.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

