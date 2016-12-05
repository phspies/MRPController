using MRMPService.MRMPService.Log;
using System;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPAPI;
using System.Collections.Generic;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Drs;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Drs;
using System.Xml;
using System.Threading.Tasks;

namespace MRMPService.MCPCGCollection
{
    class MCPCGPoller
    {
        public static async Task PollerDo(MRPManagementobjectType _mrp_managementobject)
        {
            //refresh managementobject from portal

            _mrp_managementobject = await MRMPServiceBase._mrmp_api.managementobject().getmanagementobject_id(_mrp_managementobject.id);

            //check for credentials
            MRPPlatformType _target_platform = _mrp_managementobject.target_platform;
            MRPCredentialType _platform_credential = _target_platform.credential;
            if (_platform_credential == null)
            {
                throw new ArgumentException(String.Format("MCP CG: Error finding credentials for platform {0}", _target_platform.rp4vm_url));
            }

            //check for working IP

            Logger.log(String.Format("MCP CG: Start CG collection for {0}", _mrp_managementobject.moname), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            consistencyGroupSnapshots _group_snapshot_list = new consistencyGroupSnapshots();
            IEnumerable<ConsistencyGroupType> _group_stats = new List<ConsistencyGroupType>();

            bool _can_connect = false;
            try
            {
                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_mrp_managementobject.target_platform.url), new NetworkCredential(_mrp_managementobject.target_platform.credential.username, _mrp_managementobject.target_platform.credential.encrypted_password));

                var _account = await CaaS.Login();

                _can_connect = true;

                //now we try to get the snapshot information we have. IF we can't, then we know the group has been deleted...

                List<Filter> _filter = new List<Filter>();
                _filter.Add(new Filter() { Field = "consistencyGroupId", Operator = FilterOperator.Equals, Value = _mrp_managementobject.moid });
                _group_snapshot_list = await CaaS.ConsistencyGroups.GetConsistencyGroupSnapshots(new ConsistencyGroupSnapshotListOptions() { Filters = _filter });
                _group_stats = await CaaS.ConsistencyGroups.GetConsistencyGroups(new ConsistencyGroupListOptions() { Id = new Guid(_mrp_managementobject.moid) });
            }
            catch (Exception ex)
            {
                //Mark the job as being unavailable because the system can't be reached

                if (_can_connect)
                {
                    Logger.log(String.Format("MCP CG: Error collecting information from {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);

                    await MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _mrp_managementobject.id,
                        internal_state = "deleted",
                    });
                }
                else
                {
                    await MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _mrp_managementobject.id,
                        internal_state = "unavailable",
                        last_contact = DateTime.UtcNow
                    });
                }

                Logger.log(String.Format("MCP CG: Error contacting {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);
            }

            //now collect job information from target workload
            if (_can_connect)
            {
                MRPManagementobjectType _mrp_mo_update = new MRPManagementobjectType() { id = _mrp_managementobject.id };
                try
                {

                    _mrp_mo_update.managementobjectsnapshots = _mrp_managementobject.managementobjectsnapshots;
                    _mrp_mo_update.managementobjectsnapshots.ForEach(x => x._destroy = true);
                    if (_group_snapshot_list != null)
                    {
                        foreach (ConsistencyGroupSnapshotType _mcp_snapshot in _group_snapshot_list.snapshot)
                        {
                            MRPManagementobjectSnapshotType _mrp_snapshot = new MRPManagementobjectSnapshotType();
                            if (_mrp_mo_update.managementobjectsnapshots.Exists(x => x.snapshotmoid == _mcp_snapshot.id))
                            {
                                _mrp_snapshot = _mrp_mo_update.managementobjectsnapshots.FirstOrDefault(x => x.snapshotmoid == _mcp_snapshot.id);
                            }
                            else
                            {
                                _mrp_mo_update.managementobjectsnapshots.Add(_mrp_snapshot);
                            }

                            _mrp_snapshot.snapshotmoid = _mcp_snapshot.id;
                            _mrp_snapshot.state = _group_stats.First().state;
                            _mrp_snapshot.snapshot_size = _mcp_snapshot.sizeKb;
                            _mrp_snapshot.timestamp = DateTime.Parse(_mcp_snapshot.createTime);
                            _mrp_snapshot.comment = "";
                            _mrp_snapshot._destroy = false;

                        }
                    }

                    MRPManagementobjectStatType _managedobject_stat = new MRPManagementobjectStatType();
                    {
                        if (_group_snapshot_list != null)
                        {
                            _managedobject_stat.journal_size = _group_stats.First().journal.sizeGb;
                            _managedobject_stat.journal_used = _group_snapshot_list.journalUsageGb;
                            _managedobject_stat.replication_status = _group_stats.First().state;
                            _managedobject_stat.stransmit_mode = _group_stats.First().operationStatus;
                            TimeSpan _ts = XmlConvert.ToTimeSpan(_group_snapshot_list.protectionWindow);
                            _managedobject_stat.recovery_point_latency = _ts.Seconds;

                            if (_mrp_mo_update.managementobjectstats == null)
                            {
                                _mrp_mo_update.managementobjectstats = new List<MRPManagementobjectStatType>();
                            }
                            _mrp_mo_update.managementobjectstats.Add(_managedobject_stat);
                        }
                        else
                        {
                            _mrp_mo_update.replication_status = "";
                            _mrp_mo_update.mirror_status = "";
                        }
                    }

                    _mrp_mo_update.state = "Active";
                    _mrp_mo_update.internal_state = "active";
                    _mrp_mo_update.last_contact = DateTime.UtcNow;
                    await MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(_mrp_mo_update);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("MCP CG: Error collecting CG information for {0} on {1} : {2}", _mrp_managementobject.moid, _mrp_managementobject.target_platform.platform, ex.ToString()), Logger.Severity.Info);
                }
            }

            Logger.log(String.Format("MCP CG: Completed CG collection for {0}", _target_platform.platform), Logger.Severity.Info);
        }
    }
}

