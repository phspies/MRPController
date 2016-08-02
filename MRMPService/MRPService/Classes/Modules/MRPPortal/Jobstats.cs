using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPManagementobjectStat : Core
    {
        public MRPManagementobjectStat(MRMP_ApiClient _MRP) : base(_MRP) { }

        public MRPManagementobjectStatListType listManagementobjectStats()
        {
            endpoint = "/managementobjectstats/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPManagementobjectStatListType)post<MRPManagementobjectStatListType>(worker);
        }

        public ResultType createManagementobjectStat(MRPManagementobjectStatType _ManagementobjectStat)
        {
            MRPManagementobjectStatsCRUDType ManagementobjectStat = new MRPManagementobjectStatsCRUDType()
            {
                ManagementobjectStat = _ManagementobjectStat
            };

            endpoint = "/managementobjectstats/create.json";
            return post<ResultType>(ManagementobjectStat);
        }

    }
}


