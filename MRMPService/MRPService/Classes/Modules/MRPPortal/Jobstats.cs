using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPJobstat : Core
    {
        public MRPJobstat(MRP_ApiClient _MRP) : base(_MRP) { }

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


