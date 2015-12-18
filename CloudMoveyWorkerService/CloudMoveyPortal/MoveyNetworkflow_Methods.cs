using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class MoveyNetworkflow : Core
    {
        public MoveyNetworkflow(CloudMoveyPortal _CloudMovey) : base(_CloudMovey) {
        }
         
        public void createnetworkflow(MoveyNetworkFlowCRUDType _networkflow)
        {
            MoveyNetworkFlowsCRUDType networkflow = new MoveyNetworkFlowsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                networkflow = _networkflow
            };
            endpoint = "/api/v1/networkflows/create.json";
            post<MoveyNetworkFlowCRUDType>(networkflow);

        }
    }
}


