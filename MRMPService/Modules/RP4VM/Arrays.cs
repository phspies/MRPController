using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{

    public class Arrays : Core
    {
        public Arrays(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public void addArray_Method(AddArrayParams addArrayParams_object)
        {
            endpoint = "/arrays";
            mediatype = "*/*";
            post(addArrayParams_object);
        }
    }
}
