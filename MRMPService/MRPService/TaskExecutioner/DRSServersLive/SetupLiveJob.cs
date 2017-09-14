﻿using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Modules.DoubleTake.Common;
using MRMPService.Modules.MCP;
using System;
using MRMPService.Modules.DoubleTake.Availability;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;

namespace MRMPService.TaskExecutioner.DRSServersLive
{
    partial class DRSServersLive
    {
        static public void SetupLiveJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRMPWorkloadBaseType _source_workload = _payload.source_workload;
            MRMPWorkloadBaseType _target_workload = _payload.target_workload;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _payload.target_platform;
            _source_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id);
            _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);


            try
            {
                if (!(bool)_target_workload.provisioned)
                {
                    MCP_Platform.ProvisionVM(_task, _platform, _target_workload, _protectiongroup, 1, 33, true);
                    _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);
                }

                if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                {
                    ModuleCommon.DeployWindowsDoubleTake(_task, _source_workload, _target_workload, 34, 66);
                    Availability.CreateHAServerProtectionJob(_task, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Full_Failover);
                }
                else
                {
                    ModuleCommon.DeployLinuxDoubleTake(_task, _source_workload, _target_workload, 34, 66);
                    Availability.CreateHAServerProtectionJob(_task, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99, DT_JobTypes.HA_Linux_FullFailover);
                }
                _task.successcomplete("Successfully configured protection job");

            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.Message);
            }
        }
    }
}

