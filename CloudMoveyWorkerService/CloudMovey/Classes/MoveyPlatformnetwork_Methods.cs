using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyPlatformnetwork : Core
    {
        public MoveyPlatformnetwork(CloudMovey _CloudMovey) : base(_CloudMovey) {
        }
         
        public MoveyPlatformnetworkListType listplatformnetworks()
        {
            endpoint = "/api/v1/platformnetworks/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyPlatformnetworkListType)post<MoveyPlatformnetworkListType>(worker);
        }

        public MoveyPlatformnetworkType createplatformnetwork(MoveyPlatformnetworkCRUDType _platformnetwork)
        {
            MoveyPlatformnetworksCRUDType platformnetwork = new MoveyPlatformnetworksCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformnetwork = _platformnetwork
            };

            endpoint = "/api/v1/platformnetworks/create.json";
            return (MoveyPlatformnetworkType)post<MoveyPlatformnetworkType>(platformnetwork);
        }
        public MoveyPlatformnetworkType updateplatformnetwork(MoveyPlatformnetworkCRUDType _platformnetwork)
        {
            MoveyPlatformnetworksCRUDType platformnetwork = new MoveyPlatformnetworksCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformnetwork = _platformnetwork
            };

            endpoint = "/api/v1/platformnetworks/update.json";
            return (MoveyPlatformnetworkType)put<MoveyPlatformnetworkType>(platformnetwork);
        }
    }
}


