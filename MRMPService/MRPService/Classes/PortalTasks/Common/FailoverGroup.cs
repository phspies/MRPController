using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using System.Linq;
using MRMPService.Utilities;

namespace MRMPService.PortalTasks
{
    partial class Common
    {
        static public void FailoverDoubleTakeGroup(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;

            try
            {
                if (_payload.managementobjects.Count() == 0)
                {
                    using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_portal.task().progress(_mrmp_task.id, String.Format("No failover operations found"), 99);
                        _mrp_portal.task().successcomplete(_mrmp_task.id);
                    }
                }
                else
                {
                    int _mo_count = _payload.managementobjects.Count;
                    int _count = 1;
                    int _increment = (100 / _mo_count);
                    foreach (MRPManagementobjectOrderType _mo_order in _payload.managementobjects.OrderBy(x => x.position))
                    {
                        MRPManagementobjectType _managementobject = _mo_order.managementobject;
                        int _low = _count == 1 ? 1 : (_increment * (_count - 1) + 1);
                        int _high = _increment * _count;
                        ModuleCommon.Failoverjob(_mrmp_task.id, _mo_order.original, _managementobject.target_workload, _managementobject, _low, _high, true, (bool)_mo_order.firedrill);
                        _count++;
                    }
                    using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_portal.task().progress(_mrmp_task.id, String.Format("Successfully migrated group"), 99);
                        _mrp_portal.task().successcomplete(_mrmp_task.id);
                    }
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

