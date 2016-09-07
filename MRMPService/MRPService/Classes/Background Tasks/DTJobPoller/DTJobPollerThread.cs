using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using System.Linq;
using System.Collections.Generic;

namespace MRMPService.DTPollerCollection
{
    class DTJobPollerThread
    {
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(Global.dt_job_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring Double-Take collection process with {0} threads", Global.os_performance_concurrency), Logger.Severity.Info);

                List<MRPManagementobjectType> _jobs;
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _jobs = _mrmp.managementobject().listmanagementobjects().managementobjects.Where(x => x.target_workload.dt_collection_enabled == true).GroupBy(i => i.moid).Select(group => group.First()).ToList();

                }
                Parallel.ForEach(_jobs,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.dt_job_polling_concurrency },
                    (Action<MRPManagementobjectType>)((job) =>
                    {
                        try
                        {
                            DTJobPoller.PollerDo((MRPManagementobjectType)job);
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().DoubleTakeUpdateStatus(job.target_workload, "Success", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.log(string.Format("Error collecting Double-Take information from {0} with error {1}", job.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                            using (MRMP_ApiClient _api = new MRMP_ApiClient())
                            {
                                _api.workload().DoubleTakeUpdateStatus(job.target_workload, ex.Message, false);
                            }
                        }
                    }));

                sw.Stop();

                Logger.log(String.Format("Completed Double-Take collection for {0} jobs in {1} [next run at {2}]",
                    _jobs.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

