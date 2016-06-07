
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{
    public class RP4VMTime : Core
    {
        public RP4VMTime(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public RecoverPointTimeStamp getCurrentTime_Method()
        {
            endpoint = "/time/current_time";
            mediatype = "application/json";
            return get<RecoverPointTimeStamp>();
        }


        public void setTimeSettings_Method(TimeSettings timeSettings_object)
        {
            endpoint = "/time/settings";
            mediatype = "*/*";
            put(timeSettings_object);
        }
    }
}
