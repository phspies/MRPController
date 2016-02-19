using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPWorkload : Core
    {
        public MRPWorkload(ApiClient _MRP) : base(_MRP) {
        }
        public ApiClient MRP = new ApiClient();

        public MRPWorkloadListType listworkloads()
        {
            endpoint = "/api/v1/workloads/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPWorkloadListType)post<MRPWorkloadListType>(worker);
        }

        public MRPWorkloadType getworkload(string _workload_id)
        {
            endpoint = "/api/v1/workloads/get.json";
            MRPWorkloadGETType worker = new MRPWorkloadGETType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                workload_id = _workload_id
                
            };
            return post<MRPWorkloadType>(worker);
        }

        public MRPWorkloadType createworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType platform = new MRPWorkloadsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                workload = _workload
            };

            endpoint = "/api/v1/workloads/create.json";
            return post<MRPWorkloadType>(platform);
        }
        public MRPWorkloadType updateworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType workload = new MRPWorkloadsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                workload = _workload
            };

            endpoint = "/api/v1/workloads/update.json";
            return (MRPWorkloadType)put<MRPWorkloadType>(workload);
        }

    }
}


