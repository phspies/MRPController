using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.Modules.MCP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public void FailoverCG(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;
            MRPManagementobjectSnapshotType _snapshot = _payload.managementobjectsnapshot;

            try
            {
                MCP_Platform.FailoverCG(_mrmp_task.id, _platform, _protectiongroup, _managementobject, _snapshot, _workload_pairs, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);

                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.GetBaseException().Message);

            }
        }
    }
}
