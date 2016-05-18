using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPJobstat : Core
    {
        public MRPJobstat(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPJobstatListType listjobstats()
        {
            endpoint = "/jobstats/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPJobstatListType)post<MRPJobstatListType>(worker);
        }

        public ResultType createjobstat(MRPJobstatType _jobstat)
        {
            MRPJobstatsCRUDType jobstat = new MRPJobstatsCRUDType()
            {
                jobstat = _jobstat
            };

            endpoint = "/jobstats/create.json";
            return post<ResultType>(jobstat);
        }

    }
}


