using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPPerformanceCounter : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPPerformanceCounter()
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
        public MRPPerformanceCounter(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create(List<MRPPerformanceCounterCRUDType> _performancecounters)
        {
            MRPPerformanceCountersCRUDType performance = new MRPPerformanceCountersCRUDType()
            {
                performancecounters = _performancecounters
            };
            endpoint = "/performancecounters/create.json";
            post<MRPPerformanceCounterCRUDType>(performance);

        }
    }
}


