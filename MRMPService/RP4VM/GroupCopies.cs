
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{

    public class GroupCopies : Core
    {
        public GroupCopies(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public consistencyGroupCopyProtectionWindowsInfoSet getAllGroupCopiesProtectionWindows_Method()
        {
            endpoint = "/group_copies/protection_windows";
            mediatype = "application/json";
            return get<consistencyGroupCopyProtectionWindowsInfoSet>();
        }


        public void unregulateAllConsistencyGroupCopies_Method()
        {
            endpoint = "/group_copies/unregulate";
            mediatype = "*/*";
            put();
        }


        public void addConsistencyGroupCopies_Method(newConsistencyGroupCopySettingsParamSet newConsistencyGroupCopySettingsParamSet_object)
        {
            endpoint = "/group_copies";
            mediatype = "*/*";
            post(newConsistencyGroupCopySettingsParamSet_object);
        }
    }
}
