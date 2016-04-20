using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPNetworkflow : Core
    {
        public MRPNetworkflow(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void createnetworkflow(List<MRPNetworkFlowCRUDType> _networkflows)
        {
            MRPNetworkFlowsCRUDType networkflow = new MRPNetworkFlowsCRUDType()
            {
                networkflows = _networkflows
            };
            endpoint = "/networkflows/create.json";
            post<MRPNetworkFlowCRUDType>(networkflow);

        }
    }
}


