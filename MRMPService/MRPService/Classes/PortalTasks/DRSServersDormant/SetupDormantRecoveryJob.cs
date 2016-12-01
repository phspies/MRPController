using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class DRSServersDormant
    {
        static public async void SetupDormantRecoveryJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPWorkloadType _original_workload = _payload.original;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPPlatformType _platform = _payload.target_platform;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                await MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);

                //update target workload
                _target_workload = await MRMPServiceBase._mrmp_api_endpoint.workload().get_by_id(_target_workload.id);

                ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 65);
                DisasterRecovery.CreateDRServerRecoveryJob(_mrmp_task.id, _source_workload, _target_workload, _original_workload, _protectiongroup, _managementobject, 66, 99);

                await MRMPServiceBase._mrmp_api_endpoint.task().successcomplete(_mrmp_task.id, "Successfully configured recovery job");
            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api_endpoint.task().failcomplete(_mrmp_task.id, ex.GetBaseException().Message);
            }
        }
    }
}

