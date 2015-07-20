using CloudMoveyWorkerService.CloudMovey.Models;
using CloudMoveyWorkerService.CloudMovey.Types;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class TasksObject : Core
    {
        public TasksObject(CloudMovey _CloudMovey) : base(_CloudMovey) {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }
        public CloudMovey CloudMovey = new CloudMovey(Global.apiBase, null, null);

        public dynamic tasks()
        {
            endpoint = "/api/v1/tasks/list.json";
            CommandWorker worker = new CommandWorker(CloudMovey) { id = Global.agentId, hostname = Environment.MachineName };
            RestResponse tasks = post(worker) as RestResponse;
            //string JsonContentFixed = tasks.Content.Replace(@"\", " ");
            dynamic dynamicObject = null;
            if (tasks != null)
            {
                dynamicObject = JObject.Parse(tasks.Content);
            }
            return dynamicObject;
        }
        public bool successcomplete(dynamic payload, string returnpayload)
        {
            CompleteTaskUpdateObject task = new CompleteTaskUpdateObject()
            {
                id = Global.agentId,
                hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttriubutes()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = 0,
                    step = "Complete"
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(task);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(),System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool successcomplete(dynamic payload)
        {
            CompleteTaskUpdateObject task = new CompleteTaskUpdateObject()
            {
                id = Global.agentId,
                hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttriubutes()
                {
                    percentage = 100,
                    status = 0,
                    step = "Complete"
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(task);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool failcomplete(dynamic payload, string returnpayload)
        {
            CompleteTaskUpdateObject task = new CompleteTaskUpdateObject()
            {
                id = Global.agentId,
                hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttriubutes()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = 2,
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(task);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(dynamic payload, string _step, double _progress)
        {
            ProgressTaskUpdateObject task = new ProgressTaskUpdateObject()
            {
                id = Global.agentId,
                hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new ProgressTaskUpdateAttriubutes()
                {
                    percentage = _progress,
                    step = _step
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(task);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(dynamic payload, string _step)
        {
            ProgressTaskUpdateObject task = new ProgressTaskUpdateObject()
            {
                id = Global.agentId,
                hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new ProgressTaskUpdateAttriubutes()
                {
                    step = _step
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(task);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool update(object _object) 
        {
            endpoint = "/api/v1/tasks/update.json";
            object returnval = put(_object);
            if (returnval is Error)
            {
                Global.eventLog.WriteEntry(returnval.ToString(), System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        
        }

    }
}


