using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using MRMPService.Modules.MCP;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Deploy
{
    partial class Deploy
    {
        static public void DeployWorkload(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPPlatformType _platform = _target_workload.platform;
            MRPProtectiongroupType _protectiongroup = _target_workload.protectiongroup;
            try
            {
                MCP_Platform.ProvisionVM(_task, _platform, _target_workload, _protectiongroup, 1, 99, true);
                _task.successcomplete("Successfully deployed workload");
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

