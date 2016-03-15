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
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                networkstat = _networkstat
            };
            endpoint = "/api/v1/networkstats/create.json";
            post<MRPNetworkStatCRUDType>(networkflow);
        }
    }
}


