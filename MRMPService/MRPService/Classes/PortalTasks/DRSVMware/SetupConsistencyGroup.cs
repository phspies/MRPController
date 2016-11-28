using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using MRMPService.Tasks.RP4VM;
using System.Linq;

namespace MRMPService.PortalTasks
{
    partial class DRSVMWare
    {
        static public void SetupConsistencyGroup(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPPlatformType _source_platform = _payload.source_platform;
            MRPPlatformType _target_platform = _payload.target_platform;

            MRPManagementobjectType _mo = _payload.managementobject;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;


            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    RP4VM.CreateConsistencyGroup(_mrmp_task.id, _payload.workloadpairs.Select(x => x.source_workload).ToList(), _protectiongroup, _mo, 1, 99);
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully configured consistency group");
                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

