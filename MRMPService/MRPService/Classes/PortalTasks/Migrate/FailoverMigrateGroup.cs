using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using System.Linq;
using MRMPService.Utilities;

namespace MRMPService.PortalTasks
{
    partial class Migrate
    {
        static public void FailoverMigrateGroup(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            int _mo_count = _payload.managementobjects.Count;
            int _count = 1;
            int _increment = (100 / _mo_count);
            try
            {
                foreach (MRPManagementobjectOrderType _mo_order in _payload.managementobjects.OrderBy(x => x.position))
                {
                    MRPManagementobjectType _managementobject = _mo_order.managementobject;
                    int _low = _count == 1 ? 1 : (_increment * (_count - 1) + 1);
                    int _high = _increment * _count;
                    Migration.FailoverServerMigration(_mrmp_task.id, _managementobject.source_workload, _managementobject.target_workload, _managementobject, _low, _high,true);
                    _count++;
                }
                using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_portal.task().progress(_mrmp_task.id, String.Format("Successfully migrated group"), 99);
                    _mrp_portal.task().successcomplete(_mrmp_task.id, String.Format("Successfully migrated group"));
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

