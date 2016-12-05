using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using System;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;

namespace MRMPService.PortalTasks
{
    partial class Workload
    {
        static public async void DiscoverWorkload(MRPTaskType _mrmp_task)
        {
            MRPWorkloadType _target_workload = _mrmp_task.taskdetail.target_workload;

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
                await MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, "Successfully discovered workload");
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
            }
        }
    }
}

