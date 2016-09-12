using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class Common
    {
        static public void PauseDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _target_workload = _payload.target;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    ModuleCommon.PauseJob(_mrmp_task.id, _target_workload, _managementobject, 1, 100);

                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

