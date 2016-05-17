using MRMPService.API.Types.API;
using System;
using System.Net;

namespace MRMPService.API
{
    class MRPPlatformDomain : Core
    {
        public MRPPlatformDomain(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformdomainListType list()
        {
            endpoint = "/platformdomains/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformdomainListType)post<MRPPlatformdomainListType>(worker);
        }
        public MRPPlatformdomainListType list_by_platform(MRPPlatformType _platform)
        {
            endpoint = "/platformdomains/list_by_platform.json";
            MRPPlatformGETType _filter_by_platform = new MRPPlatformGETType()
            {
                platform_id = _platform.id
            };
            return post<MRPPlatformdomainListType>(_filter_by_platform);
        }
        public MRPPlatformdomainType create(MRPPlatformdomainType _platformdomain)
        {
            MRPPlatformdomainsCRUDType platformdomain = new MRPPlatformdomainsCRUDType()
            {
                platformdomain = _platformdomain
            };

            endpoint = "/platformdomains/create.json";
            return (MRPPlatformdomainType)post<MRPPlatformdomainType>(platformdomain);
        }
        public MRPPlatformdomainType update(MRPPlatformdomainType _platformdomain)
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


