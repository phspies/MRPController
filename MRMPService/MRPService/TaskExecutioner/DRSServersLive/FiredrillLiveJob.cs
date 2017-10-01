using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.DoubleTake.Common;
using MRMPService.Modules.MCP;
using System;
using MRMPService.Modules.DoubleTake.Availability;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.DRSServersLive
{
    partial class DRSServersLive
    {
        static public void FiredrillLiveJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRMPWorkloadBaseType _source_workload = _payload.source_workload;
            MRMPWorkloadBaseType _target_workload = _payload.target_workload;
            MRMPWorkloadBaseType _firedrill_workload = _payload.firedrill_workload;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            MRPPlatformType _platform = _firedrill_workload.platform;
            _source_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id);
            _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);


            try
            {
                if (!(bool)_firedrill_workload.provisioned)
                {
                    MCP_Platform.ProvisionVM(_task, _platform, _firedrill_workload, _protectiongroup, 1, 33, true);
                    _firedrill_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_firedrill_workload.id);
                }

                if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                {
                    ModuleCommon.DeployWindowsDoubleTake(_task, _target_workload, _firedrill_workload, 34, 66);
                    Availability.ConfigureHAFiredrillJobChain(_task, _source_workload, _target_workload, _firedrill_workload, _protectiongroup, _managementobject, 67, 99);
                }
                else
                {
                    ModuleCommon.DeployLinuxDoubleTake(_task, _target_workload, _firedrill_workload, 34, 66);
                    Availability.ConfigureHAFiredrillJobChain(_task, _source_workload, _target_workload, _firedrill_workload, _protectiongroup, _managementobject, 67, 99);
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

