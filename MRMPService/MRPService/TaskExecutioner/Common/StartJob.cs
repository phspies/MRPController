using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public async void StartDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                ModuleCommon.StartJob(_mrmp_task.id, _target_workload, _managementobject, 1, 100);

            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

