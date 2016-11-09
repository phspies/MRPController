using MRMPService.MRMPService.Log;
using System;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPAPI;
using System.Collections.Generic;
using System.Linq;
using MRMPService.RP4VMAPI;
using MRMPService.RP4VMTypes;

namespace MRMPService.DTPollerCollection
{
    class RPGroupPoller : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RPGroupPoller()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public static void PollerDo(MRPManagementobjectType _mrp_managementobject)
        {
            //refresh managementobject from portal
            using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
            {
                _mrp_managementobject = _mrmp.managementobject().getmanagementobject_id(_mrp_managementobject.id);
            }

            //check for credentials
            MRPPlatformType _target_platform = _mrp_managementobject.target_platform;
            MRPCredentialType _platform_credential = _target_platform.credential;
            if (_platform_credential == null)
            {
                throw new ArgumentException(String.Format("RP4VM Group: Error finding credentials for appliance {0}", _target_platform.rp4vm_url));
            }

            //check for working IP

            Logger.log(String.Format("RP4VM Group: Start RP4vM collection for {0}", _target_platform.rp4vm_url), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            ConsistencyGroupSnapshots _group_snapshot_list = new ConsistencyGroupSnapshots();
            ConsistencyGroupStatistics _group_stats = new ConsistencyGroupStatistics();
            ConsistencyGroupState _group_info = new ConsistencyGroupState();

            bool _can_connect = false;
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);

                using (RP4VM_ApiClient _rp4vm = new RP4VM_ApiClient(_target_platform.rp4vm_url, username, _platform_credential.encrypted_password))
                {
                    ManagementSettings _info = _rp4vm.settings().getManagementSettings_Method();

                    _can_connect = true;

                    //now we try to get the snapshot information we have. IF we can't, then we know the group has been deleted...
                    _group_snapshot_list = _rp4vm.groups().getGroupSnapshots_Method(Int64.Parse(_mrp_managementobject.moid));
                    _group_stats = _rp4vm.groups().getGroupStatistics_Method(Int64.Parse(_mrp_managementobject.moid));
                    _group_info = _rp4vm.groups().getGroupState_Method(Int64.Parse(_mrp_managementobject.moid));

                }

            }
            catch (Exception ex)
            {
                //Mark the job as being unavailable because the system can't be reached
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    if (_can_connect)
                    {
                        Logger.log(String.Format("RP4VM Group: Error collecting information from {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);

                        _mrmp.managementobject().updatemanagementobject(new MRPManagementobjectType()
                        {
                            id = _mrp_managementobject.id,
                            internal_state = "deleted",
                        });
                    }
                    else
                    {
                        _mrmp.managementobject().updatemanagementobject(new MRPManagementobjectType()
                        {
                            id = _mrp_managementobject.id,
                            internal_state = "unavailable",
                            last_contact = DateTime.UtcNow
                        });
                    }
                }

                Logger.log(String.Format("RP4VM Group: Error contacting {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);
            }

            //now collect job information from target workload
            if (_can_connect)
            {
                MRPManagementobjectType _mrp_mo_update = new MRPManagementobjectType() { id = _mrp_managementobject.id };
                try
                {
                    using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                    {
                        foreach (Snapshot _rp_snapshot in _group_snapshot_list.copiesSnapshots.First().snapshots)
                        {
                            MRPManagementobjectSnapshotType _mrp_snapshot = new MRPManagementobjectSnapshotType();
                            if (_mrp_managementobject.managementobjectsnapshots_attributes.Exists(x => x.snapshotmoid == _rp_snapshot.snapshotUID.id.ToString()))
                            {
                                _mrp_snapshot.id = _mrp_managementobject.managementobjectsnapshots_attributes.FirstOrDefault(x => x.snapshotmoid == _rp_snapshot.snapshotUID.id.ToString()).id;
                            }
                            else
                            {
                                _mrp_snapshot.id = _mrp_managementobject.managementobjectsnapshots_attributes.FirstOrDefault(x => x.snapshotmoid == _rp_snapshot.snapshotUID.id.ToString()).id;
                                _mrp_snapshot._destroy = true;
                            }

                            _mrp_snapshot.snapshotmoid = _rp_snapshot.snapshotUID.id.ToString();
                            _mrp_snapshot.reason = _rp_snapshot.description;
                            _mrp_snapshot.state = _rp_snapshot.consolidationInfo.consolidationPolicy.ToString();
                            //_mrp_snapshot.timestamp = TimeSpan.FromMilliseconds(_rp_snapshot.closingTimeStamp.timeInMicroSeconds).;
                            _mrp_snapshot.comment = _rp_snapshot.userSnapshot == true ? "User Snapshot" : "System Snapshot";

                            if (_mrp_mo_update.managementobjectsnapshots_attributes == null)
                            {
                                _mrp_mo_update.managementobjectsnapshots_attributes = new List<MRPManagementobjectSnapshotType>();
                            }
                            _mrp_mo_update.managementobjectsnapshots_attributes.Add(_mrp_snapshot);
                        }
                        //Update job details
                        if (_group_stats != null)
                        {
                            MRPManagementobjectStatType _managedobject_stat = new MRPManagementobjectStatType();
                            {
                                if (_group_stats != null)
                                {
                                    var _link_stats = _group_stats.consistencyGroupLinkStatistics.First();
                                    _managedobject_stat.replication_bytes_sent = _link_stats.pipeStatistics.outgoingThroughput;
                                    _managedobject_stat.replication_bytes_sent_compressed = 0;
                                    _managedobject_stat.replication_disk_queue = _link_stats.pipeStatistics.lag.dataCounter;
                                    _managedobject_stat.replication_queue = _link_stats.pipeStatistics.lag.dataCounter;

                                    _managedobject_stat.replication_status = _link_stats.pipeStatistics.replicationMode.ToString();


                                    _mrp_mo_update.replication_status = _link_stats.pipeStatistics.replicationMode.ToString();

                                    _mrp_mo_update.mirror_status = "";
                                    _managedobject_stat.recovery_point_objective = Convert.ToDateTime(TimeSpan.FromMilliseconds(_link_stats.pipeStatistics.lag.timeCounter));
                                    _managedobject_stat.recovery_point_latency = _link_stats.pipeStatistics.lag.timeCounter;

                                    if (_mrp_mo_update.managementobjectstats_attributes == null)
                                    {
                                        _mrp_mo_update.managementobjectstats_attributes = new List<MRPManagementobjectStatType>();
                                    }
                                    _mrp_mo_update.managementobjectstats_attributes.Add(_managedobject_stat);
                                }
                                else
                                {
                                    _mrp_mo_update.replication_status = "";
                                    _mrp_mo_update.mirror_status = "";
                                }
                            }
                        }

                        _mrp_mo_update.state = _group_info.groupCopiesState.First().active ? "Active" : "Disabled";
                        _mrp_mo_update.internal_state = "active";
                        _mrp_mo_update.last_contact = DateTime.UtcNow;

                        _mrmp.managementobject().updatemanagementobject(_mrp_mo_update);
                    }
                }

                //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
                catch (Exception ex)
                {
                    Logger.log(String.Format("Double-Take Job : Error collecting job information for {0} on {1} : {2}", _mrp_managementobject.moid, _mrp_managementobject.target_workload, ex.ToString()), Logger.Severity.Info);
                    return;
                }
            }

            Logger.log(String.Format("Double-Take Job: Completed Double-Take collection for {0}", _target_platform.rp4vm_url), Logger.Severity.Info);
        }
    }
}

