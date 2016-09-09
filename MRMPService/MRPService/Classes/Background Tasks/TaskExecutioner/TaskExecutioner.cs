using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MRMPService.MRMPAPI;
using MRMPService.Tasks.DiscoveryPlatform;
using MRMPService.PortalTasks;

namespace MRMPService.TaskExecutioner
{
    class TaskWorker
    {
        public void Start()
        {
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();

            while (true)
            {
                MRPTaskListType tasklist = null;
                using (MRMP_ApiClient MRP = new MRMP_ApiClient())
                {
                    tasklist = MRP.task().tasks();
                }

                if (tasklist != null)
                {
                    foreach (MRPTaskType task in tasklist.tasks)
                    {
                        //make sure new target task does not have an active task busy
                        if ((lstThreads.FindAll(x => x.target_id == task.target_id).Count() == 0 && lstThreads.Count < Global.scheduler_concurrency) || task.hidden == true)
                        {
                            switch (task.task_type)
                            {
                                //drs_servers_dormant
                                case "dr_servers_dormant_create_protection_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_protection_job_Thread = new Thread(() => DRSServersDormant.SetupDormantJob(task));
                                    dr_servers_dormant_create_protection_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_protection_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_protection_job_Thread, target_id = task.target_id });
                                    break;
                                case "dr_servers_dormant_create_firedrill_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_firedrill_job_Thread = new Thread(() => DRSServersDormant.SetupDormantRecoveryJob(task));
                                    dr_servers_dormant_create_firedrill_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_firedrill_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_firedrill_job_Thread, target_id = task.target_id });
                                    break;

                                //migrate
                                case "migrate_create_job":
                                    ClaimTask(task);
                                    Thread migrate_create_job_Thread = new Thread(() => Migrate.SetupMigrateJob(task));
                                    migrate_create_job_Thread.Name = task.target_id;
                                    migrate_create_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = migrate_create_job_Thread, target_id = task.target_id });
                                    break;
                                case "migrate_failover_job":
                                    ClaimTask(task);
                                    Thread migrate_failover_job_Thread = new Thread(() => Migrate.FailoverMigrateJob(task));
                                    migrate_failover_job_Thread.Name = task.target_id;
                                    migrate_failover_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_job_Thread, target_id = task.target_id });
                                    break;
                                case "migrate_failover_group":
                                    ClaimTask(task);
                                    Thread migrate_failover_group_Thread = new Thread(() => Migrate.FailoverMigrateGroup(task));
                                    migrate_failover_group_Thread.Name = task.target_id;
                                    migrate_failover_group_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_group_Thread, target_id = task.target_id });
                                    break;


                                //DT common tasks 
                                case "dt_stop_job":
                                    ClaimTask(task);
                                    Thread dt_stop_job_Thread = new Thread(() => Common.StopJob(task));
                                    dt_stop_job_Thread.Name = task.target_id;
                                    dt_stop_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dt_stop_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_pause_job":
                                    ClaimTask(task);
                                    Thread dt_pause_job_Thread = new Thread(() => Common.PauseJob(task));
                                    dt_pause_job_Thread.Name = task.target_id;
                                    dt_pause_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dt_pause_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_start_job":
                                    ClaimTask(task);
                                    Thread dt_start_job_Thread = new Thread(() => Common.StartJob(task));
                                    dt_start_job_Thread.Name = task.target_id;
                                    dt_start_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dt_start_job_Thread, target_id = task.target_id });
                                    break;



                                //platform
                                case "discover_datacenters_method":
                                    ClaimTask(task);
                                    Thread discover_datacenters_method_Thread = new Thread(() => DatacenterDiscovery.DatacenterDiscoveryDo(task));
                                    discover_datacenters_method_Thread.Name = task.target_id;
                                    discover_datacenters_method_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = discover_datacenters_method_Thread, target_id = task.target_id });
                                    break;
                                case "discovery_method":
                                    ClaimTask(task);
                                    Thread discovery_method_Thread = new Thread(() => PlatformDiscovery.PlatformDiscoveryDo(task));
                                    discovery_method_Thread.Name = task.target_id;
                                    discovery_method_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = discovery_method_Thread, target_id = task.target_id });
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Logger.log("Agent not associated to organization!", Logger.Severity.Warn);
                }

                lstThreads.RemoveAll(x => x.task.ThreadState != ThreadState.Running);
                Global.worker_queue_count = lstThreads.Count();

                Thread.Sleep(new TimeSpan(0, 0, Global.scheduler_interval));
            }
        }
        static private void ClaimTask(MRPTaskType _task)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(_task.id, String.Format("Task claimed by {0}", System.Environment.MachineName), 1);
            }
        }
    }
    public class ThreadObject
    {
        public Thread task { get; set; }
        public String target_id { get; set; }
    }
}
