using MRPService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.Portal
{
    class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
         
        public void create(MRPPerformanceCounterCRUDType _performancecounter)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                performancecounter = _performancecounter
            };
            endpoint = "/api/v1/performancecounters/create.json";
            post<MRPPerformanceCounterCRUDType>(performance);

        }
    }
}


