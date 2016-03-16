using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPNetworkstat : Core
    {
        public MRPNetworkstat(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(MRPNetworkStatCRUDType _networkstat)
        {
            MRPNetworkStatsCRUDType networkflow = new MRPNetworkStatsCRUDType()
            {
                networkstat = _networkstat
            };
            endpoint = "/networkstats/create.json";
            post<MRPNetworkStatCRUDType>(networkflow);
        }
    }
}


