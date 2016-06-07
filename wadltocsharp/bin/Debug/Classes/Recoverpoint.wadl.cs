using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Recoverpoint.wadl : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void getWadl_Method()
{
	endpoint = "/recoverpoint.wadl";
	mediatype="application/vnd.sun.wadl+xml";
	get();
}



}
}
