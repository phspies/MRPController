﻿using MRPService.API.Types.API;
using System;
using System.Net;

namespace MRPService.API
{
    class MRPPlatformDomain : Core
    {
        public MRPPlatformDomain(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformdomainListType listplatformdomains()
        {
            endpoint = "/platformdomains/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPPlatformdomainListType)post<MRPPlatformdomainListType>(worker);
        }

        public MRPPlatformdomainType createplatformdomain(MRPPlatformdomainCRUDType _platformdomain)
        {
            MRPPlatformdomainsCRUDType platformdomain = new MRPPlatformdomainsCRUDType()
            {
                platformdomain = _platformdomain
            };

            endpoint = "/platformdomains/create.json";
            return (MRPPlatformdomainType)post<MRPPlatformdomainType>(platformdomain);
        }
        public MRPPlatformdomainType updateplatformdomain(MRPPlatformdomainCRUDType _platformdomain)
        {
            MRPPlatformdomainsCRUDType platformdomain = new MRPPlatformdomainsCRUDType()
            {
                platformdomain = _platformdomain
            };

            endpoint = "/platformdomains/update.json";
            return (MRPPlatformdomainType)put<MRPPlatformdomainType>(platformdomain);
        }
    }
}


