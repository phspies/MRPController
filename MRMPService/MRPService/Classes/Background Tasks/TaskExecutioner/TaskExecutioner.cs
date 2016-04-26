using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MRMPService.API;
using MRMPService.Tasks;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;

namespace MRMPService.TaskExecutioner
{
    class TaskWorker
    {
        static int maxThreads = 30;
        MRP_ApiClient MRP = new MRP_ApiClient();
        public void Start()
        {
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();

            while (true)
            {
                MRPTaskListType tasklist = MRP.task().tasks();
                if (tasklist != null)
                {
                    foreach (MRPTaskType task in tasklist.tasks)
                    {
                        //make sure new target task does not have an active task busy
                        if ((lstThreads.FindAll(x => x.target_id == task.target_id).Count() == 0 && lstThreads.Count < maxThreads) || task.hidden == true)
                        {
                            switch (task.target_type)
                            {
                                case "dt":
                                    {
                                        switch (task.task_type)
                                        {
                                            case "deploy_method":
                                                {
                                                    Thread newThread = new Thread(() => Deploy.dt_deploy(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "sync_ha_method":
                                                {
                                                    Thread newThread = new Thread(() => Availability.dt_create_ha_syncjob(task));
                                                    newThread.Name = task.target_id;
                                                    newThread.Start();
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;

                                                //case "getproductinformation":
                                                //    if (task.submitpayload.dt != null)
                                                //    {
                                                //        //Thread newThread = new Thread(() => DoubleTakeNS.dt_getproductinformation(task));
                                                //        //newThread.Name = task.target_id;
                                                //        //newThread.Start(task);
                                                //        //lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                //    }
                                                //    break;
                                                //case "createdrseedjob":
                                                //    {
                                                //        //Thread newThread = new Thread(() => DT_DR.dt_create_dr_seedjob(task));
                                                //        //newThread.Name = task.target_id;
                                                //        //newThread.Start(task);
                                                //        //lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                //    }
                                                //    break;
                                                //case "createdrsyncjob":
                                                //    {
                                                //        //Thread newThread = new Thread(() => DT_DR.dt_create_dr_syncjob(task));
                                                //        //newThread.Name = task.target_id;
                                                //        //newThread.Start();
                                                //        //lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                //    }
                                                //    break;
                                                //case "createdrpopulatejob":
                                                //    {
                                                //        //Thread newThread = new Thread(() => DT_DR.dt_create_dr_restorejob(task));
                                                //        //newThread.Name = task.target_id;
                                                //        //newThread.Start();
                                                //        //lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                //    }
                                                //    break;
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
                                        switch ((string)task.task_type)
                                        {
                                            //Start MCP datacenters thread
                                            //case "getdatacenters":

                                            //    if (task.submitpayload.mcp != null)
                                            //    {
                                            //        Thread newThread = new Thread(() => MRPPlatform.mcp_getdatacenters(task));
                                            //        newThread.Name = task.target_id;
                                            //        newThread.Start();
                                            //        lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            //    }
                                            //    break;
                                            ////Start MCP templates thread
                                            //case "gettemplates":
                                            //    {
                                            //        if (task.submitpayload.mcp != null)
                                            //        {
                                            //            Thread newThread = new Thread(() => MRPPlatform.mcp_gettemplates(task));
                                            //            newThread.Name = task.target_id;
                                            //            newThread.Start();
                                            //            lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            //        }
                                            //        break;
                                            //    }
                                            ////Start MCP workloads thread
                                            //case "retrieveworkloads":
                                            //    {
                                            //        if (task.submitpayload.mcp != null)
                                            //        {
                                            //            Thread newThread = new Thread(() => MRPPlatform.mcp_retrieveworkloads(task));
                                            //            newThread.Name = task.target_id;
                                            //            newThread.Start();
                                            //            lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            //        }
                                            //        break;
                                            //    }
                                            //case "retrievenetworks":
                                            //    {
                                            //        if (task.submitpayload.mcp != null)
                                            //        {
                                            //            Thread newThread = new Thread(() => MRPPlatform.mcp_retrievenetworks(task));
                                            //            newThread.Name = task.target_id;
                                            //            newThread.Start(task);
                                            //            lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            //        }
                                            //        break;
                                            //    }
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

                lstThreads.RemoveAll(x => x.task.ThreadState == ThreadState.Stopped);
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
