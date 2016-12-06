using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.Modules.MCP;
using System;
using System.Threading.Tasks;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public async void StopPreviewCG(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;

            try
            {
                Task _task = MCP_Platform.StopPreviewCG(_mrmp_task.id, _platform, _managementobject, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

