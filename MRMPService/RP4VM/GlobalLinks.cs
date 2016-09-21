
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{

    public class GlobalLinks : Core
    {
        public GlobalLinks(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public globalLinkStateSet getGlobalLinksState_Method()
        {
            endpoint = "/global_links/state";
            mediatype = "application/json";
            return get<globalLinkStateSet>();
        }
    }
}
