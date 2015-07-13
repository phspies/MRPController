using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class Scheduler
    {
        CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
        public void Start()
        {
            TasksObject tasks = new TasksObject(CloudMovey);
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();
            while (true)
            {
                dynamic tasklist = tasks.tasks() as dynamic;
                foreach (var task in tasklist.list.Children())
                {
                    //make sure new target task does not have an active task busy
                    if (lstThreads.FindAll(x => x.target_id == (string)task.target_id).Count() == 0)
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
                lstThreads.RemoveAll(x => x.task.IsAlive == false);
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
