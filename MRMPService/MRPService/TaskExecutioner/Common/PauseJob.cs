using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public async void PauseDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPWorkloadType _target_workload = _managementobject.target_workload;

            try
            {
                await ModuleCommon.PauseJob(_mrmp_task.id, _target_workload, _managementobject, 1, 100);

            }
            catch (Exception ex)
            {
                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

