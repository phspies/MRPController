using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI
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
       
        public MRPTask(MRMP_ApiClient _MRP) : base(_MRP)
        {
        }
        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public async Task<MRPTaskListType> tasks()
        {
            endpoint = "/tasks/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPTaskListType)await post<MRPTaskListType>(worker);
        }
        public async Task<MRPTaskType> get(string _task_id)
        {
            var _get_task = new MRPTaskGetType() { task_id = _task_id };
            endpoint = "/tasks/get.json";
            return (MRPTaskType)await post<MRPTaskType>(_get_task);
        }
        public async Task<ResultType> successcomplete(String _task_id, string returnpayload)
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
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> successcomplete(MRPTaskType payload, string returnpayload)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };
            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> successcomplete(String _task_id)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = "Complete",
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> successcomplete(MRPTaskType payload)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = "Complete",
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> failcomplete(String _task_id, string returnpayload)
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
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> failcomplete(MRPTaskType payload, string returnpayload)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    returnpayload = returnpayload,
                    status = TaskStatus.Failed,
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> progress(String _task_id, string _step, double _progress)
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
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> progress(MRPTaskType payload, string _step, double _progress)
        {
            MRPProgressTaskUpdateType task = new MRPProgressTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPProgressTaskUpdateAttributesType()
                {
                    percentage = _progress,
                    step = _step,
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> progress(String _task_id, string _step)
        {
            MRPProgressTaskUpdateType task = new MRPProgressTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPProgressTaskUpdateAttributesType()
                {
                    step = _step
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> progress(MRPTaskType payload, string _step)
        {
            MRPProgressTaskUpdateType task = new MRPProgressTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPProgressTaskUpdateAttributesType()
                {
                    step = _step
                }
            };

            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(task);
        }
        public async Task<ResultType> update(MRPTaskType _object)
        {
            endpoint = "/tasks/update.json";
            return (ResultType)await put<ResultType>(_object);
        }
    }
}


