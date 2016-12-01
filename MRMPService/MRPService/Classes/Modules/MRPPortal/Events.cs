using MRMPService.MRMPAPI.Contracts;
using System.Threading.Tasks;

namespace MRMPService.MRMPAPI
{
    public class MRPEvent : Core
    {
        public MRPEvent(MRMP_ApiClient _MRP) : base(_MRP) { }

        public async Task<ResultType> create(MRMPEventType _event)
        {
            MRPEventsCRUDType @event = new MRPEventsCRUDType()
            {
                @event = _event
            };

            endpoint = "/events/create.json";
            return (ResultType)await post<ResultType>(@event);
        }


    }
}


