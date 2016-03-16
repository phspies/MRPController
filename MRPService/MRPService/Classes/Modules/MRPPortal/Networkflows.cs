﻿using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPNetworkflow : Core
    {
        public MRPNetworkflow(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void createnetworkflow(MRPNetworkFlowCRUDType _networkflow)
        {
            MRPNetworkFlowsCRUDType networkflow = new MRPNetworkFlowsCRUDType()
            {
                networkflow = _networkflow
            };
            endpoint = "/api/v1/networkflows/create.json";
            post<MRPNetworkFlowCRUDType>(networkflow);

        }
    }
}


