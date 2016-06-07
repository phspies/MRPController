using MRMPService.RP4VM;

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

public class Logs : Core
{

public Clusters(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

public TransactionID collectLogs_Method(collectLogsParams collectLogsParams_object)
{
	endpoint = "/logs";
	mediatype="application/json";
	return post<TransactionID>(collectLogsParams_object);
}



}
}
