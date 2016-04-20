using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPWorkload : Core
    {
        public MRPWorkload(MRP_ApiClient _MRP) : base(_MRP) {
        }
        public MRP_ApiClient MRP = new MRP_ApiClient();

        public MRPWorkloadListType listworkloads()
        {
            endpoint = "/workloads/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPWorkloadListType)post<MRPWorkloadListType>(worker);
        }

        public object getworkload(string _workload_id)
        {
            endpoint = "/workloads/get.json";
            MRPWorkloadGETType worker = new MRPWorkloadGETType()
            {
                workload_id = _workload_id
            };
            return post<MRPWorkloadType>(worker);
        }

        public ResultType createworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType platform = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/create.json";
            return post<ResultType>(platform);
        }
        public ResultType updateworkload(MRPWorkloadCRUDType _workload)
        {
            MRPWorkloadsCRUDType workload = new MRPWorkloadsCRUDType()
            {
                workload = _workload
            };

            endpoint = "/workloads/update.json";
            return put<ResultType>(workload);
        }

    }
}


