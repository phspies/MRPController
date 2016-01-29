using MRPService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.Portal
{
    class PortalPlatform : Core
    {
        public PortalPlatform(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
         
        public MRPPlatformListType listplatforms()
        {
            endpoint = "/api/v1/platforms/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPlatformListType)post<MRPPlatformListType>(worker);
        }

        public MRPPlatformType createplatform(MRPPlatformCRUDType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platform = _platform
            };

            endpoint = "/api/v1/platforms/create.json";
            return (MRPPlatformType)post<MRPPlatformType>(platform);
        }
        public MRPPlatformType updateplatform(MRPPlatformCRUDType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platform = _platform
            };

            endpoint = "/api/v1/platforms/update.json";
            return (MRPPlatformType)put<MRPPlatformType>(platform);
        }
    }
}


