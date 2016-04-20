using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRMPService.API
{
    class MRPPlatformtemplate : Core
    {
        public MRPPlatformtemplate(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformtemplateListType listplatformtemplates()
        {
            endpoint = "/platformtemplates/list.json";
            MRPCommandManagerType worker = new MRPCommandManagerType();
            return (MRPPlatformtemplateListType)post<MRPPlatformtemplateListType>(worker);
        }

        public MRPPlatformtemplateType createplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                platformtemplate = _platformtemplate
            };

            endpoint = "/platformtemplates/create.json";
            return (MRPPlatformtemplateType)post<MRPPlatformtemplateType>(platformtemplate);
        }
        public MRPPlatformtemplateType updateplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
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


