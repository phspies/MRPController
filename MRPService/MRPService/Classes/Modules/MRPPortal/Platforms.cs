using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class PortalPlatform : Core
    {
        public PortalPlatform(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformListType listplatforms()
        {
            endpoint = "/api/v1/platforms/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPPlatformListType)post<MRPPlatformListType>(worker);
        }

        public MRPPlatformType getplatform(string _platform_id)
        {
            endpoint = "/api/v1/platforms/get_byid.json";
            MRPPlatformGETType worker = new MRPPlatformGETType()
            {
                platform_id = _platform_id
            };
            return post<MRPPlatformType>(worker);
        }

        public MRPPlatformType createplatform(MRPPlatformCRUDType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/api/v1/platforms/create.json";
            return (MRPPlatformType)post<MRPPlatformType>(platform);
        }
        public MRPPlatformType updateplatform(MRPPlatformCRUDType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/api/v1/platforms/update.json";
            return (MRPPlatformType)put<MRPPlatformType>(platform);
        }
    }
}


