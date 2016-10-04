using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class Migrate
    {
        static public void SetupMigrateJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _source_workload = _payload.source;
            MRPWorkloadType _target_workload = _payload.target;
            MRPPlatformType _platform = _payload.platform;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    if (_target_workload.provisioned == false)
                    {
                        MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);
                        //refresh source and target workload objects from portal
                        _source_workload = _mrp_portal.workload().get_by_id(_source_workload.id);
                        _target_workload = _mrp_portal.workload().get_by_id(_target_workload.id);

                        if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                        {
                            ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                        }
                        else if (_source_workload.ostype.ToLower() == "unix" && _target_workload.ostype.ToLower() == "unix")
                        {
                            ModuleCommon.DeployLinuxDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 66);
                        }
                        Migration.CreateServerMigrationJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 67, 99);
                    }
                    else
                    {
                        //refresh source and target workload objects from portal
                        _source_workload = _mrp_portal.workload().get_by_id(_source_workload.id);
                        _target_workload = _mrp_portal.workload().get_by_id(_target_workload.id);

                        if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                        {
                            ModuleCommon.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 1, 50);
                        }
                        else if (_source_workload.ostype.ToLower() == "unix" && _target_workload.ostype.ToLower() == "unix")
                        {
                            ModuleCommon.DeployLinuxDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 1, 50);
                        }
                        Migration.CreateServerMigrationJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 51, 99);
                    }
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully configured migration job");
                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.GetBaseException().Message);

                }
            }
        }
    }
}

