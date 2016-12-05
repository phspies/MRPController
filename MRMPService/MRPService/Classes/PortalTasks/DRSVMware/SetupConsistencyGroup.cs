﻿using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;
using MRMPService.Tasks.RP4VM;
using System.Linq;
using MRMPService.MRMPAPI;

namespace MRMPService.PortalTasks
{
    partial class DRSVMWare
    {
        static public async void SetupConsistencyGroup(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            MRPPlatformType _source_platform = _payload.source_platform;
            MRPPlatformType _target_platform = _payload.target_platform;

            MRPManagementobjectType _mo = _payload.managementobject;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;

            try
            {
                await RP4VM.CreateConsistencyGroup(_mrmp_task.id, _payload.workloadpairs.Select(x => x.source_workload).ToList(), _protectiongroup, _mo, 1, 99);
                await MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id, "Successfully configured consistency group");
            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}

