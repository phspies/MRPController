using MRPService.API.Types.API;
using System;
using System.Net;

namespace MRPService.API
{
    class MRPPlatformNetwork : Core
    {
        public MRPPlatformNetwork(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformnetworkListType listplatformnetworks()
        {
            endpoint = "/platformnetworks/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPPlatformnetworkListType)post<MRPPlatformnetworkListType>(worker);
        }

        public MRPPlatformnetworkType createplatformnetwork(MRPPlatformnetworkCRUDType _platformnetwork)
        {
            MRPPlatformnetworksCRUDType platformnetwork = new MRPPlatformnetworksCRUDType()
            {
                platformnetwork = _platformnetwork
            };

            endpoint = "/platformnetworks/create.json";
            return (MRPPlatformnetworkType)post<MRPPlatformnetworkType>(platformnetwork);
        }
        public MRPPlatformnetworkType updateplatformnetwork(MRPPlatformnetworkCRUDType _platformnetwork)
        {
            MRPPlatformnetworksCRUDType platformnetwork = new MRPPlatformnetworksCRUDType()
            {
                platformnetwork = _platformnetwork
            };

            endpoint = "/platformnetworks/update.json";
            return (MRPPlatformnetworkType)put<MRPPlatformnetworkType>(platformnetwork);
        }
    }
}


