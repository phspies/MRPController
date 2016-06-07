using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Systemconflicts : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public RestBoolean areThereSystemSettingsConflicts_Method()
{
	endpoint = "/system/conflicts/are_there";
	mediatype="application/json";
	return get<RestBoolean>();
}


public SystemSettingsConflicts getSystemSettingsConflicts_Method()
{
	endpoint = "/system/conflicts";
	mediatype="application/json";
	return get<SystemSettingsConflicts>();
}



}
}
