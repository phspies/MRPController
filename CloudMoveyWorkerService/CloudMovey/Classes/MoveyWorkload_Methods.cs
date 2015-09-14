using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyWorkload : Core
    {
        public MoveyWorkload(CloudMovey _CloudMovey) : base(_CloudMovey) {
        }
        public CloudMovey CloudMovey = new CloudMovey();

        public MoveyWorkloadListType listworkloads()
        {
            endpoint = "/api/v1/workloads/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyWorkloadListType)post<MoveyWorkloadListType>(worker);
        }

        public MoveyWorkloadType createworkload(MoveyWorkloadCRUDType _workload)
        {
            MoveyWorkloadsCRUDType platform = new MoveyWorkloadsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                workload = _workload
            };

            endpoint = "/api/v1/workloads/create.json";
            return (MoveyWorkloadType)post<MoveyWorkloadType>(platform);
        }
        public MoveyWorkloadType updateworkload(MoveyWorkloadCRUDType _workload)
        {
            MoveyWorkloadsCRUDType workload = new MoveyWorkloadsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                workload = _workload
            };

            endpoint = "/api/v1/workloads/update.json";
            return (MoveyWorkloadType)put<MoveyWorkloadType>(workload);
        }

    }
}


