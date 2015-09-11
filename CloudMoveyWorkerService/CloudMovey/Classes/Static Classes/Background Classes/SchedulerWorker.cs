using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class SchedulerWorker
    {
        static int maxThreads = 30;
        CloudMovey CloudMovey = new CloudMovey();
        public void Start()
        {
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();
          
            while (true)
            {
                MoveyTaskListType tasklist = CloudMovey.task().tasks();
                if (tasklist != null)
                { 
                    foreach (MoveyTaskType task in tasklist.tasks)
                    {
                        //make sure new target task does not have an active task busy
                        if ((lstThreads.FindAll(x => x.target_id == task.target_id).Count() == 0 && lstThreads.Count < maxThreads) || task.hidden == true)
                        {
                            switch ((string)task.target_type)
                            {
                                case "dt":
                                    {
                                        switch ((string)task.submitpayload.task_action)
                                        {
                                            case "deploy":
                                                if (task.submitpayload.dt != null)
                                                {
                                                    Thread newThread = new Thread(() => DT.dt_deploy(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "getproductinformation":
                                                if (task.submitpayload.dt != null)
                                                {
                                                    Thread newThread = new Thread(() => DT.dt_getproductinformation(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createdrseedjob":
                                                {
                                                    Thread newThread = new Thread(() => DT_DR.dt_create_dr_seedjob(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createdrsyncjob":
                                                {
                                                    Thread newThread = new Thread(() => DT_DR.dt_create_dr_syncjob(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createdrpopulatejob":
                                                {
                                                    Thread newThread = new Thread(() => DT_DR.dt_create_dr_restorejob(task));
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
                                        switch ((string)task.submitpayload.task_action)
                                        {
                                            //Start MCP datacenters thread
                                            case "retrieveinformation":
                                                if (task.submitpayload.windows != null)
                                                {
                                                    Thread newThread = new Thread(() => CloudMoveyWorkload.workload_getinformation(task));
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createvmjob":
                                                if (task.submitpayload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(() => CloudMoveyPlatform.mcp_provisionvm(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                        }
                                        break;
                                    }
                                case "platform":
                                {
                                    switch ((string)task.submitpayload.task_action) 
                                    {
                                        //Start MCP datacenters thread
                                        case "getdatacenters":
                                        
                                            if (task.submitpayload.mcp != null)
                                            {
                                                Thread newThread = new Thread(() => CloudMoveyPlatform.mcp_getdatacenters(task));
                                                newThread.Name = task.target_id;
                                                newThread.Start();
                                                lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            }
                                            break;
                                        //Start MCP templates thread
                                        case "gettemplates":
                                            {
                                                if (task.submitpayload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(() => CloudMoveyPlatform.mcp_gettemplates(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            }
                                        //Start MCP workloads thread
                                        case "retrieveworkloads":
                                            {
                                                if (task.submitpayload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(() => CloudMoveyPlatform.mcp_retrieveworkloads(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            }
                                        case "retrievenetworks":
                                            {
                                                if (task.submitpayload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(() => CloudMoveyPlatform.mcp_retrievenetworks(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Global.event_log.WriteEntry("Agent not associated to organization!", System.Diagnostics.EventLogEntryType.Warning);
                }

                lstThreads.RemoveAll(x => x.task.ThreadState == ThreadState.Stopped);
                Global.worker_queue_count = lstThreads.Count();

                Thread.Sleep(5000);
            }
        }
    }
    public class ThreadObject
    {
        public Thread task { get; set; }
        public String target_id { get; set; }
    } 
}
