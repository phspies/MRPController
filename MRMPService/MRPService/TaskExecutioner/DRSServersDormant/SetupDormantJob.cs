using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.Modules.DoubleTake.DR;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.DRSServersDormant
{
    partial class DRSServersDormant
    {
        static public void SetupDormantJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRMPWorkloadBaseType _source_workload = _payload.source_workload;
            MRMPWorkloadBaseType _target_workload = _payload.repository_workload;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                {
                    ModuleCommon.DeployWindowsDoubleTake(_task, _source_workload, _target_workload, 1, 50);
                    DisasterRecovery.CreateDRServerProtectionJob(_task, _source_workload, _target_workload, _protectiongroup, _managementobject, 51, 99);
                }
                _task.successcomplete("Successfully configured protection job");

            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error creating DR Protection Job {0}", ex.ToString()), Logger.Severity.Error);

                _task.failcomplete(ex.Message);

            }
        }
    }
}

