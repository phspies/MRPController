using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using MRMPService.Tasks.MCP;
using System;
using System.Collections.Generic;

namespace MRMPService.PortalTasks
{
    partial class DRSMCP
    {
        static public void SetupCG(MRPTaskType _mrmp_task)
        {
            MRPTaskDetailType _payload = _mrmp_task.taskdetail;
            List<MRPWorkloadPairType> _workload_pairs = _payload.workloadpairs;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
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
                            _start_progress = _increment * (_index - 1 );
                            _end_progress = _increment * (_index) - 1;
                        }
                        MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _vm_pair.target_workload, _protectiongroup, _start_progress, _end_progress, false);
                    }
                    MCP_Platform.CreateCG(_mrmp_task.id, _platform, _protectiongroup, _managementobject, _workload_pairs, 51, 99);
                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

