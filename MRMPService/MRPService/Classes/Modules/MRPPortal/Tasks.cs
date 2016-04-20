using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.API.Types.API;
using System;
using System.Diagnostics;
using System.Net;

namespace MRMPService.API
{
    class MRPTask : Core
    {
        public MRPTask(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPTaskListType tasks()
        {
            endpoint = "/tasks/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return post<MRPTaskListType>(worker);
        }

        public ResultType successcomplete(MRPTaskType payload, string returnpayload)
        {
            int _status = (bool)payload.internal_complete == true ? 3 : 0;
            int _percentage = (bool)payload.internal_complete == true ? 99 : 100;
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = _percentage,
                    returnpayload = returnpayload,
                    status = _status,
                    step = "Complete"
                }
            };
            endpoint = "/tasks/update.json";
            return put<ResultType>(task);
        }
        public ResultType successcomplete(MRPTaskType payload)
        {
            int _status = (bool)payload.internal_complete == true ? 3 : 0;
            int _percentage = (bool)payload.internal_complete == true ? 99 : 100;
            MRPCompleteTaskUpdateType task = new MRPCompleteTaskUpdateType()
            {
                task_id = payload.id,
                attributes = new MRPCompleteTaskUpdateAttributesType()
                {
                    percentage = _percentage,
                    status = _status,
                    step = "Complete"
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
                    status = 2,
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


