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
                        if ((!lstThreads.Exists(x => x.target_id == task.target_id) && lstThreads.Where(x => x.task.IsAlive == true).Count() < Global.scheduler_concurrency))
                        {
                            switch (task.task_type)
                            {
                                //drs_servers_dormant
                                case "dr_servers_dormant_create_protection_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_protection_job_Thread = new Thread(() => DRSServersDormant.SetupDormantJob(task));
                                    dr_servers_dormant_create_protection_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_protection_job_Thread.Start();
                                    dr_servers_dormant_create_protection_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_protection_job_Thread, target_id = task.target_id });
                                    break;
                                case "dr_servers_dormant_create_firedrill_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_firedrill_job_Thread = new Thread(() => DRSServersDormant.SetupDormantRecoveryJob(task));
                                    dr_servers_dormant_create_firedrill_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_firedrill_job_Thread.Start();
                                    dr_servers_dormant_create_firedrill_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_firedrill_job_Thread, target_id = task.target_id });
                                    break;

                                //migrate
                                case "migrate_create_job":
                                    ClaimTask(task);
                                    Thread migrate_create_job_Thread = new Thread(() => Migrate.SetupMigrateJob(task));
                                    migrate_create_job_Thread.Name = task.target_id;
                                    migrate_create_job_Thread.Start();
                                    migrate_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_create_job_Thread, target_id = task.target_id });
                                    break;

                                //live
                                case "drs_servers_live_create_job":
                                    ClaimTask(task);
                                    Thread drs_servers_live_create_job_Thread = new Thread(() => DRSServersLive.SetupLiveJob(task));
                                    drs_servers_live_create_job_Thread.Name = task.target_id;
                                    drs_servers_live_create_job_Thread.Start();
                                    drs_servers_live_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_servers_live_create_job_Thread, target_id = task.target_id });
                                    break;

                                //vmware
                                case "drs_vmware_create_cg":
                                    ClaimTask(task);
                                    Thread drs_vmware_create_cg_Thread = new Thread(() => DRSVMWare.SetupConsistencyGroup(task));
                                    drs_vmware_create_cg_Thread.Name = task.target_id;
                                    drs_vmware_create_cg_Thread.Start();
                                    drs_vmware_create_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_vmware_create_cg_Thread, target_id = task.target_id });
                                    break;


                                //DT common tasks 
                                case "dt_stop_job":
                                    ClaimTask(task);
                                    Thread dt_stop_job_Thread = new Thread(() => Common.StopDoubleTakeJob(task));
                                    dt_stop_job_Thread.Name = task.target_id;
                                    dt_stop_job_Thread.Start();
                                    dt_stop_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_stop_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_pause_job":
                                    ClaimTask(task);
                                    Thread dt_pause_job_Thread = new Thread(() => Common.PauseDoubleTakeJob(task));
                                    dt_pause_job_Thread.Name = task.target_id;
                                    dt_pause_job_Thread.Start();
                                    dt_pause_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_pause_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_start_job":
                                    ClaimTask(task);
                                    Thread dt_start_job_Thread = new Thread(() => Common.StartDoubleTakeJob(task));
                                    dt_start_job_Thread.Name = task.target_id;
                                    dt_start_job_Thread.Start();
                                    dt_start_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_start_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_job":
                                    ClaimTask(task);
                                    Thread migrate_failover_job_Thread = new Thread(() => Common.FailoverDoubleTakeJob(task));
                                    migrate_failover_job_Thread.Name = task.target_id;
                                    migrate_failover_job_Thread.Start();
                                    migrate_failover_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_group":
                                    ClaimTask(task);
                                    Thread migrate_failover_group_Thread = new Thread(() => Common.FailoverDoubleTakeGroup(task));
                                    migrate_failover_group_Thread.Name = task.target_id;
                                    migrate_failover_group_Thread.Start();
                                    migrate_failover_group_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_group_Thread, target_id = task.target_id });
                                    break;

                                //deploy only tasks
                                case "deploy_workload":
                                    ClaimTask(task);
                                    Thread deploy_workload_Thread = new Thread(() => Deploy.DeployWorkload(task));
                                    deploy_workload_Thread.Name = task.target_id;
                                    deploy_workload_Thread.Start();
                                    deploy_workload_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = deploy_workload_Thread, target_id = task.target_id });
                                    break;

                                //platform
                                case "discover_datacenters_method":
                                    ClaimTask(task);
                                    Thread discover_datacenters_method_Thread = new Thread(() => DatacenterDiscovery.DatacenterDiscoveryDo(task));
                                    discover_datacenters_method_Thread.Name = task.target_id;
                                    discover_datacenters_method_Thread.Start();
                                    discover_datacenters_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discover_datacenters_method_Thread, target_id = task.target_id });
                                    break;
                                case "discovery_method":
                                    ClaimTask(task);
                                    Thread discovery_method_Thread = new Thread(() => PlatformDiscovery.PlatformDiscoveryDo(task));
                                    discovery_method_Thread.Name = task.target_id;
                                    discovery_method_Thread.Start();
                                    discovery_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discovery_method_Thread, target_id = task.target_id });
                                    break;

                                //workload
                                case "discover_workload":
                                    ClaimTask(task);
                                    Thread discover_workload_Thread = new Thread(() => Workload.DiscoverWorkload(task));
                                    discover_workload_Thread.Name = task.target_id;
                                    discover_workload_Thread.Start();
                                    discover_workload_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discover_workload_Thread, target_id = task.target_id });
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
