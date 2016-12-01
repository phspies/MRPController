﻿using MRMPService.MRMPAPI.Contracts;
using System.Collections.Generic;

namespace MRMPService.MRMPAPI
{
    public class MRPNetworkflow : Core
    {
        public MRPNetworkflow(MRMP_ApiClient _MRP) : base(_MRP) {
        }

        public async void createnetworkflow(List<MRPNetworkFlowCRUDType> _networkflows)
        {
            MRPNetworkFlowsCRUDType networkflow = new MRPNetworkFlowsCRUDType()
            {
                networkflows = _networkflows
            };
            endpoint = "/networkflows/create.json";
            await post<ResultType>(networkflow);

        }
    }
}


