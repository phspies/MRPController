using MRMPService.MRMPAPI.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI
{
    public class MRPNetworkstat : Core
    {
        public MRPNetworkstat(MRMP_ApiClient _MRP) : base(_MRP) {
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


