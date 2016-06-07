using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Global_links : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public GlobalLinkStateSet getGlobalLinksState_Method()
{
	endpoint = "/global_links/state";
	mediatype="application/json";
	return get<GlobalLinkStateSet>();
}




}
}
