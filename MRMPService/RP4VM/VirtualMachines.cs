

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

    public class VirtualMachines : Core
    {

        public VirtualMachines(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public ConsistencyGroupUID GetConsistencyGroupUIDAccordingToVMBiosUuid_Method(string vmBiosUuid)
        {
            endpoint = "/virtual_machines/{vmBiosUuid}/group";
            endpoint.Replace("{vmBiosUuid}", vmBiosUuid.ToString());
            mediatype = "application/json";
            return get<ConsistencyGroupUID>();
        }
    }
}
