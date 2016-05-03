using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.API;
using MRMPService.API.Types.API;

namespace MRMPService.DTPollerCollection
{
    class DTPollerThread
    { 
        public void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(Global.job_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring Double-Take collection process with {0} threads", Global.os_performance_concurrency), Logger.Severity.Info);

                MRPJobListType _jobs;
                using (MRP_ApiClient _mrmp = new MRP_ApiClient())
                {
                    _jobs = _mrmp.job().listjobs();

                }
                Parallel.ForEach(_jobs.jobs,
                    new ParallelOptions { MaxDegreeOfParallelism = Global.job_polling_concurrency },
                    (job) =>
                    {
                        try
                        {
                            DTPoller.PollerDo(job);
                            Workloads_Update.DoubleTakeUpdateStatus(job.target_workload_id, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Error collecting Double-Take information from {0} with error {1}", job.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                            Workloads_Update.DoubleTakeUpdateStatus(job.target_workload_id, ex.Message, false);
                        }
                    });

                sw.Stop();

                Logger.log(String.Format("Completed Double-Take collection for {0} jobs in {1} [next run at {2}]",
                    _jobs.jobs.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

