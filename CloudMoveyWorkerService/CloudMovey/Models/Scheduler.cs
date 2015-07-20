using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Scheduler
    {
        static int maxThreads = 30;
        CloudMovey CloudMovey = new CloudMovey(Global.apiBase, null, null);
        public void Start()
        {
            TasksObject tasks = new TasksObject(CloudMovey);
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();
          
            while (true)
            {
                dynamic tasklist = tasks.tasks() as dynamic;
                if (tasklist != null)
                { 
                    foreach (var task in tasklist.list.Children())
                    {
                        //make sure new target task does not have an active task busy
                        if (lstThreads.FindAll(x => x.target_id == (string)task.target_id).Count() == 0 && lstThreads.Count < maxThreads)
                        {
                            switch ((string)task.target_type)
                            {
                                case "dt":
                                    {
                                        switch ((string)task.payload.task_action)
                                        {
                                            case "deploy":
                                                if (task.payload.dt != null)
                                                {
                                                    Thread newThread = new Thread(DT.dt_deploy);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "getproductinformation":
                                                if (task.payload.dt != null)
                                                {
                                                    Thread newThread = new Thread(DT.dt_getproductinformation);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createdrseedjob":
                                                {
                                                    Thread newThread = new Thread(DT.dt_create_dr_seedjob);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createdrsyncjob":
                                                {
                                                    Thread newThread = new Thread(DT.dt_create_dr_syncjob);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                        }
                                        break;
                                    }
                                case "server":
                                    {
                                        switch ((string)task.payload.task_action)
                                        {
                                            //Start MCP datacenters thread
                                            case "retrieveinformation":
                                                if (task.payload.windows != null)
                                                {
                                                    Thread newThread = new Thread(Server.server_getinformation);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            case "createvmjob":
                                                if (task.payload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(Platform.mcp_provisionvm);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                        }
                                        break;
                                    }
                                case "platform":
                                {
                                    switch ((string)task.payload.task_action) 
                                    {
                                        //Start MCP datacenters thread
                                        case "getdatacenters":
                                        
                                            if (task.payload.mcp != null)
                                            {
                                                Thread newThread = new Thread(Platform.mcp_getdatacenters);
                                                newThread.Name = task.target_id;
                                                newThread.Start(task);
                                                lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                            }
                                            break;
                                        //Start MCP templates thread
                                        case "gettemplates":
                                            {
                                                if (task.payload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(Platform.mcp_gettemplates);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            }
                                        //Start MCP servers thread
                                        case "retrieveservers":
                                            {
                                                if (task.payload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(Platform.mcp_retrieveservers);
                                                    newThread.Name = task.target_id;
                                                    newThread.Start(task);
                                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
                                                }
                                                break;
                                            }
                                        case "retrievenetworks":
                                            {
                                                if (task.payload.mcp != null)
                                                {
                                                    Thread newThread = new Thread(Platform.mcp_retrievenetworks);
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
                    Global.eventLog.WriteEntry("Agent not associated to organization!", System.Diagnostics.EventLogEntryType.Warning);
                }

                lstThreads.RemoveAll(x => x.task.ThreadState == ThreadState.Stopped);
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
