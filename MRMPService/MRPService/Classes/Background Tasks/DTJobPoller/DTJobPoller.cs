using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using MRMPService.Utilities;
using MRMPService.MRMPAPI.Types.API;
using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;

namespace MRMPService.DTPollerCollection
{
    class DTJobPoller
    {
        public static void PollerDo(MRPJobType job)
        {
            //check for credentials
            MRPWorkloadType _target_workload = job.target_workload;
            if (_target_workload == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error finding workload in local database {0} {1}", job.target_workload.id, job.target_workload.hostname));
            }
            MRPCredentialType _credential = _target_workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Double-Take Job: Error finding credentials for workload {0} {1}", job.target_workload.id, job.target_workload.hostname));
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
            Logger.log(String.Format("Double-Take Job: Start Double-Take collection for {0} using {1}", job.target_workload.hostname, workload_ip), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _target_workload))
                {
                    ProductInfoModel _dt_info = _dt.management().GetProductInfo().Result;
                }
            }
            catch (Exception ex)
            {
                //Mark the job as being unavailable because the system can't be reached
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _mrmp.job().updatejob(new MRPJobType()
                    {
                        id = job.id,
                        internal_state = "unavailable",
                    });
                }

                Logger.log(String.Format("Double-Take Job: Error contacting Double-Take Management Service for {0} using {1} : {2}", job.target_workload, workload_ip, ex.ToString()), Logger.Severity.Info);
                throw new ArgumentException(String.Format("Double-Take Job: Error contacting Double-Take management service for {0} using {1}", job.target_workload, workload_ip));
            }
            //now collect job information from target workload
            JobInfoModel _dt_job;
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _target_workload))
                {
                    _dt_job = _dt.job().GetJob(Guid.Parse(job.dt_job_id)).Result;
                }
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    CoreConnectionDetailsModel _connection_details = _dt_job.Statistics.CoreConnectionDetails;
                    MRPJobstatType _job_stat = new MRPJobstatType();
                    {
                        _job_stat.job_id = job.id;
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
                        }
                    }
                    _mrmp.jobstat().createjobstat(_job_stat);
                    _mrmp.job().updatejob(new MRPJobType()
                    {
                        id = job.id,
                        internal_state = "active",
                        last_contact = DateTime.UtcNow,
                        can_create_image_recovery = _dt_job.Status.CanCreateImageRecovery,
                        can_delete = _dt_job.Status.CanDelete,
                        can_edit = _dt_job.Status.CanEdit,
                        can_failback = _dt_job.Status.CanFailback,
                        can_failover = _dt_job.Status.CanFailover,
                        can_pause = _dt_job.Status.CanPause,
                        can_restore = _dt_job.Status.CanRestore,
                        can_reverse = _dt_job.Status.CanReverse,
                        can_start = _dt_job.Status.CanStart,
                        can_stop = _dt_job.Status.CanStop,
                        can_undo_failover = _dt_job.Status.CanUndoFailover,

                    });
                }
            }
            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Job : Error collecting job information for {0} on {1} : {2}", job.dt_job_id, job.target_workload, ex.ToString()), Logger.Severity.Info);
                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    _mrmp.job().updatejob(new MRPJobType()
                    {
                        id = job.id,
                        internal_state = "deleted",
                    });
                }
                return;
            }

            Logger.log(String.Format("Double-Take Job: Completed Double-Take collection for {0} using {1}", job.target_workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

