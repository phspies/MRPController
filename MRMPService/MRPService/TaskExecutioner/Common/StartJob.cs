using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void StartDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPWorkloadType _target_workload = _managementobject.target_workload;
            try
            {
                ModuleCommon.StartJob(_mrmp_task.id, _target_workload, _managementobject, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Failover Error: {0} ", ex.ToString()), Logger.Severity.Fatal);
                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
            }
        }
    }
}

