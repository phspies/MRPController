using MRMPService.Modules.MRMPPortal.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(MRMPApiClient _MRP) : base(_MRP) {
        }
         
        public async Task<ResultType> create(List<MRPPerformanceCounterCRUDType> _performancecounters)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                performancecounters = _performancecounters
            };
            endpoint = "/performancecounters/create.json";
            return await post<ResultType>(performance);
        }
    }
}


