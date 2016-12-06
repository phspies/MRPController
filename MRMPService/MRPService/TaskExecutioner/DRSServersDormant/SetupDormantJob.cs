using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.Modules.DoubleTake.DR;

namespace MRMPService.TaskExecutioner.DRSServersDormant
{
    partial class DRSServersDormant
    {
        static public async void SetupDormantJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.repository;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                {
                    ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 1, 50);
                    await DisasterRecovery.CreateDRServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 51, 99);
                }
                await MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, "Successfully configured protection job");

            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

