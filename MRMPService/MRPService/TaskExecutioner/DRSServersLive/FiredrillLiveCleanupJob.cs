using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Modules.DoubleTake.Common;
using MRMPService.Modules.MCP;
using System;
using MRMPService.Modules.DoubleTake.Availability;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;

namespace MRMPService.TaskExecutioner.DRSServersLive
{
    partial class DRSServersLive
    {
        static public void FiredrillLiveCleanupJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRMPWorkloadBaseType _source_workload = _payload.source_workload;
            MRMPWorkloadBaseType _target_workload = _payload.target_workload;
            MRMPWorkloadBaseType _firedrill_workload = _payload.firedrill_workload;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _firedrill_workload.platform;
            _firedrill_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_firedrill_workload.id);


            try
            {
                ModuleCommon.DeployWindowsDoubleTake(_task, _target_workload, _firedrill_workload, 34, 66);
                MCP_Platform.DestroyWorkload(_task, _platform, _firedrill_workload, 67, 99);
                _task.successcomplete("Successfully cleaned up firedrill job and workload");

            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

