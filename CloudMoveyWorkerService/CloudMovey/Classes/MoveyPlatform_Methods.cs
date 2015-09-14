using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyPlatform : Core
    {
        public MoveyPlatform(CloudMovey _CloudMovey) : base(_CloudMovey) {
        }
         
        public MoveyPlatformListType listplatforms()
        {
            endpoint = "/api/v1/platforms/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyPlatformListType)post<MoveyPlatformListType>(worker);
        }

        public MoveyPlatformType createplatform(MoveyPlatformCRUDType _platform)
        {
            MoveyPlatformsCRUDType platform = new MoveyPlatformsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platform = _platform
            };

            endpoint = "/api/v1/platforms/create.json";
            return (MoveyPlatformType)post<MoveyPlatformType>(platform);
        }
        public MoveyPlatformType updateplatform(MoveyPlatformCRUDType _platform)
        {
            MoveyPlatformsCRUDType platform = new MoveyPlatformsCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platform = _platform
            };

            endpoint = "/api/v1/platforms/update.json";
            return (MoveyPlatformType)put<MoveyPlatformType>(platform);
        }
    }
}


