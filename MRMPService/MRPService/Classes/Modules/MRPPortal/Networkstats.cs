using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPNetworkstat : Core, IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPNetworkstat()
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
        public MRPNetworkstat(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public void create_bulk(List<MRPNetworkStatCRUDType> _networkstats)
        {
            MRPNetworkStatsBulkCRUDType networkflow = new MRPNetworkStatsBulkCRUDType()
            {
                networkstats = _networkstats
            };
            endpoint = "/networkstats/create_bulk.json";
            post<ResultType>(networkflow);
        }
    }
}


