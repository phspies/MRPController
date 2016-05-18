using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(List<MRPPerformanceCounterCRUDType> _performancecounters)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                performancecounters = _performancecounters
            };
            endpoint = "/performancecounters/create.json";
            post<MRPPerformanceCounterCRUDType>(performance);

        }
    }
}


