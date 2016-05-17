using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MRMPService.API;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
using MRMPService.Tasks.DiscoveryPlatform;

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
                using (MRP_ApiClient MRP = new MRP_ApiClient())
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
                            switch (task.target_type)
                            {
                                case "dt":
                                    {
                                        switch (task.task_type)
                                        {
                                            case "deploy_method":
                                                {
                                                    Thread newThread = new Thread(() => Deploy.DeployDoubleTake(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "sync_ha_method":
                                                {
                                                    Thread newThread = new Thread(() => Availability.CreateJob(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "move_setup_method":
                                                {
                                                    Thread newThread = new Thread(() => Migration.CreateServerMigrationJob(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "move_failover_method":
                                                {
                                                    Thread newThread = new Thread(() => Migration.FailoverServerMigration(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                        }
                                        break;
                                    }
                                case "workload":
                                    {
                                        switch (task.task_type)
                                        {
                                            case "create_method":
                                                Logger.log(String.Format("Found workload creation job in tasks: {0}", task.submitpayload.target.hostname), Logger.Severity.Info);
                                                Thread newThread = new Thread(() => MCP_Platform.ProvisionVM(task));
                                                newThread.Name = task.target_id;
                                                newThread.Start();
                                                lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                break;
                                        }
                                        break;
                                    }
                                case "platform":
                                    {
                                        switch (task.task_type)
                                        {
                                            case "discover_datacenters_method":
                                                {
                                                    Thread newThread = new Thread(() => DatacenterDiscovery.DatacenterDiscoveryDo(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "discovery_method":
                                                {
                                                    Thread newThread = new Thread(() => PlatformDiscovery.PlatformDiscoveryDo(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;

                                        }
                                        break;
                                    }
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
