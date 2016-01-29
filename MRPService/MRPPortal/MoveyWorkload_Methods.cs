using MRPService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.Portal
{
    class MRPWorkload : Core
    {
        public MRPWorkload(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
        public CloudMRPPortal CloudMRP = new CloudMRPPortal();

        public MRPWorkloadListType listworkloads()
        {
            endpoint = "/api/v1/workloads/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPWorkloadListType)post<MRPWorkloadListType>(worker);
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
            return (MRPWorkloadType)post<MRPWorkloadType>(platform);
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


