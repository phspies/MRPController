using DD.CBU.Compute.Api.Client;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Linq;
using DD.CBU.Compute.Api.Contracts.Requests.Drs;
using System.Threading.Tasks;
using System.Threading;
using MRMPService.MRMPAPI;

namespace MRMPService.Modules.MCP
{
    partial class MCP_Platform
    {
        public static async Task FailoverCG(String _task_id, MRPPlatformType _platform, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, List<MRPWorkloadPairType> _workloadpairs, MRPManagementobjectSnapshotType _snapshot, float _start_progress, float _end_progress)
        {
            //Get workload object from portal to perform updates once provisioned

            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Starting Consistency Group failover process"), ReportProgress.Progress(_start_progress, _end_progress, 1));

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.encrypted_password));
            var _account = await CaaS.Login();

            //test to see if the workload exists in the platform
            var _filter = new ConsistencyGroupListOptions();
            _filter.Id = new Guid(_managementobject.moid);
            var _cg_status = await CaaS.ConsistencyGroups.GetConsistencyGroups(_filter);
            ConsistencyGroupType _mcp_cg = _cg_status.First();
            if (_mcp_cg != null)
            {
                if (_mcp_cg.state == "NORMAL" && _mcp_cg.operationStatus == "PREVIEWING_SNAPSHOT" && _mcp_cg.drsInfrastructure.status == "ACTIVE")
                {
                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Consistency Group and Infrastructure is healthy"), ReportProgress.Progress(_start_progress, _end_progress, 10));
                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Starting Consistency Group failover"), ReportProgress.Progress(_start_progress, _end_progress, 15));
                    var _preview_status = await CaaS.ConsistencyGroups.InitiateFailoverForConsistencyGroup(new InitiateFailoverType() { consistencyGroupId = _managementobject.moid });
                    if (_preview_status.responseCode == "IN_PROGRESS")
                    {
                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("{0}", _preview_status.message), ReportProgress.Progress(_start_progress, _end_progress, 15));
                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Waiting for failover task to complete"), ReportProgress.Progress(_start_progress, _end_progress, 16));
                        while (true)
                        {
                            try
                            {
                                _mcp_cg = (await CaaS.ConsistencyGroups.GetConsistencyGroups(_filter)).First();
                            }
                            catch (Exception)
                            {
                                break;
                            }
                            if (_mcp_cg.progress != null)
                            {
                                if (_mcp_cg.progress.step != null)
                                {
                                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("{0}", _mcp_cg.progress.step.name), ReportProgress.Progress(_start_progress, _end_progress, _mcp_cg.progress.step.number + 16));
                                }
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                        foreach (var _workload_pair in _workloadpairs)
                        {
                            await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Updating network settings for {0}", _workload_pair.source_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _workloadpairs.IndexOf(_workload_pair) + 20));

                            try
                            {
                                UpdateIP.setInterfaceAddresses(_workload_pair.source_workload, _workload_pair.target_workload);
                                var _workable_ip = "";
                                using (Connection _connection = new Connection())
                                {
                                    _workable_ip = _connection.FindConnection(_workload_pair.target_workload.iplist, false);
                                }
                                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Successfully updated network settings for {0}", _workload_pair.source_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _workloadpairs.IndexOf(_workload_pair) + 22));
                            }
                            catch (Exception ex)
                            {
                                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Could not update network configuration on {0} : {1}", _workload_pair.source_workload.hostname, ex.GetBaseException().Message), ReportProgress.Progress(_start_progress, _end_progress, _workloadpairs.IndexOf(_workload_pair) + 21));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("{0}", _preview_status.message));
                    }
                }
                else
                {
                    throw new Exception(String.Format("Protection group not in ready state [State: {0}, Operation State: {1}, Instrastructure State: {2}", _mcp_cg.state, _mcp_cg.operationStatus, _mcp_cg.drsInfrastructure.status));
                }
            }
            else
            {
                throw new Exception(String.Format("Could not find consistency group {0}", _managementobject.moid));
            }
            await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Successfully configured failed over consistency group", 99);
            await MRMPServiceBase._mrmp_api.task().successcomplete(_task_id);
        }
    }
}
