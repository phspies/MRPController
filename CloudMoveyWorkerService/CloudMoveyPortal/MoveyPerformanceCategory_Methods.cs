using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class MoveyPerformanceCategory : Core
    {
        public MoveyPerformanceCategory(CloudMoveyPortal _CloudMovey) : base(_CloudMovey) {
        }
         
        public void create(MoveyPerformanceCategoryCRUDType _performancecategory)
        {
            MoveyPerformanceCategoriesCRUDType performance = new MoveyPerformanceCategoriesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                performancecategory = _performancecategory
            };
            endpoint = "/api/v1/performancecategories/create.json";
            post<MoveyPerformanceCategoryType>(performance);

        }
        public MoveyPerformanceCategoryListType list()
        {
            endpoint = "/api/v1/performancecategories/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyPerformanceCategoryListType)post<MoveyPerformanceCategoryListType>(worker);

        }

    }
}


