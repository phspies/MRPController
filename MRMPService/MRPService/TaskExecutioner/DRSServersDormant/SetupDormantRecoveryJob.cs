using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.Modules.MCP;
using MRMPService.Modules.DoubleTake.DR;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.DRSServersDormant
{
    partial class DRSServersDormant
    {
        static public void SetupDormantRecoveryJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPWorkloadType _original_workload = _payload.original_workload;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPPlatformType _platform = _payload.target_platform;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPManagementobjectSnapshotType _snapshot = _payload.managementobjectsnapshot;

            try
            {
                if (!(bool)_target_workload.provisioned)
                {
                    MCP_Platform.ProvisionVM(_task, _platform, _target_workload, _protectiongroup, 1, 33, true);
                    _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);
                }
                ModuleCommon.DeployWindowsDoubleTake(_task, _source_workload, _target_workload, 34, 65);
                DisasterRecovery.CreateDRServerRecoveryJob(_task, _source_workload, _target_workload, _original_workload, _protectiongroup, _managementobject, _snapshot, 66, 99);
                _task.successcomplete("Successfully configured recovery job");
            }
           catch (Exception ex)
            {
                Logger.log(String.Format("Error creating DR Recovery Job {0}", ex.ToString()), Logger.Severity.Error);
                _task.failcomplete(ex.GetBaseException().Message);
            }
        }
    }
}

