using DD.CBU.Compute.Api.Client;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Threading;
using MRMPService.MRMPService.Log;
using MRMPService.MRPService.Modules.MCP;

namespace MRMPService.Modules.MCP
{
    partial class MCP_Platform
    {
        public static void DestroyWorkload(MRPTaskType _task, MRPPlatformType _platform, MRMPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            try
            {
                ComputeApiClient _caas_object = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.decrypted_password));
                _caas_object.Login().Wait();

                _task.progress(String.Format("Powering down workload"), ReportProgress.Progress(_start_progress, _end_progress, 30));
                _caas_object.ServerManagement.Server.PowerOffServer(new Guid(_target_workload.moid));
                WorkloadWaitUntilComplete.Invoke(_caas_object, new Guid(_target_workload.moid));

                _task.progress(String.Format("Destroying workload"), ReportProgress.Progress(_start_progress, _end_progress, 60));
                _caas_object.ServerManagement.Server.DeleteServer(new Guid(_target_workload.moid));

            }
            catch (Exception ex)
            {
                _task.progress(String.Format("Error cleaning up workload {0} : {1}", _target_workload.hostname, ex.GetBaseException().Message), ReportProgress.Progress(_start_progress, _end_progress, 17));
            }
        }
    }
}
