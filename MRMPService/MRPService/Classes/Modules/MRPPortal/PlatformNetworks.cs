using MRMPService.API.Types.API;
using System;
using System.Net;

namespace MRMPService.API
{
    class MRPPlatformNetwork : Core
    {
        public MRPPlatformNetwork(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformnetworkListType list_all()
        {
            endpoint = "/platformnetworks/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformnetworkListType)post<MRPPlatformnetworkListType>(worker);
        }
        public MRPPlatformnetworkListType list_by_platform(MRPPlatformType _platform)
        {
            endpoint = "/platformnetworks/list_by_platform.json";
            MRPPlatformGETType _filter_by_platform = new MRPPlatformGETType()
            {
                platform_id = _platform.id
            };
            return post<MRPPlatformnetworkListType>(_filter_by_platform);
        }
        public MRPPlatformnetworkType create(MRPPlatformnetworkType _platformnetwork)
        {
            MRPPlatformnetworksCRUDType platformnetwork = new MRPPlatformnetworksCRUDType()
            {
                platformnetwork = _platformnetwork
            };

            endpoint = "/platformnetworks/create.json";
            return (MRPPlatformnetworkType)post<MRPPlatformnetworkType>(platformnetwork);
        }
        public MRPPlatformnetworkType update(MRPPlatformnetworkType _platformnetwork)
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


