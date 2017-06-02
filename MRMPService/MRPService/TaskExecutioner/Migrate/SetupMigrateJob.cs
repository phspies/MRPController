using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.DoubleTake.Common;
using System;
using MRMPService.Modules.MCP;
using MRMPService.Modules.DoubleTake.Move;
using MRMPService.MRMPService.Log;

namespace MRMPService.TaskExecutioner.Migrate
{
    partial class Migrate
    {
        static public void SetupMigrateJob(MRPTaskType _task)
        {
            MRPTaskDetailType _payload = _task.taskdetail;
            MRPWorkloadType _source_workload = _payload.source_workload;
            MRPWorkloadType _target_workload = _payload.target_workload;
            MRPPlatformType _platform = _payload.target_platform;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPManagementobjectType _managementobject = _payload.managementobject;

            try
            {
                if (_target_workload.provisioned == false)
                {
                    MCP_Platform.ProvisionVM(_task, _platform, _target_workload, _protectiongroup, 1, 33, true);
                    //refresh source and target workload objects from portal
                    _source_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id);
                    _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);

                    if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                    {
                        ModuleCommon.DeployWindowsDoubleTake(_task, _source_workload, _target_workload, 34, 66);
                    }
                    else if (_source_workload.ostype.ToLower() == "unix" && _target_workload.ostype.ToLower() == "unix")
                    {
                        ModuleCommon.DeployLinuxDoubleTake(_task, _source_workload, _target_workload, 34, 66);
                    }
                    Migration.CreateServerMigrationJob(_task, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99);
                }
                else
                {
                    //refresh source and target workload objects from portal
                    _source_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id);
                    _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);

                    if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                    {
                        ModuleCommon.DeployWindowsDoubleTake(_task, _source_workload, _target_workload, 1, 50);
                    }
                    else if (_source_workload.ostype.ToLower() == "unix" && _target_workload.ostype.ToLower() == "unix")
                    {
                        ModuleCommon.DeployLinuxDoubleTake(_task, _source_workload, _target_workload, 1, 50);
                    }
                    Migration.CreateServerMigrationJob(_task, _source_workload, _target_workload, _protectiongroup, _managementobject, 51, 99);
                }
                _task.successcomplete("Successfully configured migration job");
            }
            catch (Exception ex)
            {
                Logger.log(ex.ToString(), Logger.Severity.Fatal);
                _task.failcomplete(ex.GetBaseException().Message);
            }
        }
    }
}

