using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void StartDoubleTakeJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRMPWorkloadBaseType _target_workload = _managementobject.target_workload;
            try
            {
                ModuleCommon.StartJob(_task, _target_workload, _managementobject, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Failover Error: {0} ", ex.ToString()), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

