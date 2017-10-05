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
        public static void DestroyWorkload(MRPTaskType _task, MRMPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            try
            {
                ComputeApiClient _caas_object = ComputeApiClient.GetComputeApiClient(new Uri(_target_workload.platform.url), new NetworkCredential(_target_workload.platform.credential.username, _target_workload.platform.credential.decrypted_password));
                _caas_object.Login().Wait();

                _task.progress(String.Format("Powering down workload"), ReportProgress.Progress(_start_progress, _end_progress, 30));
                _caas_object.ServerManagement.Server.PowerOffServer(new Guid(_target_workload.moid));
                WorkloadWaitUntilComplete.Invoke(_caas_object, new Guid(_target_workload.moid));

                _task.progress(String.Format("Destroying workload"), ReportProgress.Progress(_start_progress, _end_progress, 60));
                _caas_object.ServerManagement.Server.DeleteServer(new Guid(_target_workload.moid));

            }
            catch (Exception _ex)
            {
                Logger.log($"Error removing workload : {_ex.ToString()}", Logger.Severity.Fatal);
                throw;
            }
        }
    }
}
