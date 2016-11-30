using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.MCP;
using System;
using System.Threading.Tasks;

namespace MRMPService.PortalTasks
{
    partial class DRSMCP
    {
        static public void StopPreviewCG(MRPTaskType _mrmp_task)
        {
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
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

                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

