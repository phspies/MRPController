using MRMPService.MRMPAPI;
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
        static public async void ApplyMetaInformation(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _source_platform = _payload.source_platform;

            MRPPlatformType _target_platform = _payload.target_platform;
            try
            {
                foreach (var _pair in _workload_pairs)
                {
                    int _index = _workload_pairs.IndexOf(_pair) + 1;
                    await MCP_Platform.ApplyMetaInformation(_mrmp_task.id, _workload_pairs, _source_platform, _target_platform, _pair.source_workload, _pair.target_workload, _index * 5, _index * 9);
                }
                await MRMPServiceBase._mrmp_api.task().successcomplete(_mrmp_task.id);

            }
            catch (Exception ex)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_mrmp_task.id, ex.Message);
                Logger.log(ex.ToString(), Logger.Severity.Fatal);

            }
        }
    }
}

