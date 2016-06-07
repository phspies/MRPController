using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VM
{

    public class Logs : Core
    {

        public Logs(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public TransactionID collectLogs_Method(CollectLogsParams collectLogsParams_object)
        {
            endpoint = "/logs";
            mediatype = "application/json";
            return post<TransactionID>(collectLogsParams_object);
        }
    }
}
