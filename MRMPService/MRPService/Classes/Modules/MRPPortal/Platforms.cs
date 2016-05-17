using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class PortalPlatform : Core
    {
        public PortalPlatform(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformListType list()
        {
            endpoint = "/platforms/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformListType)post<MRPPlatformListType>(worker);
        }

        public MRPPlatformType get_by_id(string _platform_id)
        {
            endpoint = "/platforms/get_byid.json";
            MRPPlatformGETType worker = new MRPPlatformGETType()
            {
                platform_id = _platform_id
            };
            return post<MRPPlatformType>(worker);
        }

        public MRPPlatformType create(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/create.json";
            return (MRPPlatformType)post<MRPPlatformType>(platform);
        }
        public MRPPlatformType update(MRPPlatformType _platform)
        {
            MRPPlatformsCRUDType platform = new MRPPlatformsCRUDType()
            {
                platform = _platform
            };

            endpoint = "/platforms/update.json";
            return (MRPPlatformType)put<MRPPlatformType>(platform);
        }
    }
}


