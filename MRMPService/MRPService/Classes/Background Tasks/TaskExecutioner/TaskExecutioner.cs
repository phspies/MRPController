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
                        if ((!lstThreads.Exists(x => x.target_id == task.target_id) && lstThreads.Where(x => x.task.IsAlive).Count() < Global.scheduler_concurrency))
                        {
                            MRPTaskType _current_task = new MRPTaskType();
                            switch (task.task_type)
                            {
                                //drs_servers_dormant
                                case "dr_servers_dormant_create_protection_job":
                                    _current_task = ClaimTask(task);
                                    Thread dr_servers_dormant_create_protection_job_Thread = new Thread(() => DRSServersDormant.SetupDormantJob(_current_task));
                                    dr_servers_dormant_create_protection_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_protection_job_Thread.Start();
                                    dr_servers_dormant_create_protection_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_protection_job_Thread, target_id = task.target_id });
                                    break;
                                case "dr_servers_dormant_create_firedrill_job":
                                    _current_task = ClaimTask(task);
                                    Thread dr_servers_dormant_create_firedrill_job_Thread = new Thread(() => DRSServersDormant.SetupDormantRecoveryJob(_current_task));
                                    dr_servers_dormant_create_firedrill_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_firedrill_job_Thread.Start();
                                    dr_servers_dormant_create_firedrill_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_firedrill_job_Thread, target_id = task.target_id });
                                    break;

                                //migrate
                                case "migrate_create_job":
                                    _current_task = ClaimTask(task);
                                    Thread migrate_create_job_Thread = new Thread(() => Migrate.SetupMigrateJob(_current_task));
                                    migrate_create_job_Thread.Name = task.target_id;
                                    migrate_create_job_Thread.Start();
                                    migrate_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_create_job_Thread, target_id = task.target_id });
                                    break;

                                //live
                                case "drs_servers_live_create_job":
                                    _current_task = ClaimTask(task);
                                    Thread drs_servers_live_create_job_Thread = new Thread(() => DRSServersLive.SetupLiveJob(_current_task));
                                    drs_servers_live_create_job_Thread.Name = task.target_id;
                                    drs_servers_live_create_job_Thread.Start();
                                    drs_servers_live_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_servers_live_create_job_Thread, target_id = task.target_id });
                                    break;

                                //vmware
                                case "drs_vmware_create_cg":
                                    _current_task = ClaimTask(task);
                                    Thread drs_vmware_create_cg_Thread = new Thread(() => DRSVMWare.SetupConsistencyGroup(_current_task));
                                    drs_vmware_create_cg_Thread.Name = task.target_id;
                                    drs_vmware_create_cg_Thread.Start();
                                    drs_vmware_create_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_vmware_create_cg_Thread, target_id = task.target_id });
                                    break;


                                //DT common tasks 
                                case "dt_stop_job":
                                    _current_task = ClaimTask(task);
                                    Thread dt_stop_job_Thread = new Thread(() => Common.StopDoubleTakeJob(_current_task));
                                    dt_stop_job_Thread.Name = task.target_id;
                                    dt_stop_job_Thread.Start();
                                    dt_stop_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_stop_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_pause_job":
                                    _current_task = ClaimTask(task);
                                    Thread dt_pause_job_Thread = new Thread(() => Common.PauseDoubleTakeJob(_current_task));
                                    dt_pause_job_Thread.Name = task.target_id;
                                    dt_pause_job_Thread.Start();
                                    dt_pause_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_pause_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_start_job":
                                    _current_task = ClaimTask(task);
                                    Thread dt_start_job_Thread = new Thread(() => Common.StartDoubleTakeJob(_current_task));
                                    dt_start_job_Thread.Name = task.target_id;
                                    dt_start_job_Thread.Start();
                                    dt_start_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_start_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_job":
                                    _current_task = ClaimTask(task);
                                    Thread migrate_failover_job_Thread = new Thread(() => Common.FailoverDoubleTakeJob(_current_task));
                                    migrate_failover_job_Thread.Name = task.target_id;
                                    migrate_failover_job_Thread.Start();
                                    migrate_failover_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_group":
                                    _current_task = ClaimTask(task);
                                    Thread migrate_failover_group_Thread = new Thread(() => Common.FailoverDoubleTakeGroup(_current_task));
                                    migrate_failover_group_Thread.Name = task.target_id;
                                    migrate_failover_group_Thread.Start();
                                    migrate_failover_group_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_group_Thread, target_id = task.target_id });
                                    break;

                                //deploy only tasks
                                case "deploy_workload":
                                    _current_task = ClaimTask(task);
                                    Thread deploy_workload_Thread = new Thread(() => Deploy.DeployWorkload(_current_task));
                                    deploy_workload_Thread.Name = task.target_id;
                                    deploy_workload_Thread.Start();
                                    deploy_workload_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = deploy_workload_Thread, target_id = task.target_id });
                                    break;

                                //platform
                                case "discover_datacenters_method":
                                    _current_task = ClaimTask(task);
                                    Thread discover_datacenters_method_Thread = new Thread(() => DatacenterDiscovery.DatacenterDiscoveryDo(_current_task));
                                    discover_datacenters_method_Thread.Name = task.target_id;
                                    discover_datacenters_method_Thread.Start();
                                    discover_datacenters_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discover_datacenters_method_Thread, target_id = task.target_id });
                                    break;
                                case "discovery_method":
                                    _current_task = ClaimTask(task);
                                    Thread discovery_method_Thread = new Thread(() => PlatformDiscovery.PlatformDiscoveryDo(_current_task));
                                    discovery_method_Thread.Name = task.target_id;
                                    discovery_method_Thread.Start();
                                    discovery_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discovery_method_Thread, target_id = task.target_id });
                                    break;

                                //workload
                                case "discover_workload":
                                    _current_task = ClaimTask(task);
                                    Thread discover_workload_Thread = new Thread(() => Workload.DiscoverWorkload(_current_task));
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

                lstThreads.RemoveAll(x => !x.task.IsAlive);
                Global.worker_queue_count = lstThreads.Count();

                Thread.Sleep(new TimeSpan(0, 0, Global.scheduler_interval));
            }
        }
        static private MRPTaskType ClaimTask(MRPTaskType _task)
        {
            MRPTaskType _task_details = new MRPTaskType();
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(_task.id, String.Format("Task claimed by {0}", System.Environment.MachineName), 1);
                try
                {
                    _task_details = _mrp_api.task().get(_task.id);
                    return _task_details;
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error retrieving task details from platform: {0}", ex.GetBaseException().Message), Logger.Severity.Fatal);
                }
                return _task_details;
            }
        }
    }
    public class ThreadObject
    {
        public Thread task { get; set; }
        public String target_id { get; set; }
    }
}
