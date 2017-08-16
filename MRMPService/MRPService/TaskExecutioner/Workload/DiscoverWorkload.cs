using MRMPService.Modules.MRMPPortal.Contracts;

using System;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;

namespace MRMPService.TaskExecutioner.Workload
{
    partial class Workload
    {
        static public void DiscoverWorkload(MRPTaskType _task)
        {
            MRMPWorkloadBaseType _target_workload = _task.taskdetail.target_workload;

            try
            {
                switch (_target_workload.ostype.ToUpper())
                {
                    case "UNIX":
                        WorkloadInventory.WorkloadInventoryLinuxDo(_target_workload);
                        break;
                    case "WINDOWS":
                        WorkloadInventory.WorkloadInventoryWindowsDo(_target_workload);
                        break;
                }
                _task.successcomplete("Successfully discovered workload");
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.GetBaseException().Message);
            }
        }
    }
}

