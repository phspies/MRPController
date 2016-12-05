using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using System.Linq;
using System.Collections.Generic;

namespace MRMPService.DTPollerCollection
{
    class DTJobPollerThread
    {
        public async void Start()
        {
            while (true)
            {
                DateTime _next_poller_run = DateTime.UtcNow.AddSeconds(MRMPServiceBase.dt_job_polling_interval);
                Stopwatch sw = Stopwatch.StartNew();

                Logger.log(String.Format("Staring Double-Take collection process with {0} threads", MRMPServiceBase.os_performance_concurrency), Logger.Severity.Info);

                List<MRPManagementobjectType> _jobs;

                MRManagementobjectFilterType _filter = new MRManagementobjectFilterType() { entitytype = 0 };
                _jobs = (await MRMPServiceBase._mrmp_api.managementobject().list_filtered(_filter)).managementobjects.Where(x => x.target_workload.dt_collection_enabled == true && x.target_workload.provisioned == true).GroupBy(i => i.moid).Select(group => group.First()).ToList();
                List<Thread> lstThreads = new List<Thread>();
                var splashStart = new ManualResetEvent(false);

                foreach (MRPManagementobjectType job in _jobs)
                {
                    while (lstThreads.Count(x => x.IsAlive) > MRMPServiceBase.os_inventory_concurrency)
                    {
                        Thread.Sleep(1000);
                    }

                    Thread _inventory_thread = new Thread(async delegate ()
                    {
                        splashStart.Set();
                        try
                        {
                            await DTJobPoller.PollerDo(job);
                            await MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(job.target_workload, "Success", true);
                        }
                        catch (Exception ex)
                        {
                            Logger.log(string.Format("Error collecting Double-Take information from {0} with error {1}", job.target_workload.hostname, ex.ToString()), Logger.Severity.Error);
                            await MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(job.target_workload, ex.Message, false);
                        }
                    });
                    lstThreads.Add(_inventory_thread);
                    _inventory_thread.Start();
                    splashStart.WaitOne();
                    Logger.log(String.Format("DT Job Poller Thread Count [active: {0}] [total: {1}] [complete {2}]", lstThreads.Count(x => x.IsAlive), lstThreads.Count(), lstThreads.Count(x => !x.IsAlive)), Logger.Severity.Info);
                }
                while (lstThreads.Any(x => x.IsAlive))
                {
                    await Task.Delay(new TimeSpan(0, 0, 5));
                }

                sw.Stop();

                Logger.log(String.Format("Completed Double-Take collection for {0} jobs in {1} [next run at {2}]",
                    _jobs.Count, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), _next_poller_run), Logger.Severity.Info);

                //Wait for next run
                while (_next_poller_run > DateTime.UtcNow)
                {
                    await Task.Delay(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}

