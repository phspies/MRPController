using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using System;
using MRMPService.Tasks.MCP;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;

namespace MRMPService.PortalTasks
{
    partial class Deploy
    {
        static public async void DeployWorkload(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPPlatformType _platform = _target_workload.platform;
            MRPProtectiongroupType _protectiongroup = _target_workload.protectiongroup;
            try
            {
                await MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 99, true);
                await MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, "Successfully deployed workload");
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
            }
        }
    }
}

