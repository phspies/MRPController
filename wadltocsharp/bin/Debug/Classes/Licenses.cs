using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Licenses : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public ClusterLicenseReportSet getLicenseReport_Method()
{
	endpoint = "/licenses/report";
	mediatype="application/json";
	return get<ClusterLicenseReportSet>();
}


public void removeLicense_Method(restString restString_object)
{
	endpoint = "/licenses";
	mediatype="*/*";
	delete(restString_object);
}



}
}
