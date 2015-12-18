using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class MoveyPerformance : Core
    {
        public MoveyPerformance(CloudMoveyPortal _CloudMovey) : base(_CloudMovey) {
        }
         
        public void createnetworkflow(MoveyPerformanceCRUDType _performancecounter)
        {
            MoveyPerformancesCRUDType performance = new MoveyPerformancesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                performancecounter = _performancecounter
            };
            endpoint = "/api/v1/performancecounters/create.json";
            post<MoveyPerformanceCRUDType>(performance);

        }
    }
}


