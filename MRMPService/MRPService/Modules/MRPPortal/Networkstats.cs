using MRMPService.Modules.MRMPPortal.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPNetworkstat : Core
    {
        public MRPNetworkstat(MRMPApiClient _MRP) : base(_MRP) {
        }

        public async Task<ResultType> create_bulk(List<MRPNetworkStatCRUDType> _networkstats)
        {
            MRPNetworkStatsBulkCRUDType networkflow = new MRPNetworkStatsBulkCRUDType()
            {
                networkstats = _networkstats
            };
            endpoint = "/networkstats/create_bulk.json";
            return await post<ResultType>(networkflow);
        }
    }
}


