

using MRMPService.RP4VMAPI;

using System.Collections.Generic;



namespace MRMPService.RP4VM
{

    public class Reports : Core
    {

        public Reports(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public consistencyGroupReportSet getConsistencyGroupReports_Method(reportUIDSet reportUIDSet_object)
        {
            endpoint = "/reports/all";
            mediatype = "application/json";
            return post<consistencyGroupReportSet>(reportUIDSet_object);
        }


        public consistencyGroupReportStatsSet getConsistencyGroupReportStats_Method(reportUIDSet reportUIDSet_object)
        {
            endpoint = "/reports/statistics";
            mediatype = "application/json";
            return post<consistencyGroupReportStatsSet>(reportUIDSet_object);
        }




    }
}
