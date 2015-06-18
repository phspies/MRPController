using CladesWorkerService.Clades.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.Controllers
{
    class Scheduler
    {
        CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
        public void Start()
        {
            TasksObject tasks = new TasksObject(clades);
            List<string> activeObjects = new List<string>();
            List<ThreadObject> lstThreads = new List<ThreadObject>();
            while (true)
            {
                dynamic tasklist = tasks.tasks() as dynamic;
                foreach (var task in tasklist.list.Children())
                {
                    //make sure new target task does not have an active task busy
                    if (lstThreads.FindAll(x => x.target_id == task.target_id).Count() == 0)
                    {
                        switch ((string)task.target_type)
                        {
                            case "platform":
                            {
                                //Start MCP platform thread
                                if (task.payload.mcp != null)
                                {
                                    Thread newThread = new Thread(Platform.discover_mcp);
                                    newThread.Name = task.target_id;
                                    newThread.Start(task);
                                    lstThreads.Add(new ThreadObject() { task = newThread, target_id = task.target_id });
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
