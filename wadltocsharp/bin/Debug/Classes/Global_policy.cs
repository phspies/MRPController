using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Global_policy : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void setGlobalPolicy_Method(systemGlobalPolicy systemGlobalPolicy_object)
{
	endpoint = "/global_policy";
	mediatype="*/*";
	put(systemGlobalPolicy_object);
}



}
}
