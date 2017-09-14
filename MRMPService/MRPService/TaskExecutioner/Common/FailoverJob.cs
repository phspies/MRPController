using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void FailoverDoubleTakeJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRMPWorkloadBaseType _source_workload = _payload.original_workload;
            MRMPWorkloadBaseType _target_workload = _payload.target_workload;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPManagementobjectSnapshotType _managementobjectsnapshot = _payload.managementobjectsnapshot;
            try
            {
                ModuleCommon.Failoverjob(_task, _source_workload, _target_workload, _managementobject, _managementobjectsnapshot, 1, 100);

                _task.progress(String.Format("Successfully failed over {0} to {1}", _source_workload.hostname, _target_workload.hostname), 99);
                _task.successcomplete(String.Format("Successfully failed over {0} to {1}", _source_workload.hostname, _target_workload.hostname));
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Failover Error: {0} ", ex.ToString()), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

