using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPEvent : Core , IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MRPEvent()
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

        public MRPEvent(MRMP_ApiClient _MRP) : base(_MRP) { }

        public ResultType create(MRMPEventType _event)
        {
            MRPEventsCRUDType @event = new MRPEventsCRUDType()
            {
                @event = _event
            };

            endpoint = "/events/create.json";
            return post<ResultType>(@event);
        }


    }
}


