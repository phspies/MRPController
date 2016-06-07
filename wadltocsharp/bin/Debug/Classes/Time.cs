using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Time : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public RecoverPointTimeStamp getCurrentTime_Method()
{
	endpoint = "/time/current_time";
	mediatype="application/json";
	return get<RecoverPointTimeStamp>();
}


public void setTimeSettings_Method(timeSettings timeSettings_object)
{
	endpoint = "/time/settings";
	mediatype="*/*";
	put(timeSettings_object);
}




}
}
