using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Diagnostics;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyTask : Core
    {
        public MoveyTask(CloudMovey _CloudMovey) : base(_CloudMovey) {
        }
        public CloudMovey CloudMovey = new CloudMovey();

        public dynamic tasks()
        {
            endpoint = "/api/v1/tasks/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return post<MoveyTaskListType>(worker);
        }

        public bool successcomplete(MoveyTaskType payload, string returnpayload)
        {
            int _status = (bool)payload.internal_complete == true ? 3 : 0;
            int _percentage = (bool)payload.internal_complete == true ? 99 : 100;
            CompleteTaskUpdateType task = new CompleteTaskUpdateType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttributesType()
                {
                    percentage = _percentage,
                    returnpayload = returnpayload,
                    status = _status,
                    step = "Complete"
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(task);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool successcomplete(MoveyTaskType payload)
        {
            int _status = (bool)payload.internal_complete == true ? 3 : 0;
            int _percentage = (bool)payload.internal_complete == true ? 99 : 100;
            CompleteTaskUpdateType task = new CompleteTaskUpdateType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttributesType()
                {
                    percentage = _percentage,
                    status = _status,
                    step = "Complete"
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(task);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool failcomplete(MoveyTaskType payload, string returnpayload)
        {
            CompleteTaskUpdateType task = new CompleteTaskUpdateType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new CompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = 2,
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(task);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(MoveyTaskType payload, string _step, double _progress)
        {
            ProgressTaskUpdateType task = new ProgressTaskUpdateType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new ProgressTaskUpdateAttributesType()
                {
                    percentage = _progress,
                    step = _step
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(task);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(MoveyTaskType payload, string _step)
        {
            ProgressTaskUpdateType task = new ProgressTaskUpdateType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                task_id = payload.id,
                attributes = new ProgressTaskUpdateAttributesType()
                {
                    step = _step
                }
            };

            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(task);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool update(MoveyTaskType _object) 
        {
            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MoveyTaskType>(_object);
            if (returnval is MoveyError)
            {
                Global.event_log.WriteEntry((returnval as MoveyError).error, EventLogEntryType.Error);
                return false;
            }
            else
            {
                return true;
            }
        
        }

    }
}


