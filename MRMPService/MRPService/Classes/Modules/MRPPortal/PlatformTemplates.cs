using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.MRMPAPI
{
    class MRPPlatformtemplate : Core
    {
        public MRPPlatformtemplate(MRMP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformtemplateListType listplatformtemplates()
        {
            endpoint = "/platformtemplates/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformtemplateListType)post<MRPPlatformtemplateListType>(worker);
        }
        public MRPPlatformtemplateListType list_by_platform(MRPPlatformType _platform)
        {
            endpoint = "/platformtemplates/list_by_platform.json";
            MRPPlatformGETType _filter_by_platform = new MRPPlatformGETType()
            {
                platform_id = _platform.id
            };
            return post<MRPPlatformtemplateListType>(_filter_by_platform);
        }
        public MRPPlatformtemplateType createplatformtemplate(MRPPlatformtemplateType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                platformtemplate = _platformtemplate
            };

            endpoint = "/platformtemplates/create.json";
            return (MRPPlatformtemplateType)post<MRPPlatformtemplateType>(platformtemplate);
        }
        public MRPPlatformtemplateType updateplatformtemplate(MRPPlatformtemplateType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                platformtemplate = _platformtemplate
            };

            endpoint = "/platformtemplates/update.json";
            return (MRPPlatformtemplateType)put<MRPPlatformtemplateType>(platformtemplate);
        }
    }
}


