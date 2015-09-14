using CloudMoveyWorkerService.CloudMovey.Types.API;
using System;
using System.Collections.Generic;
using System.Net;

namespace CloudMoveyWorkerService.CloudMovey
{
    class MoveyPlatformtemplate : Core
    {
        public MoveyPlatformtemplate(CloudMovey _CloudMovey) : base(_CloudMovey) {
        }
         
        public MoveyPlatformtemplateListType listplatformtemplates()
        {
            endpoint = "/api/v1/platformtemplates/list.json";
            MoveyCommandWorkerType worker = new MoveyCommandWorkerType() { worker_id = Global.agent_id, worker_hostname = Environment.MachineName };
            return (MoveyPlatformtemplateListType)post<MoveyPlatformtemplateListType>(worker);
        }

        public MoveyPlatformtemplateType createplatformtemplate(MoveyPlatformtemplateCRUDType _platformtemplate)
        {
            MoveyPlatformtemplatesCRUDType platformtemplate = new MoveyPlatformtemplatesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/create.json";
            return (MoveyPlatformtemplateType)post<MoveyPlatformtemplateType>(platformtemplate);
        }
        public MoveyPlatformtemplateType updateplatformtemplate(MoveyPlatformtemplateCRUDType _platformtemplate)
        {
            MoveyPlatformtemplatesCRUDType platformtemplate = new MoveyPlatformtemplatesCRUDType()
            {
                worker_id = Global.agent_id,
                worker_hostname = Environment.MachineName,
                platformtemplate = _platformtemplate
            };

            endpoint = "/api/v1/platformtemplates/update.json";
            return (MoveyPlatformtemplateType)put<MoveyPlatformtemplateType>(platformtemplate);
        }
    }
}


