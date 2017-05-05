using MRMPService.Modules.MRMPPortal.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPNetworkflow : Core
    {
        public MRPNetworkflow(MRMPApiClient _MRP) : base(_MRP)
        {
        }

        public void createnetworkflow(List<MRPNetworkFlowCRUDType> _networkflows)
        {
            MRPNetworkFlowsCRUDType networkflow = new MRPNetworkFlowsCRUDType()
            {
                networkflows = _networkflows
            };
            endpoint = "/networkflows/create.json";
            post<ResultType>(networkflow);

        }
    }
}


