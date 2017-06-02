using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;

using MRMPService.Modules.MCP;
using System;
using System.Collections.Generic;

namespace MRMPService.TaskExecutioner.DRSMCP
{
    partial class DRSMCP
    {
        static public void SetupCG(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;

            try
            {
                float _increment = 50 / _workload_pairs.Count;
                foreach (var _vm_pair in _payload.workloadpairs)
                {
                    float _start_progress = 0;
                    float _end_progress = 0;
                    var _index = _workload_pairs.IndexOf(_vm_pair) + 1;
                    if (_index == 1)
                    {
                        _start_progress = 1;
                        _end_progress = _increment - 1;
                    }
                    else
                    {
                        _start_progress = _increment * (_index - 1);
                        _end_progress = _increment * (_index) - 1;
                    }
                    MCP_Platform.ProvisionVM(_task, _platform, _vm_pair.target_workload, _protectiongroup, _start_progress, _end_progress, false);
                }
                MCP_Platform.CreateCG(_task, _platform, _protectiongroup, _managementobject, _workload_pairs, 51, 99);
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

