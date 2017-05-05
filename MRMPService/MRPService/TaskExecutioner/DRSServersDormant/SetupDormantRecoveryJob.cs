using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.Modules.MCP;
using MRMPService.Modules.DoubleTake.DR;

namespace MRMPService.TaskExecutioner.DRSServersDormant
{
    partial class DRSServersDormant
    {
        static public void SetupDormantRecoveryJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPWorkloadType _original_workload = _payload.original_workload;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPPlatformType _platform = _payload.target_platform;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);

                //update target workload
                _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);

                ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 65);
                DisasterRecovery.CreateDRServerRecoveryJob(_mrmp_task.id, _source_workload, _target_workload, _original_workload, _protectiongroup, _managementobject, 66, 99);

                MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, "Successfully configured recovery job");
            }
           catch (Exception ex)
            {
                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.GetBaseException().Message);
            }
        }
    }
}

