using MRMPService.Modules.MRMPPortal.Contracts;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal
{
    public class MRPEvent : Core
    {
        public MRPEvent(MRMPApiClient _MRP) : base(_MRP) { }

        public ResultType create(MRMPEventType _event)
        {
            MRPEventsCRUDType @event = new MRPEventsCRUDType()
            {
                @event = _event
            };

            endpoint = "/events/create.json";
            return (ResultType)post<ResultType>(@event);
        }


    }
}


