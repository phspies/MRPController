using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class PortalFailovergroup : Core
    {
        public PortalFailovergroup(CloudMoveyPortal _CloudMovey) : base(_CloudMovey)
        {
        }
        public CloudMoveyPortal CloudMovey = new CloudMoveyPortal();

        public MoveyFailovergroupListType listfailovergroups()
        {
            endpoint = "/api/v1/failovergroups/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyFailovergroupListType)post<MoveyFailovergroupListType>(worker);
        }

        public MoveyFailovergroupType createfailovergroup(MoveyFailovergroupCRUDType _failovergroup)
        {
            MoveyFailovergroupsCRUDType platform = new MoveyFailovergroupsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                failovergroup = _failovergroup
            };
            endpoint = "/api/v1/failovergroups/create.json";
            return (MoveyFailovergroupType)post<MoveyFailovergroupType>(platform);
        }
        public MoveyFailovergroupType updatefailovergroup(MoveyFailovergroupCRUDType _failovergroup)
        {
            MoveyFailovergroupsCRUDType failovergroup = new MoveyFailovergroupsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                failovergroup = _failovergroup
            };
            endpoint = "/api/v1/failovergroups/update.json";
            return (MoveyFailovergroupType)put<MoveyFailovergroupType>(failovergroup);
        }

    }
}


