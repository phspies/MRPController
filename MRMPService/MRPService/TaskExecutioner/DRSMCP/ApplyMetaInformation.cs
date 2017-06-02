using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.Modules.MCP;
using System;
using System.Collections.Generic;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public async void ApplyMetaInformation(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
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
                    await MCP_Platform.ApplyMetaInformation(_task, _workload_pairs, _source_platform, _target_platform, _pair.source_workload, _pair.target_workload, _index * 5, _index * 9);
                }
                _task.successcomplete();

            }
            catch (Exception ex)
            {
                _task.failcomplete(ex.Message);
                Logger.log(ex.ToString(), Logger.Severity.Fatal);

            }
        }
    }
}

