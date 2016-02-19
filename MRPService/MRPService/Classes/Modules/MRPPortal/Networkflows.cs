using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPNetworkflow : Core
    {
        public MRPNetworkflow(ApiClient _CloudMRP) : base(_CloudMRP) {
        }
         
        public void createnetworkflow(MRPNetworkFlowCRUDType _networkflow)
        {
            MRPNetworkFlowsCRUDType networkflow = new MRPNetworkFlowsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                networkflow = _networkflow
            };
            endpoint = "/api/v1/networkflows/create.json";
            post<MRPNetworkFlowCRUDType>(networkflow);

        }
    }
}


