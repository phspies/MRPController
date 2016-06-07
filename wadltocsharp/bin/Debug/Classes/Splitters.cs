using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Splitters : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public void rescanSplittersVolumesConnectionsInAllClusters_Method()
{
	endpoint = "/splitters/volume_connections/rescan";
	mediatype="*/*";
	put();
}


public ClusterSplittersSANViewSet getSplittersSANViewFromAllClusters_Method()
{
	endpoint = "/splitters/san_view";
	mediatype="application/json";
	return get<ClusterSplittersSANViewSet>();
}


public ClusterSplittersSettingsSet getSplittersSettingsFromAllClusters_Method()
{
	endpoint = "/splitters/settings";
	mediatype="application/json";
	return get<ClusterSplittersSettingsSet>();
}


public ClusterSplittersStateSet getSplittersStateFromAllClusters_Method()
{
	endpoint = "/splitters/state";
	mediatype="application/json";
	return get<ClusterSplittersStateSet>();
}


public void rescanSpecificSplittersVolumesConnections_Method(splitterUIDSet splitterUIDSet_object)
{
	endpoint = "/splitters/specific/volume_connections/rescan";
	mediatype="*/*";
	put(splitterUIDSet_object);
}


public ClusterAvailableSplittersSet getAvailableSplittersSettingsFromAllCluster_Method()
{
	endpoint = "/splitters/available";
	mediatype="application/json";
	return get<ClusterAvailableSplittersSet>();
}




}
}
