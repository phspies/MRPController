using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Wadl.xsl : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void getXSL_Method()
{
	endpoint = "/wadl.xsl";
	mediatype="*/*";
	get();
}



}
}
