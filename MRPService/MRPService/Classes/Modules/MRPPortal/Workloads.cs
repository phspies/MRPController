using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPWorkload : Core
    {
        public MRPWorkload(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPWorkloadListType listworkloads()
        {
            endpoint = "/workloads/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPWorkloadListType)post<MRPWorkloadListType>(worker);
        }

        public MRPWorkloadType getworkload(string _workload_id)
        {
            endpoint = "/workloads/get.json";
            MRPWorkloadGETType worker = new MRPWorkloadGETType()
            {
                workload_id = _workload_id
            };
            return post<MRPWorkloadType>(worker);
        }

        public MRPWorkloadType createworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType platform = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/create.json";
            return post<MRPWorkloadType>(platform);
        }
        public MRPWorkloadType updateworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType workload = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/update.json";
            return (MRPWorkloadType)put<MRPWorkloadType>(workload);
        }

    }
}


