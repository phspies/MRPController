﻿using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPPlatformtemplate : Core
    {
        public MRPPlatformtemplate(MRP_ApiClient _MRP) : base(_MRP) {
        }
         
        public MRPPlatformtemplateListType listplatformtemplates()
        {
            endpoint = "/api/v1/platformtemplates/list.json";
            MRPCommandControllerType worker = new MRPCommandControllerType();
            return (MRPPlatformtemplateListType)post<MRPPlatformtemplateListType>(worker);
        }

        public MRPPlatformtemplateType createplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/create.json";
            return (MRPPlatformtemplateType)post<MRPPlatformtemplateType>(platformtemplate);
        }
        public MRPPlatformtemplateType updateplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/update.json";
            return (MRPPlatformtemplateType)put<MRPPlatformtemplateType>(platformtemplate);
        }
    }
}


