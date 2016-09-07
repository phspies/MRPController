using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class Migrate
    {
        static public void FailoverMigrateJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _source_workload = _payload.source;
            MRPWorkloadType _target_workload = _payload.target;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            try
            {
                Migration.FailoverServerMigration(_mrmp_task.id, _source_workload, _target_workload, _managementobject, 1, 100);
                using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_portal.task().progress(_mrmp_task.id, String.Format("Successfully migrated {0} to {1}", _source_workload.hostname, _target_workload.hostname), 99);
                    _mrp_portal.task().successcomplete(_mrmp_task.id, String.Format("Successfully migrated {0} to {1}", _source_workload.hostname, _target_workload.hostname));
                }
            }
            catch (Exception ex)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);
                }
            }
        }
    }
}

