using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Diagnostics;
using System.Net;

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
    class MRPTask : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPTask()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public MRPTask(MRMP_ApiClient _MRP) : base(_MRP)
        {
        }
        public MRMP_ApiClient MRP = new MRMP_ApiClient();

        public MRPTaskListType tasks()
        {
            endpoint = "/tasks/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPTaskListType>(worker);
        }
        public ResultType successcomplete(String _task_id, string returnpayload)
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
            return put<ResultType>(task);
        }
        public ResultType successcomplete(MRPTaskType payload, string returnpayload)
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
            return put<ResultType>(task);
        }
        public ResultType successcomplete(String _task_id)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = _task_id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };

            endpoint = "/tasks/update.json";
            return put<ResultType>(task);
        }
        public ResultType successcomplete(MRPTaskType payload)
        {
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = 100,
                    status = TaskStatus.Success,
                    step = "Complete"
                }
            };

            endpoint = "/tasks/update.json";
            return put<ResultType>(task);
        }
        public ResultType failcomplete(String _task_id, string returnpayload)
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
            return put<ResultType>(task);
        }
        public ResultType failcomplete(MRPTaskType payload, string returnpayload)
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
            return put<ResultType>(task);
        }
        public ResultType progress(String _task_id, string _step, double _progress)
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
            return put<ResultType>(task);
        }
        public ResultType progress(MRPTaskType payload, string _step, double _progress)
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
            return put<ResultType>(task);
        }
        public ResultType progress(String _task_id, string _step)
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
            return put<ResultType>(task);
        }
        public ResultType progress(MRPTaskType payload, string _step)
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
            return put<ResultType>(task);
        }
        public ResultType update(MRPTaskType _object)
        {
            endpoint = "/tasks/update.json";
            return put<ResultType>(_object);
        }

    }
}


