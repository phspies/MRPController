using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Arrays : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void addArray_Method(addArrayParams addArrayParams_object)
{
	endpoint = "/arrays";
	mediatype="*/*";
	post(addArrayParams_object);
}



}
}
