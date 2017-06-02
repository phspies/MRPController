using MRMPService.Modules.MCP;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public void PreviewCG(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPManagementobjectSnapshotType _managementobjectsnapshot = _payload.managementobjectsnapshot;
            MRPPlatformType _platform = _payload.target_platform;
            try
            {
                MCP_Platform.PreviewCG(_task, _platform, _protectiongroup, _managementobject, _workload_pairs, _managementobjectsnapshot, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.GetBaseException().Message);
            }
        }
    }
}

