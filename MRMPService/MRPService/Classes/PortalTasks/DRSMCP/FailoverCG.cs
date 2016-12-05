﻿using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRMPService.PortalTasks
{
    partial class DRSMCP
    {
        static public async void FailoverCG(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPManagementobjectSnapshotType _managementobjectsnapshot = _payload.managementobjectsnapshot;
            MRPPlatformType _platform = _payload.target_platform;

            try
            {
                Task _task = MCP_Platform.FailoverCG(_mrmp_task.id, _platform, _protectiongroup, _managementobject, _workload_pairs, _managementobjectsnapshot, 1, 100);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);

                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);

            }
        }
    }
}
