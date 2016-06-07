using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Reports : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public ConsistencyGroupReportSet getConsistencyGroupReports_Method(reportUIDSet reportUIDSet_object)
{
	endpoint = "/reports/all";
	mediatype="application/json";
	return post<ConsistencyGroupReportSet>(reportUIDSet_object);
}


public ConsistencyGroupReportStatsSet getConsistencyGroupReportStats_Method(reportUIDSet reportUIDSet_object)
{
	endpoint = "/reports/statistics";
	mediatype="application/json";
	return post<ConsistencyGroupReportStatsSet>(reportUIDSet_object);
}




}
}
