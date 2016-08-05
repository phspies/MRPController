using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MRMPService.MRMPAPI;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
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
                                    Thread dr_servers_dormant_create_protection_job_Thread = new Thread(() => DRSServersDormant.SetupProtectionJob(task));
                                    dr_servers_dormant_create_protection_job_Thread.Name = task.target_id;
                                    dr_servers_dormant_create_protection_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = dr_servers_dormant_create_protection_job_Thread, target_id = task.target_id });
                                    break;


                                //migrate
                                case "migrate_create_job":
                                    Thread migrate_create_job_Thread = new Thread(() => Migrate.SetupMigrateJob(task));
                                    migrate_create_job_Thread.Name = task.target_id;
                                    migrate_create_job_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = migrate_create_job_Thread, target_id = task.target_id });
                                    break;

                                //platform
                                case "discover_datacenters_method":
                                    Thread discover_datacenters_method_Thread = new Thread(() => DatacenterDiscovery.DatacenterDiscoveryDo(task));
                                    discover_datacenters_method_Thread.Name = task.target_id;
                                    discover_datacenters_method_Thread.Start();
                                    lstThreads.Add(new ThreadObject() { task = discover_datacenters_method_Thread, target_id = task.target_id });
                                    break;
                                case "discovery_method":
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
    }
    public class ThreadObject
    {
        public Thread task { get; set; }
        public String target_id { get; set; }
    }
}
