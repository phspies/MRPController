using MRMPService.Modules.MRMPPortal.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPPerformanceCounter : Core
    {
        public MRPPerformanceCounter(MRMPApiClient _MRP) : base(_MRP) {
        }
         
        public ResultType create(List<MRPPerformanceCounterCRUDType> _performancecounters)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                performancecounters = _performancecounters
            };
            endpoint = "/performancecounters/create.json";
            return post<ResultType>(performance);
        }
    }
}


