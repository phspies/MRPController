using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Rpas : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public RpaStatisticsSet getAllRPAStatistics_Method()
{
	endpoint = "/rpas/statistics";
	mediatype="application/json";
	return get<RpaStatisticsSet>();
}


public ClusterRPAsStateSet getRPAsStateFromAllClusters_Method()
{
	endpoint = "/rpas/state";
	mediatype="application/json";
	return get<ClusterRPAsStateSet>();
}




}
}
