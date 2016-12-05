using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;

namespace MRMPService.PortalTasks
{
    partial class Common
    {
        static public async void StartDoubleTakeJob(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                ModuleCommon.StartJob(_mrmp_task.id, _target_workload, _managementobject, 1, 100);

            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

