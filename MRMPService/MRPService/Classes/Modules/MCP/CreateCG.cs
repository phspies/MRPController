using DD.CBU.Compute.Api.Client;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Linq;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        public static async void CreateCG(String _task_id, MRPPlatformType _platform, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, List<MRPWorkloadPairType> _workloadpairs, float _start_progress, float _end_progress)
        {
            //Get workload object from portal to perform updates once provisioned
            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(_task_id, String.Format("Starting Consistency Group configuration process"), ReportProgress.Progress(_start_progress, _end_progress, 1));

                List<DrsServerPairType> _mcp_server_pairs = new List<DrsServerPairType>();

                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.encrypted_password));
                var _account = await CaaS.Login();

                try
                {
                    //test to see if the workload exists in the platform
                    foreach (var _workloadpair in _workloadpairs)
                    {
                        
                        var _workloads = new List<MRPWorkloadType>() { _workloadpair.source_workload, _workloadpair.target_workload };
                        foreach (var _workload in _workloads)
                        {
                            int _workload_drs_attempt = 0;
                            var _progress = ReportProgress.Progress(_start_progress, _end_progress, _workloadpairs.IndexOf(_workloadpair) + _workloads.IndexOf(_workload));
                            while (true)
                            {
                                var _cass_server = await CaaS.ServerManagement.Server.GetServer(new Guid(_workload.moid));
                                if ((!(_cass_server.drsEligible is drsEligible)) && _cass_server.consistencyGroup == null)
                                {
                                    if (_workload_drs_attempt == 0)
                                    {
                                        _mrp_api.task().progress(_task_id, String.Format("Workload {0} not eligible for DRS. Setting meta information for workload", _workload.hostname), _progress + 2);
                                        try
                                        {
                                            var _meta_information = new editServerMetadata() { id = _workload.moid, drsEligible = true, drsEligibleSpecified = true };
                                            var _meta_result = await CaaS.ServerManagement.Server.EditServerMetadata(_meta_information);
                                            _mrp_api.task().progress(_task_id, String.Format("{0}", _meta_result.message), _progress + 3);
                                            _workload_drs_attempt += 1;
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(String.Format("Error setting workload meta information {0} : {1}", _workload.hostname, ex.GetBaseException().Message));
                                        }
                                    }
                                    else if (_workload_drs_attempt > 3)
                                    {
                                        throw new Exception(String.Format("Error setting workload meta information {0} after 3 tries", _workload.hostname));
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        _mcp_server_pairs.Add(new DrsServerPairType() { sourceServerId = _workloadpair.source_workload.moid, targetServerId = _workloadpair.target_workload.moid });
                        //deploy workload in platform
                    }

                    _mrp_api.task().progress(_task_id, String.Format("Creating consitency group with a total of {0} server pairs", _mcp_server_pairs.Count), ReportProgress.Progress(_start_progress, _end_progress, 90));
                    var _caas_consistency_group = new CreateConsistencyGroupType();
                    _caas_consistency_group.description = String.Format("{0} Consintency Group", _protectiongroup.group);
                    _caas_consistency_group.name = _protectiongroup.group;
                    _caas_consistency_group.journalSizeGb = (int)_protectiongroup.recoverypolicy.mcp_journal_size;
                    _caas_consistency_group.serverPair = _mcp_server_pairs.ToArray();
                    var _cg_create_response = await CaaS.ConsistencyGroups.CreateConsistencyGroup(_caas_consistency_group);

                    var _cg_moid = _cg_create_response.info.ToList().FirstOrDefault(x => x.name == "consistencyGroupId").value;

                    _mrp_api.task().progress(_task_id, String.Format("{0} : {1}", _cg_create_response.message, _cg_moid), 90);

                    _mrp_api.task().progress(_task_id, String.Format("Registering contency group {0} with portal", _cg_moid), ReportProgress.Progress(_start_progress, _end_progress, 91));
                    _mrp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _managementobject.id,
                        moid = _cg_moid,
                        moname = _protectiongroup.group,
                        motype = "MCPConsistencyGroup"
                    });
                    _mrp_api.task().successcomplete(_task_id, "Successfully configured protection group");

                }
                catch (Exception e)
                {
                    Logger.log(e.ToString(), Logger.Severity.Error);
                    throw new Exception(e.GetBaseException().Message);
                }
            }
        }
    }
}
