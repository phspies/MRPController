using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPEvent : Core
    {
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


