using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.TaskExecutioner.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.TaskExecutioner
{
    class TaskWorker
    {
        public async void Start()
        {
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();

            while (true)
            {
                MRPTaskListType tasklist = null;

                try
                {
                    tasklist = MRMPServiceBase._mrmp_api.task().tasks();
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error retrieving task details from platform: {0}", ex.GetBaseException().Message), Logger.Severity.Fatal);
                }

                if (tasklist != null)
                {
                    foreach (MRPTaskType task in tasklist.tasks)
                    {
                        //make sure new target task does not have an active task busy
                        if ((!lstThreads.Exists(x => x.target_id == task.target_id) && lstThreads.Where(x => x.task.IsAlive).Count() < MRMPServiceBase.scheduler_concurrency))
                        {
                            switch (task.task_type)
                            {
                                //MCP
                                case "drs_mcp_create_cg":
                                    ClaimTask(task);
                                    Thread create_mcp_cg_Thread = new Thread(() => DRSMCP.DRSMCP.SetupCG(task));
                                    create_mcp_cg_Thread.Name = task.target_id;
                                    create_mcp_cg_Thread.Start();
                                    create_mcp_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = create_mcp_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_firedrill_cg":
                                    ClaimTask(task);
                                    Thread drs_mcp_firedrill_cg_Thread = new Thread(() => DRSMCP.DRSMCP.PreviewCG(task));
                                    drs_mcp_firedrill_cg_Thread.Name = task.target_id;
                                    drs_mcp_firedrill_cg_Thread.Start();
                                    drs_mcp_firedrill_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_firedrill_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_stop_firedrill_cg":
                                    ClaimTask(task);
                                    Thread drs_mcp_stop_firedrill_cg_Thread = new Thread(() => DRSMCP.DRSMCP.StopPreviewCG(task));
                                    drs_mcp_stop_firedrill_cg_Thread.Name = task.target_id;
                                    drs_mcp_stop_firedrill_cg_Thread.Start();
                                    drs_mcp_stop_firedrill_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_stop_firedrill_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_setup_failover_cg":
                                    ClaimTask(task);
                                    Thread drs_mcp_setup_failover_cg_Thread = new Thread(() => DRSMCP.DRSMCP.SetupFailoverCG(task));
                                    drs_mcp_setup_failover_cg_Thread.Name = task.target_id;
                                    drs_mcp_setup_failover_cg_Thread.Start();
                                    drs_mcp_setup_failover_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_setup_failover_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_stop_failover_cg":
                                    ClaimTask(task);
                                    Thread drs_mcp_stop_failover_cg_Thread = new Thread(() => DRSMCP.DRSMCP.StopPreviewCG(task));
                                    drs_mcp_stop_failover_cg_Thread.Name = task.target_id;
                                    drs_mcp_stop_failover_cg_Thread.Start();
                                    drs_mcp_stop_failover_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_stop_failover_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_failover_cg":
                                    ClaimTask(task);
                                    Thread drs_mcp_failover_cg_Thread = new Thread(() => DRSMCP.DRSMCP.FailoverCG(task));
                                    drs_mcp_failover_cg_Thread.Name = task.target_id;
                                    drs_mcp_failover_cg_Thread.Start();
                                    drs_mcp_failover_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_failover_cg_Thread, target_id = task.target_id });
                                    break;
                                case "drs_mcp_apply_meta":
                                    ClaimTask(task);
                                    Thread drs_mcp_apply_meta_Thread = new Thread(() => DRSMCP.DRSMCP.ApplyMetaInformation(task));
                                    drs_mcp_apply_meta_Thread.Name = task.target_id;
                                    drs_mcp_apply_meta_Thread.Start();
                                    drs_mcp_apply_meta_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_mcp_apply_meta_Thread, target_id = task.target_id });
                                    break;

                                //drs_servers_dormant
                                case "drs_servers_dormant_create_protection_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_protection_job_Thread = new Thread(() => DRSServersDormant.DRSServersDormant.SetupDormantJob(task));
                                    dr_servers_dormant_create_protection_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_protection_job_Thread.Start();
                                    dr_servers_dormant_create_protection_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_protection_job_Thread, target_id = task.target_id });
                                    break;

                                case "drs_servers_dormant_create_firedrill_job":
                                    ClaimTask(task);
                                    Thread dr_servers_dormant_create_firedrill_job_Thread = new Thread(() => DRSServersDormant.DRSServersDormant.SetupDormantRecoveryJob(task));
                                    dr_servers_dormant_create_firedrill_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_firedrill_job_Thread.Start();
                                    dr_servers_dormant_create_firedrill_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_firedrill_job_Thread, target_id = task.target_id });
                                    break;

                                case "drs_servers_dormant_create_recovery_job":
                                    ClaimTask(task);
                                    Thread drs_servers_dormant_create_recovery_job_Thread = new Thread(() => DRSServersDormant.DRSServersDormant.SetupDormantRecoveryJob(task));
                                    drs_servers_dormant_create_recovery_job_Thread.Name = task.target_id;
                                    drs_servers_dormant_create_recovery_job_Thread.Start();
                                    drs_servers_dormant_create_recovery_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_servers_dormant_create_recovery_job_Thread, target_id = task.target_id });
                                    break;
                                //migrate
                                case "migrate_create_job":
                                    ClaimTask(task);
                                    Thread migrate_create_job_Thread = new Thread(() => Migrate.Migrate.SetupMigrateJob(task));
                                    migrate_create_job_Thread.Name = task.target_id;
                                    migrate_create_job_Thread.Start();
                                    migrate_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_create_job_Thread, target_id = task.target_id });
                                    break;

                                //live
                                case "drs_servers_live_create_workload":
                                    ClaimTask(task);
                                    Thread drs_servers_live_create_job_Thread = new Thread(() => DRSServersLive.DRSServersLive.SetupLiveJob(task));
                                    drs_servers_live_create_job_Thread.Name = task.target_id;
                                    drs_servers_live_create_job_Thread.Start();
                                    drs_servers_live_create_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_servers_live_create_job_Thread, target_id = task.target_id });
                                    break;

                                //vmware
                                case "drs_vmware_create_cg":
                                    ClaimTask(task);
                                    Thread drs_vmware_create_cg_Thread = new Thread(() => DRSVMWare.DRSVMWare.SetupConsistencyGroup(task));
                                    drs_vmware_create_cg_Thread.Name = task.target_id;
                                    drs_vmware_create_cg_Thread.Start();
                                    drs_vmware_create_cg_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = drs_vmware_create_cg_Thread, target_id = task.target_id });
                                    break;


                                //DT common tasks 
                                case "dt_stop_job":
                                    ClaimTask(task);
                                    Thread dt_stop_job_Thread = new Thread(() => Common.Common.StopDoubleTakeJob(task));
                                    dt_stop_job_Thread.Name = task.target_id;
                                    dt_stop_job_Thread.Start();
                                    dt_stop_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_stop_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_pause_job":
                                    ClaimTask(task);
                                    Thread dt_pause_job_Thread = new Thread(() => Common.Common.PauseDoubleTakeJob(task));
                                    dt_pause_job_Thread.Name = task.target_id;
                                    dt_pause_job_Thread.Start();
                                    dt_pause_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_pause_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_start_job":
                                    ClaimTask(task);
                                    Thread dt_start_job_Thread = new Thread(() => Common.Common.StartDoubleTakeJob(task));
                                    dt_start_job_Thread.Name = task.target_id;
                                    dt_start_job_Thread.Start();
                                    dt_start_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = dt_start_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_job":
                                    ClaimTask(task);
                                    Thread migrate_failover_job_Thread = new Thread(() => Common.Common.FailoverDoubleTakeJob(task));
                                    migrate_failover_job_Thread.Name = task.target_id;
                                    migrate_failover_job_Thread.Start();
                                    migrate_failover_job_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_job_Thread, target_id = task.target_id });
                                    break;
                                case "dt_failover_group":
                                    ClaimTask(task);
                                    Thread migrate_failover_group_Thread = new Thread(() => Common.Common.FailoverDoubleTakeGroup(task));
                                    migrate_failover_group_Thread.Name = task.target_id;
                                    migrate_failover_group_Thread.Start();
                                    migrate_failover_group_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = migrate_failover_group_Thread, target_id = task.target_id });
                                    break;

                                //deploy only tasks
                                case "deploy_workload":
                                    ClaimTask(task);
                                    Thread deploy_workload_Thread = new Thread(() => Deploy.Deploy.DeployWorkload(task));
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
                                    discover_datacenters_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discover_datacenters_method_Thread, target_id = task.target_id });
                                    discover_datacenters_method_Thread.Start();
                                    break;
                                case "discovery_method":
                                    ClaimTask(task);
                                    Thread discovery_method_Thread = new Thread(() => PlatformDiscovery.PlatformDiscoveryDo(task));
                                    discovery_method_Thread.Name = task.target_id;
                                    discovery_method_Thread.Priority = ThreadPriority.Highest;
                                    lstThreads.Add(new ThreadObject() { task = discovery_method_Thread, target_id = task.target_id });
                                    discovery_method_Thread.Start();

                                    break;

                                //workload
                                case "discover_workload":
                                    ClaimTask(task);
                                    Thread discover_workload_Thread = new Thread(() => Workload.Workload.DiscoverWorkload(task));
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
                MRMPServiceBase.worker_queue_count = lstThreads.Count();

                Thread.Sleep(new TimeSpan(0, 0, MRMPServiceBase.scheduler_interval));
            }
        }
        static private void ClaimTask(MRPTaskType _task)
        {
            MRMPServiceBase._mrmp_api.task().progress(_task.id, String.Format("Task claimed by {0}", System.Environment.MachineName), 1);
        }
    }
    public class ThreadObject
    {
        public Thread task { get; set; }
        public String target_id { get; set; }
    }
}
