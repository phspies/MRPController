using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Group_copies : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public ConsistencyGroupCopyProtectionWindowsInfoSet getAllGroupCopiesProtectionWindows_Method()
{
	endpoint = "/group_copies/protection_windows";
	mediatype="application/json";
	return get<ConsistencyGroupCopyProtectionWindowsInfoSet>();
}


public void unregulateAllConsistencyGroupCopies_Method()
{
	endpoint = "/group_copies/unregulate";
	mediatype="*/*";
	put();
}


public void addConsistencyGroupCopies_Method(newConsistencyGroupCopySettingsParamSet newConsistencyGroupCopySettingsParamSet_object)
{
	endpoint = "/group_copies";
	mediatype="*/*";
	post(newConsistencyGroupCopySettingsParamSet_object);
}



}
}
