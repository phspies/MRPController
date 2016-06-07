using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Repository_volumes : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public RepositoryVolumeStateSet getRepositoryVolumeStateFromAllClusters_Method()
{
	endpoint = "/repository_volumes/state";
	mediatype="application/json";
	return get<RepositoryVolumeStateSet>();
}




}
}
