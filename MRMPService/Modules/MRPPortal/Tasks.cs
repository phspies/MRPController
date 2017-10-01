using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public static class TaskStatus
    {
        public const int Success = 0;
        public const int Queue = 1;
        public const int Processing = 2;
        public const int Failed = 3;
        public const int InternalComplete = 4;
        }
    public class MRPTask : Core
    {
       
        public MRPTask(MRMPApiClient _MRP) : base(_MRP)
        {
        }
        public MRPTaskListType tasks()
        {
            endpoint = "/tasks/list.json";
            MRPTaskListType _result = new MRPTaskListType() { tasks = new List<MRPTaskType>() };
            try
            {
                _result = post<MRPTaskListType>(null);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating task: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
            return _result;
        }
        public MRPTaskType get(MRPTaskType _task)
        {
            var _get_task = new MRPTaskGetType() { task_id = _task.id };
            endpoint = "/tasks/get.json";
            MRPTaskType _result = new MRPTaskType();
            try
            {
                _result = post<MRPTaskType>(_get_task);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating task: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
            return _result;
        }
        public void update_successcomplete(String _task_id, string returnpayload = null)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };
            endpoint = "/tasks/update.json";
            try
            {
                put<ResultType>(task);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating task: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }
        public void update_failcomplete(String _task_id, string returnpayload)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = TaskStatus.Failed,
                }
            };

            endpoint = "/tasks/update.json";
            try
            {
                put<ResultType>(task);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating task: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }
        public void update_progress(String _task_id, string _step, double? _progress = null)
        {
            MRPProgressTaskUpdateType task = new MRPProgressTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPProgressTaskUpdateAttributesType()
                {
                    percentage = _progress,
                    step = _step,
                }
            };

            endpoint = "/tasks/update.json";
            try
            {
                put<ResultType>(task);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating task: {0}", ex.ToString()), Logger.Severity.Fatal);
            }
        }
    }
}


