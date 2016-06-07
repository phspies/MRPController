using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class ObjectsBrowser : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void getObjectsBrowserImageResource_Method(string resourceName)
{
	endpoint = "/objectsBrowser/images/{resourceName}";
	endpoint.Replace("{resourceName}",resourceName.ToString());
	mediatype="*/*";
	get();
}


public EntitiesIDInformation getEntitiesIdInformation_Method()
{
	endpoint = "/objectsBrowser/entitiesIdInformation";
	mediatype="application/json";
	return get<EntitiesIDInformation>();
}


public void getResource_Method(string resourceName)
{
	endpoint = "/objectsBrowser/{resourceName}/";
	endpoint.Replace("{resourceName}",resourceName.ToString());
	mediatype="*/*";
	get();
}


public void getObjectsBrowser_Method()
{
	endpoint = "/objectsBrowser";
	mediatype="*/*";
	get();
}



}
}
