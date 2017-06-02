using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using MRMPService.Modules.MCP;
using System;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public void StopPreviewCG(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;

            try
            {
                MCP_Platform.StopPreviewCG(_task, _platform, _managementobject, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

