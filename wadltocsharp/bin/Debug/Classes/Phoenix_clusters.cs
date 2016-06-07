using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Phoenix_clusters : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public PhoenixClusterState getPhoenixClusterStateFromCluster_Method(long clusterId)
{
	endpoint = "/phoenix_clusters/{clusterId}/state";
	endpoint.Replace("{clusterId}",clusterId.ToString());
	mediatype="application/json";
	return get<PhoenixClusterState>();
}


public PhoenixClusterStateSet getPhoenixClusterStateFromAllCluster_Method()
{
	endpoint = "/phoenix_clusters/state";
	mediatype="application/json";
	return get<PhoenixClusterStateSet>();
}




}
}
