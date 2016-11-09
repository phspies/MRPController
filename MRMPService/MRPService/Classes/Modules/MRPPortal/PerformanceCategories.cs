using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPPerformanceCategory : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPPerformanceCategory()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
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
        public MRPPerformanceCategoryListType list(MRPPerformanceCategoryFilterType _filter = null)
        {
            endpoint = "/performancecategories/list.json";
            return (MRPPerformanceCategoryListType)post<MRPPerformanceCategoryListType>(_filter);

        }

    }
}


