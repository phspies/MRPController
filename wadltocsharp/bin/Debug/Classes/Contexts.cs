using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Contexts : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public FullRecoverPointContext getFullRecoverPointContext_Method()
{
	endpoint = "/contexts/full";
	mediatype="application/json";
	return get<FullRecoverPointContext>();
}


public ClariionVolumesContext getClariionVolumesContext_Method(long clusterId)
{
	endpoint = "/contexts/clariion_volumes/{clusterId}";
	endpoint.Replace("{clusterId}",clusterId.ToString());
	mediatype="application/json";
	return get<ClariionVolumesContext>();
}


public VCenterServerViewContext getVCenterServerViewContext_Method(long clusterId)
{
	endpoint = "/contexts/vcenter_servers/{clusterId}";
	endpoint.Replace("{clusterId}",clusterId.ToString());
	mediatype="application/json";
	return get<VCenterServerViewContext>();
}


public ClusterSANVolumesContext getClusterSANVolumesContext_Method(long clusterId)
{
	endpoint = "/contexts/san_volumes/{clusterId}";
	endpoint.Replace("{clusterId}",clusterId.ToString());
	mediatype="application/json";
	return get<ClusterSANVolumesContext>();
}


public SystemStatusContext getSystemStatusContext_Method()
{
	endpoint = "/contexts/system_status";
	mediatype="application/json";
	return get<SystemStatusContext>();
}


public FullRecoverPointStateContext getFullRecoverPointStateContext_Method()
{
	endpoint = "/contexts/state";
	mediatype="application/json";
	return get<FullRecoverPointStateContext>();
}


public FullRecoverPointSettingsContext getFullRecoverPointSettingsContext_Method()
{
	endpoint = "/contexts/settings";
	mediatype="application/json";
	return get<FullRecoverPointSettingsContext>();
}


public RestLinkList getClusterSANVolumesContextLinks_Method()
{
	endpoint = "/contexts/san_volumes";
	mediatype="application/json";
	return get<RestLinkList>();
}


public RestLinkList getClariionVolumesContextLinks_Method()
{
	endpoint = "/contexts/clariion_volumes";
	mediatype="application/json";
	return get<RestLinkList>();
}


public RestLinkList getVCenterServerViewContextLinks_Method()
{
	endpoint = "/contexts/vcenter_servers";
	mediatype="application/json";
	return get<RestLinkList>();
}


public RestLinkList getContextsLinks_Method()
{
	endpoint = "/contexts";
	mediatype="application/json";
	return get<RestLinkList>();
}



}
}
