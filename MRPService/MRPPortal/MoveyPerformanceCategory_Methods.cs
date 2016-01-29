using MRPService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.Portal
{
    class MRPPerformanceCategory : Core
    {
        public MRPPerformanceCategory(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
         
        public void create(MRPPerformanceCategoryCRUDType _performancecategory)
        {
            MRPPerformanceCategoriesCRUDType performance = new MRPPerformanceCategoriesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                performancecategory = _performancecategory
            };
            endpoint = "/api/v1/performancecategories/create.json";
            post<MRPPerformanceCategoryType>(performance);

        }
        public MRPPerformanceCategoryListType list()
        {
            endpoint = "/api/v1/performancecategories/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPerformanceCategoryListType)post<MRPPerformanceCategoryListType>(worker);

        }

    }
}


