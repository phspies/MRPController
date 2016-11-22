using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using MRMPService.MRMPService.Log;

namespace MRMPService.PortalTasks
{
    partial class Deploy
    {
        static public void DeployWorkload(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPPlatformType _platform = _target_workload.platform;
            MRPProtectiongroupType _protectiongroup = _target_workload.protectiongroup;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {

                    MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 99, true);
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully deployed workload");
                }
                catch (Exception ex)
                {
                    Logger.log(ex.ToString(), Logger.Severity.Fatal);
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

