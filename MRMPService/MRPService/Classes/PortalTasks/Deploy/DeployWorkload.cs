using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class Deploy
    {
        static public void DeployWorkload(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _target_workload = _payload.target;
            MRPPlatformType _platform = _payload.platform;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {

                    MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 99, true);
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully deployed workload");
                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

