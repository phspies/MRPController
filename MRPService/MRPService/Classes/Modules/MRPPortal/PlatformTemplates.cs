using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace MRPService.API
{
    class MRPPlatformtemplate : Core
    {
        public MRPPlatformtemplate(ApiClient _CloudMRP) : base(_CloudMRP) {
        }
         
        public MRPPlatformtemplateListType listplatformtemplates()
        {
            endpoint = "/api/v1/platformtemplates/list.json";
            MRPCommandWorkerType worker = new MRPCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MRPPlatformtemplateListType)post<MRPPlatformtemplateListType>(worker);
        }

        public MRPPlatformtemplateType createplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/create.json";
            return (MRPPlatformtemplateType)post<MRPPlatformtemplateType>(platformtemplate);
        }
        public MRPPlatformtemplateType updateplatformtemplate(MRPPlatformtemplateCRUDType _platformtemplate)
        {
            MRPPlatformtemplatesCRUDType platformtemplate = new MRPPlatformtemplatesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/update.json";
            return (MRPPlatformtemplateType)put<MRPPlatformtemplateType>(platformtemplate);
        }
    }
}


