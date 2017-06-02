using DD.CBU.Compute.Api.Client;
using MRMPService.MRMPService.Log;
using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Linq;
using DD.CBU.Compute.Api.Contracts.Requests.Drs;
using System.Threading;
using System.Threading.Tasks;
using MRMPService.Scheduler.MCPCGCollection;

namespace MRMPService.Modules.MCP
{
    partial class MCP_Platform
    {
        public static void CreateCG(MRPTaskType _task, MRPPlatformType _platform, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, List<MRPWorkloadPairType> _workloadpairs, float _start_progress, float _end_progress)
        {
            //Get workload object from portal to perform updates once provisioned

            _task.progress(String.Format("Starting Consistency Group configuration process"), ReportProgress.Progress(_start_progress, _end_progress, 1));

            List<DrsServerPairType> _mcp_server_pairs = new List<DrsServerPairType>();

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.decrypted_password));
            CaaS.Login().Wait();

            bool _deploy_cg = false;
            if (_managementobject.moid == null)
            {
                _deploy_cg = true;
            }
            else
            {
                ConsistencyGroupListOptions _filter = new ConsistencyGroupListOptions();
                _filter.Id = Guid.Parse(_managementobject.moid);
                IEnumerable<ConsistencyGroupType> _current_cgs = CaaS.ConsistencyGroups.GetConsistencyGroups(_filter).Result;
                if (_current_cgs != null)
                {
                    ConsistencyGroupType _current_cg = _current_cgs.First();
                    var _mcp_source_difference = _current_cg.serverPair.Where(p => !_workloadpairs.Any(l => p.sourceServer.id == l.source_workload.moid));
                    var _mcp_target_difference = _current_cg.serverPair.Where(p => !_workloadpairs.Any(l => p.targetServer.id == l.target_workload.moid));
                    var _mrmp_source_difference = _workloadpairs.Where(p => !_current_cg.serverPair.Any(l => p.target_workload.moid == l.targetServer.id));
                    var _mrmp_target_difference = _workloadpairs.Where(p => !_current_cg.serverPair.Any(l => p.target_workload.moid == l.targetServer.id));
                    if (_mcp_source_difference.Count() != 0 || _mcp_target_difference.Count() != 0 || _mrmp_source_difference.Count() != 0 || _mrmp_target_difference.Count() != 0)
                    {
                        _task.progress(String.Format("Consistency group changed, removing and recreating consistency group"), ReportProgress.Progress(_start_progress, _end_progress, 2));
                        var _delete_result = CaaS.ConsistencyGroups.DeleteConsistencyGroup(new DeleteConsistencyGroupType() { id = _managementobject.moid }).Result;
                        if (_delete_result.responseCode == "IN_PROGRESS")
                        {
                            _task.progress(_delete_result.message, 3);

                            while (true)
                            {
                                try
                                {
                                    _current_cgs = CaaS.ConsistencyGroups.GetConsistencyGroups(_filter).Result;
                                    if (_current_cgs != null)
                                    {
                                        Thread.Sleep(TimeSpan.FromSeconds(5));
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                            }
                            _task.progress("Consistency group successfully removed", ReportProgress.Progress(_start_progress, _end_progress, 4));
                            _deploy_cg = true;

                        }
                        else
                        {
                            throw new Exception(String.Format("Error removing consistency group {0}", _delete_result.message));
                        }
                    }
                    else
                    {
                        _task.progress("Consistency group remains unchanged", 99);
                    }
                }
                else
                {
                    _task.progress("Consistency group does not exist in platform", ReportProgress.Progress(_start_progress, _end_progress, 4));
                    _deploy_cg = true;
                }
            }
            if (_deploy_cg)
            {
                try
                {
                    //test to see if the workload exists in the platform
                    foreach (var _workloadpair in _workloadpairs)
                    {

                        var _workloads = new List<MRPWorkloadType>() { _workloadpair.source_workload, _workloadpair.target_workload };


                        //get new copy of workload from portal
                        MRPWorkloadType _updated_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_workloadpair.source_workload.id);

                        var _cass_server = CaaS.ServerManagement.Server.GetServer(new Guid(_updated_workload.moid)).Result;
                        var _progress = ReportProgress.Progress(_start_progress, _end_progress, _workloadpairs.IndexOf(_workloadpair) + _workloads.IndexOf(_updated_workload));

                        if (!_cass_server.started)
                        {
                            var _cass_server_start = CaaS.ServerManagement.Server.StartServer(new Guid(_updated_workload.moid)).Result;
                            if (_cass_server_start.responseCode == "IN_PROGRESS")
                            {
                                _task.progress(String.Format("Workload {0} stared", _updated_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _progress + 6));
                            }
                            else
                            {
                                throw new Exception(_cass_server_start.message);
                            }
                        }
                        foreach (var _workload in _workloads)
                        {
                            //get new copy of workload from portal
                            _updated_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_workload.id);

                            int _workload_drs_attempt = 0;
                            while (true)
                            {
                                _cass_server = CaaS.ServerManagement.Server.GetServer(new Guid(_updated_workload.moid)).Result;
                                if ((!(_cass_server.drsEligible is drsEligible)) && _cass_server.consistencyGroup == null)
                                {
                                    if (_workload_drs_attempt == 0)
                                    {
                                        _task.progress(String.Format("Workload {0} not eligible for DRS. Setting meta information for workload", _updated_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _progress + 5));
                                        try
                                        {
                                            var _meta_information = new editServerMetadata() { id = _updated_workload.moid, drsEligible = true, drsEligibleSpecified = true };
                                            var _meta_result = CaaS.ServerManagement.Server.EditServerMetadata(_meta_information).Result;
                                            _task.progress(String.Format("{0}", _meta_result.message), _progress + 10);
                                            _workload_drs_attempt += 1;
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(String.Format("Error setting workload meta information {0} : {1}", _updated_workload.hostname, ex.GetBaseException().Message));
                                        }
                                    }
                                    else if (_workload_drs_attempt > 3)
                                    {
                                        throw new Exception(String.Format("Error setting workload meta information {0} after 3 tries", _updated_workload.hostname));
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //pull latest version of workloads before we add them to the DRS pairs
                        var _source_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_workloadpair.source_workload.id);
                        var _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_workloadpair.target_workload.id);
                        _mcp_server_pairs.Add(new DrsServerPairType() { sourceServerId = _source_workload.moid, targetServerId = _target_workload.moid });
                        //deploy workload in platform
                    }
                    //sleep a bit to let the servers complete their starts if they were actioned.
                    Thread.Sleep(TimeSpan.FromSeconds(30));

                    _task.progress(String.Format("Creating consitency group with a total of {0} server pairs", _mcp_server_pairs.Count), ReportProgress.Progress(_start_progress, _end_progress, 50));
                    var _caas_consistency_group = new CreateConsistencyGroupType();
                    _caas_consistency_group.description = String.Format("{0} Consintency Group", _protectiongroup.group);
                    _caas_consistency_group.name = _protectiongroup.group;
                    _caas_consistency_group.journalSizeGb = (int)_protectiongroup.recoverypolicy.mcp_journal_size;
                    _caas_consistency_group.serverPair = _mcp_server_pairs.ToArray();
                    var _cg_create_response = CaaS.ConsistencyGroups.CreateConsistencyGroup(_caas_consistency_group).Result;

                    var _cg_moid = _cg_create_response.info.ToList().FirstOrDefault(x => x.name == "consistencyGroupId").value;

                    _task.progress(String.Format("{0} : {1}", _cg_create_response.message, _cg_moid), 51);

                    var _cg_status = (CaaS.ConsistencyGroups.GetConsistencyGroups(new ConsistencyGroupListOptions() { Id = new Guid(_cg_moid) }).Result).First();
                    while (_cg_status.state != "NORMAL")
                    {
                        if (_cg_status.state.Contains("FAILED") || _cg_status.state.Contains("SUPPORT"))
                        {
                            throw new Exception(_cg_status.progress.failureReason);
                        }
                        if (_cg_status.progress.step != null)
                        {
                            _task.progress(_cg_status.progress.step.name, ReportProgress.Progress(_start_progress, _end_progress, 55 + _cg_status.progress.step.number));
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        _cg_status = (CaaS.ConsistencyGroups.GetConsistencyGroups(new ConsistencyGroupListOptions() { Id = new Guid(_cg_moid) }).Result).First();
                    }

                    _task.progress(String.Format("Registering contency group {0} with portal", _cg_moid), ReportProgress.Progress(_start_progress, _end_progress, 91));
                    MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _managementobject.id,
                        moid = _cg_moid,
                        moname = _protectiongroup.group,
                        motype = "MCPConsistencyGroup"
                    });
                    _task.progress("Successfully configured protection group", 99);
                    MCPCGPoller.PollerDo(new MRPManagementobjectType()
                    {
                        id = _managementobject.id,
                        moid = _cg_moid,
                        moname = _protectiongroup.group,
                        motype = "MCPConsistencyGroup"
                    });

                }
                catch (Exception e)
                {
                    Logger.log(e.ToString(), Logger.Severity.Error);
                    throw new Exception(e.GetBaseException().Message);
                }
            }
            _task.successcomplete();

        }
    }
}
