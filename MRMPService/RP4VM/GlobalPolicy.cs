
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{

    public class GlobalPolicy : Core
    {
        public GlobalPolicy(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public void setGlobalPolicy_Method(SystemGlobalPolicy systemGlobalPolicy_object)
        {
            endpoint = "/global_policy";
            mediatype = "*/*";
            put(systemGlobalPolicy_object);
        }
    }
}
