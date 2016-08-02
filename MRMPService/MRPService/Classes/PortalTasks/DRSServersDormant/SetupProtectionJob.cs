using DoubleTake.Web.Models;
using MRMPService.DoubleTake;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPService.Types.API;
using MRMPService.Tasks.DoubleTake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.PortalTasks
{
    partial class DRSServersDormant
    {
        static public void SetupProtectionJob(MRPTaskType _mrmp_task)
        {
            MRPTaskSubmitpayloadType _payload = _mrmp_task.submitpayload;
            MRPWorkloadType _source_workload = _payload.source;
            MRPWorkloadType _target_workload = _payload.repository;
            MRPRecoverypolicyType _recovery_policy = _payload.protectiongroup.recoverypolicy;
            MRPProtectiongroupType _protectiongroup = _payload.protectiongroup;
            MRPProtectiongrouptreeType _protectiongrouptree = _payload.protectiongrouptree;
            MRPManagementobjectType _managementobject = _payload.managementobject;
            using (MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {
                    if (_source_workload.ostype.ToLower() == "windows" && _target_workload.ostype.ToLower() == "windows")
                    {
                        Deploy.DeployWindowsDoubleTake(_mrmp_task.id, _source_workload, _target_workload, 1, 25);
                        DisasterRecovery.CreateDRServerProtectionJob(_mrmp_task.id, _source_workload, _target_workload, _protectiongroup, _managementobject, 26, 50);
                    }
                    _mrp_portal.task().successcomplete(_mrmp_task.id, "Successfull configured protection job");

                }
                catch (Exception ex)
                {
                    _mrp_portal.task().failcomplete(_mrmp_task.id, ex.Message);

                }
            }
        }
    }
}

