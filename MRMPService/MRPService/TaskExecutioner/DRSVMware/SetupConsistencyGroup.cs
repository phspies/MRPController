using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.RP4VM;
using System;
using System.Linq;

namespace MRMPService.TaskExecutioner.DRSVMWare
{
    partial class DRSVMWare
    {
        static public async void SetupConsistencyGroup(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPPlatformType _source_platform = _payload.source_platform;
            MRPPlatformType _target_platform = _payload.target_platform;

            MRPManagementobjectType _mo = _payload.managementobject;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;

            try
            {
                await RP4VM.CreateConsistencyGroup(_task, _payload.workloadpairs.Select(x => x.source_workload).ToList(), _protectiongroup, _mo, 1, 99);
                _task.successcomplete("Successfully configured consistency group");
            }
            catch (Exception ex)
            {
                _task.failcomplete(ex.Message);
            }
        }
    }
}

