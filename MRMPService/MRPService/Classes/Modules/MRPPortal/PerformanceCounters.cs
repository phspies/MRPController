using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(MRPPerformanceCounterCRUDType _performancecounter)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                performancecounter = _performancecounter
            };
            endpoint = "/performancecounters/create.json";
            post<MRPPerformanceCounterCRUDType>(performance);

        }
    }
}


