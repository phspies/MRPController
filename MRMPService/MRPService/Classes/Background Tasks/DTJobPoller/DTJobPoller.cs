using MRMPService.MRMPService.Log;
using System;
using MRMPService.Utilities;
using MRMPService.MRMPAPI.Types.API;
using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using System.Collections.Generic;
using System.Linq;

namespace MRMPService.DTPollerCollection
{
    class DTJobPoller
    {
        public static void PollerDo(MRPJobType _mrp_job)
        {
            //check for credentials
            MRPWorkloadType _target_workload = _mrp_job.target_workload;
            MRPCredentialType _credential = _target_workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error finding credentials for workload {0} {1}", _mrp_job.target_workload.id, _mrp_job.target_workload.hostname));
            }

            //check for working IP
            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error finding contactable IP for workload {0}", _target_workload.hostname));
            }
            Logger.log(String.Format("Double-Take Job: Start Double-Take collection for {0} using {1}", _mrp_job.target_workload.hostname, workload_ip), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            JobInfoModel _dt_job;
            IEnumerable<ImageInfoModel> _dt_image_list = new List<ImageInfoModel>();

            bool _can_connect = false;
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _target_workload))
                {
                    ProductInfoModel _info = _dt.management().GetProductInfo().Result;
                    _dt_image_list = _dt.image().GetImages(_mrp_job.jobname).Result;

                    //now we try to get the job information we have. IF we can't, then we know the job has been deleted...
                    _can_connect = true;
                    _dt_job = _dt.job().GetJob((Guid)_mrp_job.dt_job_id).Result;
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
                        Logger.log(String.Format("Double-Take Job: Error collecting job information from {0} using {1} : {2}", _mrp_job.target_workload, workload_ip, ex.ToString()), Logger.Severity.Info);

                        _mrmp.job().updatejob(new MRPJobType()
                        {
                            id = _mrp_job.id,
                            internal_state = "deleted",
                        });
                    }
                    else
                    {
                        _mrmp.workload().DoubleTakeUpdateStatus(_target_workload, ex.Message, false);
                    }
                }

                Logger.log(String.Format("Double-Take Job: Error contacting Double-Take Management Service for {0} using {1} : {2}", _mrp_job.target_workload, workload_ip, ex.ToString()), Logger.Severity.Info);
                throw new ArgumentException(String.Format("Double-Take Job: Error contacting Double-Take management service for {0} using {1}", _mrp_job.target_workload, workload_ip));
            }

            //now collect job information from target workload
            try
            {
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    foreach (ImageInfoModel _dt_image in _dt_image_list)
                    {
                        MRPJobImageType _mrp_image = new MRPJobImageType();
                        if (_mrp_job.jobimages_attributes.Exists(x => x.moid == _dt_image.Id))
                        {
                            _mrp_image.id = _mrp_job.jobimages_attributes.FirstOrDefault(x => x.moid == _dt_image.Id).id;
                        }
                        else
                        {
                            if (_mrp_job.jobimages_attributes == null)
                            {
                                _mrp_job.jobimages_attributes = new List<MRPJobImageType>();
                            }
                            _mrp_job.jobimages_attributes.Add(_mrp_image);
                        }
                        _mrp_image.image_name = _dt_image.ImageName;
                        _mrp_image.image_type = _dt_image.ImageType;
                        _mrp_image.moid = _dt_image.Id;
                        _mrp_image.protection_connection_id = _dt_image.ProtectionConnectionId;
                        _mrp_image.protection_job_name = _dt_image.ProtectionJobName;
                        _mrp_image.source_image_mount_location = _dt_image.SourceImageMountLocation;
                        _mrp_image.source_name = _dt_image.SourceName;
                        _mrp_image.state = _dt_image.State;
                        _mrp_image.creation_timestamp = _dt_image.CreationTimestamp;
                        _mrp_image.description = _dt_image.Description;

                        foreach (SnapshotEntryModel _dt_snap in _dt_image.Snapshots)
                        {
                            MRPSnapshotType _mrp_snapshot = new MRPSnapshotType();
                            if (_mrp_image.jobimagesnapshots_attributes.Exists(x => x.moid == _dt_snap.Id))
                            {
                                _mrp_snapshot.id = _mrp_image.jobimagesnapshots_attributes.FirstOrDefault(x => x.moid == _dt_snap.Id).id;
                            }
                            else
                            {
                                if (_mrp_image.jobimagesnapshots_attributes == null)
                                {
                                    _mrp_image.jobimagesnapshots_attributes = new List<MRPSnapshotType>();
                                }
                                _mrp_image.jobimagesnapshots_attributes.Add(_mrp_snapshot);
                            }
                            _mrp_snapshot.moid = _dt_snap.Id;
                            _mrp_snapshot.reason = _dt_snap.Reason;
                            _mrp_snapshot.states = _dt_snap.States;
                            _mrp_snapshot.timestamp = _dt_snap.Timestamp.UtcDateTime;
                            _mrp_snapshot.comment = _dt_snap.Comment;
                        }
                    }

                    //Update job details
                    CoreConnectionDetailsModel _connection_details = _dt_job.Statistics.CoreConnectionDetails;
                    MRPJobstatType _job_stat = new MRPJobstatType();
                    {
                        if (_connection_details != null)
                        {
                            _job_stat.replication_bytes_sent = _connection_details.ReplicationBytesSent;
                            _job_stat.replication_bytes_sent_compressed = _connection_details.ReplicationBytesTransmitted;
                            _job_stat.replication_disk_queue = _connection_details.DiskQueueBytes;
                            _job_stat.replication_queue = _connection_details.ReplicationBytesQueued;
                            _job_stat.replication_status = _connection_details.ReplicationState.ToString();

                            _job_stat.mirror_remaining = _connection_details.MirrorBytesRemaining;
                            _job_stat.mirror_percent_complete = _connection_details.MirrorPermillage;
                            _job_stat.mirror_skipped = _connection_details.MirrorBytesSkipped;
                            _job_stat.mirror_status = _connection_details.MirrorState.ToString();
                            _job_stat.stransmit_mode = _dt_job.Status.HighLevelState.ToString();
                            _job_stat.recovery_point_objective = _connection_details.SourceRpo.UtcDateTime;

                            _mrp_job.jobstats_attributes.Add(_job_stat);

                        }
                    }


                    _mrp_job.internal_state = "active";
                    _mrp_job.last_contact = DateTime.UtcNow;
                    _mrp_job.can_create_image_recovery = _dt_job.Status.CanCreateImageRecovery;
                    _mrp_job.can_delete = _dt_job.Status.CanDelete;
                    _mrp_job.can_edit = _dt_job.Status.CanEdit;
                    _mrp_job.can_failback = _dt_job.Status.CanFailback;
                    _mrp_job.can_failover = _dt_job.Status.CanFailover;
                    _mrp_job.can_pause = _dt_job.Status.CanPause;
                    _mrp_job.can_restore = _dt_job.Status.CanRestore;
                    _mrp_job.can_reverse = _dt_job.Status.CanReverse;
                    _mrp_job.can_start = _dt_job.Status.CanStart;
                    _mrp_job.can_stop = _dt_job.Status.CanStop;
                    _mrp_job.can_undo_failover = _dt_job.Status.CanUndoFailover;

                    _mrmp.job().updatejob(_mrp_job);
                }
            }
            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Job : Error collecting job information for {0} on {1} : {2}", _mrp_job.dt_job_id, _mrp_job.target_workload, ex.ToString()), Logger.Severity.Info);
                return;
            }

            Logger.log(String.Format("Double-Take Job: Completed Double-Take collection for {0} using {1}", _mrp_job.target_workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

