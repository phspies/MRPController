using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.DoubleTake.Common;
using System;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void StopDoubleTakeJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRMPWorkloadBaseType _target_workload = _managementobject.target_workload;
            try
            {
                ModuleCommon.StopJob(_task, _target_workload, _managementobject, 1, 100);
            }
            catch (Exception ex)
            {
                _task.failcomplete(ex.Message);
            }
        }
    }
}

