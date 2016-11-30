using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using System;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPService.Log;

namespace MRMPService.PortalTasks
{
    partial class Workload
    {
        static public async void DiscoverWorkload(MRPTaskType _mrmp_task)
        {
            MRPWorkloadType _target_workload = _mrmp_task.taskdetail.target_workload;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    switch (_target_workload.ostype.ToUpper())
                    {
                        case "UNIX":
                            await WorkloadInventory.WorkloadInventoryLinuxDo(_target_workload);
                            break;
                        case "WINDOWS":
                            await WorkloadInventory.WorkloadInventoryWindowsDo(_target_workload);
                            break;
                    }
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully discovered workload");
                }
                catch (Exception ex)
                {
                    Logger.log(ex.ToString(), Logger.Severity.Fatal);
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);
                }
            }
        }
    }
}

