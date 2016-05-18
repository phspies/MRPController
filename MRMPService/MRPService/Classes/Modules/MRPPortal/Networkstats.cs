using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPNetworkstat : Core
    {
        public MRPNetworkstat(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(MRPNetworkStatCRUDType _networkstat)
        {
            MRPNetworkStatsCRUDType networkflow = new MRPNetworkStatsCRUDType()
            {
                networkstat = _networkstat
            };
            endpoint = "/networkstats/create.json";
            post<ResultType>(networkflow);
        }
        public void create_bulk(List<MRPNetworkStatCRUDType> _networkstats)
        {
            MRPNetworkStatsBulkCRUDType networkflow = new MRPNetworkStatsBulkCRUDType()
            {
                networkstats = _networkstats
            };
            endpoint = "/networkstats/create_bulk.json";
            post<ResultType>(networkflow);
        }
    }
}


