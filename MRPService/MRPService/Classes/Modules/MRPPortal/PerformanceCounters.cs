using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(MRP_ApiClient _MRP) : base(_MRP) {
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


