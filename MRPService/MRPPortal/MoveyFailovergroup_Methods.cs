using MRPService.Portal.Types.API;
using System;
using System.Net;

namespace MRPService.Portal
{
    class PortalFailovergroup : Core
    {
        public PortalFailovergroup(CloudMRPPortal _CloudMRP) : base(_CloudMRP)
        {
        }
        public CloudMRPPortal CloudMRP = new CloudMRPPortal();

        public MRPFailovergroupListType listfailovergroups()
        {
            endpoint = "/api/v1/failovergroups/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPFailovergroupListType)post<MRPFailovergroupListType>(worker);
        }

        public MRPFailovergroupType createfailovergroup(MRPFailovergroupCRUDType _failovergroup)
        {
            MRPFailovergroupsCRUDType platform = new MRPFailovergroupsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                failovergroup = _failovergroup
            };
            endpoint = "/api/v1/failovergroups/create.json";
            return (MRPFailovergroupType)post<MRPFailovergroupType>(platform);
        }
        public MRPFailovergroupType updatefailovergroup(MRPFailovergroupCRUDType _failovergroup)
        {
            MRPFailovergroupsCRUDType failovergroup = new MRPFailovergroupsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                failovergroup = _failovergroup
            };
            endpoint = "/api/v1/failovergroups/update.json";
            return (MRPFailovergroupType)put<MRPFailovergroupType>(failovergroup);
        }

    }
}


