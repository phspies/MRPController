using MRMPService.MRMPService.Log;
using System;
using MRMPService.Utilities;
using MRMPService.MRMPAPI.Types.API;
using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using System.Collections.Generic;
using System.Linq;
using MRMPService.MRMPDoubleTake;

namespace MRMPService.DTPollerCollection
{
    class DTJobPoller
    {
        public static void PollerDo(MRPManagementobjectType _mrp_managementobject)
        {
            //check for credentials
            MRPWorkloadType _target_workload = _mrp_managementobject.target_workload;
            MRPCredentialType _credential = _target_workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error finding credentials for workload {0} {1}", _mrp_managementobject.target_workload.id, _mrp_managementobject.target_workload.hostname));
            }

            //check for working IP
            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error contacting workload for workload {0}", _target_workload.hostname));
            }
            Logger.log(String.Format("Double-Take Job: Start Double-Take collection for {0} using {1}", _mrp_managementobject.target_workload.hostname, workload_ip), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            JobInfoModel _dt_job = new JobInfoModel();
            IEnumerable<ImageInfoModel> _dt_image_list = new List<ImageInfoModel>();
            IEnumerable<SnapshotEntryModel> _dt_snapshot_list = new List<SnapshotEntryModel>();

            bool _can_connect = false;
            try
            {
                using (Doubletake _dt = new Doubletake(null, _target_workload))
                {
                    ProductInfoModel _info = _dt.management().GetProductInfo().Result;
                    _dt_job = _dt.job().GetJob((Guid)_mrp_managementobject.moid).Result;
                    if (_dt_job.JobType == DT_JobTypes.DR_Data_Protection || _dt_job.JobType == DT_JobTypes.DR_Full_Protection)
                    {
                        _dt_image_list = _dt.image().GetImages(_dt_job.Options.Name).Result;
                    }
                    else if (_dt_job.JobType == DT_JobTypes.HA_Full_Failover || _dt_job.JobType == DT_JobTypes.HA_FilesFolders)
                    {
                        _dt_snapshot_list = _dt.job().GetSnapShots(_dt_job.Id);                        
                    }
                    //now we try to get the job information we have. IF we can't, then we know the job has been deleted...
                    _can_connect = true;

                }
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _mrmp.workload().DoubleTakeUpdateStatus(_target_workload, "Success", true);
                }

            }
            catch (Exception ex)
            {
                //Mark the job as being unavailable because the system can't be reached
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    if (_can_connect)
                    {
                        Logger.log(String.Format("Double-Take Job: Error collecting job information from {0} using {1} : {2}", _mrp_managementobject.target_workload, workload_ip, ex.ToString()), Logger.Severity.Info);

                        _mrmp.managementobject().updatemanagementobject(new MRPManagementobjectType()
                        {
                            id = _mrp_managementobject.id,
                            internal_state = "deleted",
                        });
                    }
                    else
                    {
                        _mrmp.workload().DoubleTakeUpdateStatus(_target_workload, ex.Message, false);
                    }
                }

                Logger.log(String.Format("Double-Take Job: Error contacting Double-Take Management Service for {0} using {1} : {2}", _mrp_managementobject.target_workload, workload_ip, ex.ToString()), Logger.Severity.Info);
                //throw new ArgumentException(String.Format("Double-Take Job: Error contacting Double-Take management service for {0} using {1}", _mrp_managementobject.target_workload, workload_ip));
            }

            //now collect job information from target workload
            MRPManagementobjectType _mrp_mo_update = new MRPManagementobjectType() { id = _mrp_managementobject.id };
            try
            {
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    foreach (ImageInfoModel _dt_image in _dt_image_list)
                    {
                        foreach (SnapshotEntryModel _dt_snap in _dt_image.Snapshots.Where(x => x.States == TargetStates.Good))
                        {
                            MRPManagementobjectSnapshotType _mrp_snapshot = new MRPManagementobjectSnapshotType();
                            if (_mrp_managementobject.managementobjectsnapshot_attributes.Exists(x => x.imagemoid == _dt_snap.Id))
                            {
                                _mrp_snapshot.id = _mrp_managementobject.managementobjectsnapshot_attributes.FirstOrDefault(x => x.imagemoid == _dt_snap.Id).id;
                            }
                                                   
                            _mrp_snapshot.imagemoid = _dt_snap.Id;
                            _mrp_snapshot.reason = _dt_snap.Reason.ToString();
                            _mrp_snapshot.state = _dt_snap.States;
                            _mrp_snapshot.timestamp = _dt_snap.Timestamp.UtcDateTime;
                            _mrp_snapshot.comment = _dt_snap.Comment;

                            if (_mrp_mo_update.managementobjectsnapshot_attributes == null)
                            {
                                _mrp_mo_update.managementobjectsnapshot_attributes = new List<MRPManagementobjectSnapshotType>();
                            }
                            _mrp_mo_update.managementobjectsnapshot_attributes.Add(_mrp_snapshot);
                        }
                    }
                    foreach (SnapshotEntryModel _dt_snap in _dt_snapshot_list.Where(x => x.States == TargetStates.Good))
                    {
                        MRPManagementobjectSnapshotType _mrp_snapshot = new MRPManagementobjectSnapshotType();
                        if (_mrp_managementobject.managementobjectsnapshot_attributes.Exists(x => x.imagemoid == _dt_snap.Id))
                        {
                            _mrp_snapshot.id = _mrp_managementobject.managementobjectsnapshot_attributes.FirstOrDefault(x => x.imagemoid == _dt_snap.Id).id;
                        }

                        _mrp_snapshot.imagemoid = _dt_snap.Id;
                        _mrp_snapshot.reason = _dt_snap.Reason.ToString();
                        _mrp_snapshot.state = _dt_snap.States;
                        _mrp_snapshot.timestamp = _dt_snap.Timestamp.UtcDateTime;
                        _mrp_snapshot.comment = _dt_snap.Comment;

                        if (_mrp_mo_update.managementobjectsnapshot_attributes == null)
                        {
                            _mrp_mo_update.managementobjectsnapshot_attributes = new List<MRPManagementobjectSnapshotType>();
                        }
                        _mrp_mo_update.managementobjectsnapshot_attributes.Add(_mrp_snapshot);
                    }
                    //Update job details
                    CoreConnectionDetailsModel _connection_details = _dt_job.Statistics.CoreConnectionDetails;
                    MRPManagementobjectStatType _managedobject_stat = new MRPManagementobjectStatType();
                    {
                        if (_connection_details != null)
                        {
                            _managedobject_stat.replication_bytes_sent = _connection_details.ReplicationBytesSent;
                            _managedobject_stat.replication_bytes_sent_compressed = _connection_details.ReplicationBytesTransmitted;
                            _managedobject_stat.replication_disk_queue = _connection_details.DiskQueueBytes;
                            _managedobject_stat.replication_queue = _connection_details.ReplicationBytesQueued;
                            _managedobject_stat.replication_status = _connection_details.ReplicationState.ToString();

                            _managedobject_stat.mirror_remaining = _connection_details.MirrorBytesRemaining;
                            _managedobject_stat.mirror_percent_complete = _connection_details.MirrorPermillage;
                            _managedobject_stat.mirror_skipped = _connection_details.MirrorBytesSkipped;
                            _managedobject_stat.mirror_status = _connection_details.MirrorState.ToString();
                            _managedobject_stat.stransmit_mode = _dt_job.Status.HighLevelState.ToString();
                            _managedobject_stat.recovery_point_objective = _connection_details.SourceRecoveryPointTime.UtcDateTime;
                            _managedobject_stat.recovery_point_latency = _connection_details.SourceRecoveryPointLatency;

                            if (_mrp_mo_update.managementobjectstats_attributes == null)
                            {
                                _mrp_mo_update.managementobjectstats_attributes = new List<MRPManagementobjectStatType>();
                            }
                            _mrp_mo_update.managementobjectstats_attributes.Add(_managedobject_stat);

                        }
                    }

                    _mrp_mo_update.state = _dt_job.Status.HighLevelState.ToString();
                    _mrp_mo_update.internal_state = "active";
                    _mrp_mo_update.last_contact = DateTime.UtcNow;
                    _mrp_mo_update.can_create_image_recovery = _dt_job.Status.CanCreateImageRecovery;
                    _mrp_mo_update.can_delete = _dt_job.Status.CanDelete;
                    _mrp_mo_update.can_edit = _dt_job.Status.CanEdit;
                    _mrp_mo_update.can_failback = _dt_job.Status.CanFailback;
                    _mrp_mo_update.can_failover = _dt_job.Status.CanFailover;
                    _mrp_mo_update.can_pause = _dt_job.Status.CanPause;
                    _mrp_mo_update.can_restore = _dt_job.Status.CanRestore;
                    _mrp_mo_update.can_reverse = _dt_job.Status.CanReverse;
                    _mrp_mo_update.can_start = _dt_job.Status.CanStart;
                    _mrp_mo_update.can_stop = _dt_job.Status.CanStop;
                    _mrp_mo_update.can_undo_failover = _dt_job.Status.CanUndoFailover;

                    _mrmp.managementobject().updatemanagementobject(_mrp_mo_update);
                }
            }
            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Job : Error collecting job information for {0} on {1} : {2}", _mrp_managementobject.moid, _mrp_managementobject.target_workload, ex.ToString()), Logger.Severity.Info);
                return;
            }

            Logger.log(String.Format("Double-Take Job: Completed Double-Take collection for {0} using {1}", _mrp_managementobject.target_workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

