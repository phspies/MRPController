﻿using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class PortalPlatformDatacenter : Core
    {
        public PortalPlatformDatacenter(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformdatacenterListType list(MRPPlatformType _platform)
        {
            endpoint = "/platformdatacenters/list.json";
            MRPPlatformGETType _platform_filter = new MRPPlatformGETType();
            _platform_filter.platform_id = _platform.id;
            return post<MRPPlatformdatacenterListType>(_platform_filter);
        }

        public MRPPlatformdatacenterType create(MRPPlatformdatacenterType _platform_datacenter)
        {
            MRPPlatformdatacenterCRUDType datacenter = new MRPPlatformdatacenterCRUDType()
            {
                platformdatacenter = _platform_datacenter
            };

            endpoint = "/platformdatacenters/create.json";
            return post<MRPPlatformdatacenterType>(datacenter);
        }
        public MRPPlatformdatacenterType update(MRPPlatformdatacenterType _platform_datacenter)
        {
            MRPPlatformdatacenterCRUDType datacenter = new MRPPlatformdatacenterCRUDType()
            {
                platformdatacenter = _platform_datacenter
            };

            endpoint = "/platformdatacenters/update.json";
            return (put<MRPPlatformdatacenterType>(datacenter));
        }
    }
}


