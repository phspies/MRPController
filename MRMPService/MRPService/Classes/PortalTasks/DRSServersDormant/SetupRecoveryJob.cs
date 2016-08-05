﻿using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRMPService.Tasks.MCP;

namespace MRMPService.PortalTasks
{
    partial class DRSServersDormant
    {
        static public void SetupRecoveryJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _source_workload = _payload.repository;
            MRPWorkloadType _target_workload = _payload.target;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPPlatformType _platform = _payload.platform;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    MCP_Platform.ProvisionVM(_mrmp_task.id, _platform, _target_workload, _protectiongroup, 1, 33, true);
                    Deploy.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 34, 65);
                    DisasterRecovery.CreateDRServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 66, 99);

                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfully configured recovery job");
                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);
                }
            }
        }
    }
}

