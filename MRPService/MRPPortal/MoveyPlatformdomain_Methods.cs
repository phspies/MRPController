using MRPService.Portal.Types.API;
using System;
using System.Net;

namespace MRPService.Portal
{
    class MRPPlatformdomain : Core
    {
        public MRPPlatformdomain(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
         
        public MRPPlatformdomainListType listplatformdomains()
        {
            endpoint = "/api/v1/platformdomains/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPlatformdomainListType)post<MRPPlatformdomainListType>(worker);
        }

        public MRPPlatformdomainType createplatformdomain(MRPPlatformdomainCRUDType _platformdomain)
        {
            MRPPlatformdomainsCRUDType platformdomain = new MRPPlatformdomainsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformdomain = _platformdomain
            };

            endpoint = "/api/v1/platformdomains/create.json";
            return (MRPPlatformdomainType)post<MRPPlatformdomainType>(platformdomain);
        }
        public MRPPlatformdomainType updateplatformdomain(MRPPlatformdomainCRUDType _platformdomain)
        {
            MRPPlatformdomainsCRUDType platformdomain = new MRPPlatformdomainsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformdomain = _platformdomain
            };

            endpoint = "/api/v1/platformdomains/update.json";
            return (MRPPlatformdomainType)put<MRPPlatformdomainType>(platformdomain);
        }
    }
}


