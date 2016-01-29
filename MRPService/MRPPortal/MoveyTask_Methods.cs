using MRPService.MRPService.Log;
using MRPService.Portal.Types.API;
using System;
using System.Diagnostics;
using System.Net;

namespace MRPService.Portal
{
    class MRPTask : Core
    {
        public MRPTask(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
        public CloudMRPPortal CloudMRP = new CloudMRPPortal();

        public dynamic tasks()
        {
            endpoint = "/api/v1/tasks/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return post<MRPTaskListType>(worker);
        }

        public bool successcomplete(MRPTaskType payload, string returnpayload)
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
            object returnval = put<MRPTaskType>(task);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool successcomplete(MRPTaskType payload)
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
            object returnval = put<MRPTaskType>(task);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool failcomplete(MRPTaskType payload, string returnpayload)
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
            object returnval = put<MRPTaskType>(task);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(MRPTaskType payload, string _step, double _progress)
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
            object returnval = put<MRPTaskType>(task);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool progress(MRPTaskType payload, string _step)
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
            object returnval = put<MRPTaskType>(task);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool update(MRPTaskType _object) 
        {
            endpoint = "/api/v1/tasks/update.json";
            object returnval = put<MRPTaskType>(_object);
            if (returnval is MRPError)
            {
                Logger.log((returnval as MRPError).error, Logger.Severity.Error);
                return false;
            }
            else
            {
                return true;
            }
        
        }

    }
}


