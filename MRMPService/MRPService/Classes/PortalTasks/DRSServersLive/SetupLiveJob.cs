using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
using System;

namespace MRMPService.PortalTasks
{
    partial class DRSServersLive
    {
        static public async void SetupLiveJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;

            try
            {
                await MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);

                //update target workload
                _target_workload = await MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);


                if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                {
                    ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                    await Availability.CreateHAServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Full_Failover);
                }
                else
                {
                    ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                    await Availability.CreateHAServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Linux_FullFailover);
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

