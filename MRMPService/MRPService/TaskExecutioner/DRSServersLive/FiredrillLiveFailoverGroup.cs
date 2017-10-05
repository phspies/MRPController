using MRMPService.Modules.DoubleTake.Availability;
using MRMPService.Modules.DoubleTake.Common;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System;
using System.Linq;

namespace MRMPService.TaskExecutioner.DRSServersLive
{
    partial class DRSServersLive
    {
        static public void FiredrillLiveFailoverGroup(MRPTaskType _task)
        {
            
            try
            {
                MRPTaskDetailType _payload = _task.taskdetail;
                if (_payload.managementobjects.Count() == 0)
                {

                    _task.progress(String.Format("No failover operations found"), 99);
                    _task.successcomplete(_task.id);
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
                        Availability.ConfigureHAFiredrillJobChain(_task, _mo_order.target_workload, _mo_order.firedrill_workload, _managementobject, 10, 50);
                        Availability.FailoverHAFiredrillJobChain(_task, _managementobject.source_workload, _managementobject.target_workload, _mo_order.firedrill_workload, _managementobject, 55, 90);
                        _count++;
                    }

                    _task.progress(String.Format("Successfully migrated group"), 99);
                    _task.successcomplete(_task.id);
                }
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }

        }
    }
}

