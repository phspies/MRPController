using MRMPService.Modules.MRMPPortal.Contracts;

using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void FailoverDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _source_workload = _payload.original_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPManagementobjectSnapshotType _managementobjectsnapshot = _payload.managementobjectsnapshot;
            try
            {
                ModuleCommon.Failoverjob(_mrmp_task.id, _source_workload, _target_workload, _managementobject, _managementobjectsnapshot, 1, 100);

                MRMPServiceBase._mrmp_api.task().progress(_mrmp_task.id, String.Format("Successfully failed over {0} to {1}", _source_workload.hostname, _target_workload.hostname), 99);
                MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, String.Format("Successfully failed over {0} to {1}", _source_workload.hostname, _target_workload.hostname));
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Failover Error: {0} ", ex.ToString()), Logger.Severity.Fatal);
                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
            }
        }
    }
}

