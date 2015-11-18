using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class PortalCredential : Core
    {
        public PortalCredential(CloudMoveyPortal _CloudMovey) : base(_CloudMovey) {
        }
        public CloudMoveyPortal CloudMovey = new CloudMoveyPortal();

        public MoveyCredentialListType listcredentials()
        {
            endpoint = "/api/v1/credentials/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyCredentialListType)post<MoveyCredentialListType>(worker);
        }

        public MoveyCredentialType createcredential(MoveyCredentialCRUDType _credential)
        {
            MoveyCredentialsCRUDType platform = new MoveyCredentialsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                credential = _credential
            };
            endpoint = "/api/v1/credentials/create.json";
            return (MoveyCredentialType)post<MoveyCredentialType>(platform);
        }
        public MoveyCredentialType updatecredential(MoveyCredentialCRUDType _credential)
        {
            MoveyCredentialsCRUDType credential = new MoveyCredentialsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                credential = _credential
            };
            endpoint = "/api/v1/credentials/update.json";
            return (MoveyCredentialType)put<MoveyCredentialType>(credential);
        }

    }
}


