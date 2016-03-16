using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPPerformanceCategory : Core
    {
        public MRPPerformanceCategory(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(MRPPerformanceCategoryCRUDType _performancecategory)
        {
            MRPPerformanceCategoriesCRUDType performance = new MRPPerformanceCategoriesCRUDType()
            {
                performancecategory = _performancecategory
            };
            endpoint = "/api/v1/performancecategories/create.json";
            post<MRPPerformanceCategoryType>(performance);

        }
        public MRPPerformanceCategoryListType list()
        {
            endpoint = "/api/v1/performancecategories/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPPerformanceCategoryListType)post<MRPPerformanceCategoryListType>(worker);

        }

    }
}


