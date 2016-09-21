
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{

    public class Rpas : Core
    {
        public Rpas(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public rpaStatisticsSet getAllRPAStatistics_Method()
        {
            endpoint = "/rpas/statistics";
            mediatype = "application/json";
            return get<rpaStatisticsSet>();
        }


        public clusterRPAsStateSet getRPAsStateFromAllClusters_Method()
        {
            endpoint = "/rpas/state";
            mediatype = "application/json";
            return get<clusterRPAsStateSet>();
        }




    }
}
