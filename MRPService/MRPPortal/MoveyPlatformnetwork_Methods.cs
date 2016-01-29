﻿using MRPService.Portal.Types.API;
using System;
using System.Net;

namespace MRPService.Portal
{
    class MRPPlatformnetwork : Core
    {
        public MRPPlatformnetwork(CloudMRPPortal _CloudMRP) : base(_CloudMRP) {
        }
         
        public MRPPlatformnetworkListType listplatformnetworks()
        {
            endpoint = "/api/v1/platformnetworks/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPlatformnetworkListType)post<MRPPlatformnetworkListType>(worker);
        }

        public MRPPlatformnetworkType createplatformnetwork(MRPPlatformnetworkCRUDType _platformnetwork)
        {
            MRPPlatformnetworksCRUDType platformnetwork = new MRPPlatformnetworksCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformnetwork = _platformnetwork
            };

            endpoint = "/api/v1/platformnetworks/create.json";
            return (MRPPlatformnetworkType)post<MRPPlatformnetworkType>(platformnetwork);
        }
        public MRPPlatformnetworkType updateplatformnetwork(MRPPlatformnetworkCRUDType _platformnetwork)
        {
            MRPPlatformnetworksCRUDType platformnetwork = new MRPPlatformnetworksCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformnetwork = _platformnetwork
            };

            endpoint = "/api/v1/platformnetworks/update.json";
            return (MRPPlatformnetworkType)put<MRPPlatformnetworkType>(platformnetwork);
        }
    }
}


