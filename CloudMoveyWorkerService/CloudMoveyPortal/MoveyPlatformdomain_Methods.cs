using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Net;

namespace CloudMoveyWorkerService.Portal
{
    class MoveyPlatformdomain : Core
    {
        public MoveyPlatformdomain(CloudMoveyPortal _CloudMovey) : base(_CloudMovey) {
        }
         
        public MoveyPlatformdomainListType listplatformdomains()
        {
            endpoint = "/api/v1/platformdomains/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyPlatformdomainListType)post<MoveyPlatformdomainListType>(worker);
        }

        public MoveyPlatformdomainType createplatformdomain(MoveyPlatformdomainCRUDType _platformdomain)
        {
            MoveyPlatformdomainsCRUDType platformdomain = new MoveyPlatformdomainsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformdomain = _platformdomain
            };

            endpoint = "/api/v1/platformdomains/create.json";
            return (MoveyPlatformdomainType)post<MoveyPlatformdomainType>(platformdomain);
        }
        public MoveyPlatformdomainType updateplatformdomain(MoveyPlatformdomainCRUDType _platformdomain)
        {
            MoveyPlatformdomainsCRUDType platformdomain = new MoveyPlatformdomainsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformdomain = _platformdomain
            };

            endpoint = "/api/v1/platformdomains/update.json";
            return (MoveyPlatformdomainType)put<MoveyPlatformdomainType>(platformdomain);
        }
    }
}


