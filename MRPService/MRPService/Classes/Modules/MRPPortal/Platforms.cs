using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class PortalPlatform : Core
    {
        public PortalPlatform(ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformListType listplatforms()
        {
            endpoint = "/api/v1/platforms/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPlatformListType)post<MRPPlatformListType>(worker);
        }

        public MRPPlatformType getplatform(string _platform_id)
        {
            endpoint = "/api/v1/platforms/get_byid.json";
            MRPPlatformGETType worker = new MRPPlatformGETType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platform_id = _platform_id
            };
            return post<MRPPlatformType>(worker);
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


