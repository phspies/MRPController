using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
using System;

namespace MRMPService.PortalTasks
{
    partial class DRSServersLive
    {
        static public void SetupLiveJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _source_workload = _payload.source;
            MRPWorkloadType _target_workload = _payload.target;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.platform;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);

                    //update target workload
                    _target_workload = _mrp_portal.workload().get_by_id(_target_workload.id);


                    if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                    {
                        ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                        Availability.CreateHAServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Full_Failover);
                    }
                    else
                    {
                        ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                        Availability.CreateHAServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Linux_FullFailover);
                    }
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully configured protection job");

                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

