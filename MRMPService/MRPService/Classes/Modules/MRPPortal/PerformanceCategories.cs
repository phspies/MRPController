﻿using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPPerformanceCategory : Core
    {
        public MRPPerformanceCategory(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(MRPPerformanceCategoryCRUDType _performancecategory)
        {
            MRPPerformanceCategoriesCRUDType performance = new MRPPerformanceCategoriesCRUDType()
            {
                performancecategory = _performancecategory
            };
            endpoint = "/performancecategories/create.json";
            post<MRPPerformanceCategoryType>(performance);

        }
        public MRPPerformanceCategoryListType list()
        {
            endpoint = "/performancecategories/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPerformanceCategoryListType)post<MRPPerformanceCategoryListType>(worker);

        }

    }
}


