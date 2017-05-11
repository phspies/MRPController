using MRMPService.Modules.DoubleTake.Common;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Linq;

namespace MRMPService.TaskExecutioner.Common
{
    partial class Common
    {
        static public void FailoverDoubleTakeGroup(MRPTaskType _mrmp_task)
        {
            
            try
            {
                MRPTaskDetailType _payload = _mrmp_task.taskdetail;
                if (_payload.managementobjects.Count() == 0)
                {

                    MRMPServiceBase._mrmp_api.task().progress(_mrmp_task.id, String.Format("No failover operations found"), 99);
                    MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id);
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
                        ModuleCommon.Failoverjob(_mrmp_task.id, _mo_order.original, _managementobject.target_workload, _managementobject, _mo_order.managementobjectsnapshot, _low, _high, true, (bool)_mo_order.firedrill);
                        _count++;
                    }

                    MRMPServiceBase._mrmp_api.task().progress(_mrmp_task.id, String.Format("Successfully migrated group"), 99);
                    MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id);
                }
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
            }

        }
    }
}

