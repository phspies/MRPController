using DD.CBU.Compute.Api.Client;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Linq;
using DD.CBU.Compute.Api.Contracts.Requests.Drs;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;

namespace MRMPService.Modules.MCP
{
    partial class MCP_Platform
    {
        public static void SetupFailoverCG(MRPTaskType _task, MRPPlatformType _platform, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, List<MRPWorkloadPairType> _workloadpairs, MRPManagementobjectSnapshotType _snapshot, float _start_progress, float _end_progress)
        {
            //Get workload object from portal to perform updates once provisioned

            _task.progress(String.Format("Starting Consistency Group failover process"), ReportProgress.Progress(_start_progress, _end_progress, 1));

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.decrypted_password));
            CaaS.Login().Wait();

            //test to see if the workload exists in the platform
            var _filter = new ConsistencyGroupListOptions();
            _filter.Id = new Guid(_managementobject.moid);
            var _cg_status = CaaS.ConsistencyGroups.GetConsistencyGroups(_filter).Result;
            ConsistencyGroupType _mcp_cg = _cg_status.First();
            if (_mcp_cg != null)
            {
                if (_mcp_cg.state == "NORMAL" && _mcp_cg.operationStatus == "DRS_MODE")
                {
                    _task.progress(String.Format("Consistency Group and Infrastructure is healthy"), ReportProgress.Progress(_start_progress, _end_progress, 10));
                    _task.progress(String.Format("Starting Consistency Group preview"), ReportProgress.Progress(_start_progress, _end_progress, 15));
                    var _preview_status = CaaS.ConsistencyGroups.StartPreviewSnapshot(new StartPreviewSnapshotType() { consistencyGroupId = _managementobject.moid, snapshotId = Int64.Parse(_snapshot.snapshotmoid) }).Result;
                    if (_preview_status.responseCode == "IN_PROGRESS")
                    {
                        _task.progress(String.Format("{0}", _preview_status.message), ReportProgress.Progress(_start_progress, _end_progress, 15));
                        _task.progress(String.Format("Waiting for preview mode task to complete"), ReportProgress.Progress(_start_progress, _end_progress, 16));
                        while (true)
                        {
                            _mcp_cg = (CaaS.ConsistencyGroups.GetConsistencyGroups(_filter).Result).First();
                            if (_mcp_cg.operationStatus == "PREVIEWING_SNAPSHOT")
                            {
                                _task.progress(String.Format("Preview mode task completed"), ReportProgress.Progress(_start_progress, _end_progress, 16));
                                break;
                            }
                            else
                            {
                                if (_mcp_cg.progress != null)
                                {
                                    if (_mcp_cg.progress.step != null)
                                    {
                                        _task.progress(String.Format("{0}", _mcp_cg.progress.step.name), ReportProgress.Progress(_start_progress, _end_progress, _mcp_cg.progress.step.number + 16));
                                    }
                                }
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(5));
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
            _task.progress("Successfully configured preview mode for consistency group", 99);
            _task.successcomplete();
        }
    }
}
