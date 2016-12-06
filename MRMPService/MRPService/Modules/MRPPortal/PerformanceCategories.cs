using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPPerformanceCategory : Core
    {
        public MRPPerformanceCategory(MRMPApiClient _MRP) : base(_MRP) {
        }
         
        public async Task<ResultType> create(MRPPerformanceCategoryCRUDType _performancecategory)
        {
            MRPPerformanceCategoriesCRUDType performance = new MRPPerformanceCategoriesCRUDType()
            {
                performancecategory = _performancecategory
            };
            endpoint = "/performancecategories/create.json";
            return await post<ResultType>(performance);

        }
        public async Task<MRPPerformanceCategoryListType> list(MRPPerformanceCategoryFilterType _filter = null)
        {
            endpoint = "/performancecategories/list.json";
            return await post<MRPPerformanceCategoryListType>(_filter);
        }
    }
}


