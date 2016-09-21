

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VMTypes
{

    public class State : Core
    {

        public State(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public FullRecoverPointStateContext getFullRecoverPointStateContext_Method()
        {
            endpoint = "/state/context";
            mediatype = "application/json";
            return get<FullRecoverPointStateContext>();
        }


        public FullRecoverPointState getFullRecoverPointState_Method()
        {
            endpoint = "/state";
            mediatype = "application/json";
            return get<FullRecoverPointState>();
        }



    }
}
