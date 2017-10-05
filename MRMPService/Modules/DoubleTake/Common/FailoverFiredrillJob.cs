﻿using DoubleTake.Web.Models;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using System;
using System.Threading;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void FailoverFiredrillJob(MRPTaskType _task, MRMPWorkloadBaseType _source_workload, MRMPWorkloadBaseType _target_workload, MRPManagementobjectType _managementobject, MRPManagementobjectSnapshotType _managementobjectsnapshot, float _start_progress, float _end_progress, bool _group_task = false, bool _firedrill_failover = false)
        {
            using (Doubletake _dt = new Doubletake(null, _target_workload))
            {
                _task.progress("Initiating failover operation for " + _managementobject.moname, ReportProgress.Progress(_start_progress, _end_progress, 10));
                FailoverOptionsModel _options = new FailoverOptionsModel();

                _options.FailoverType = FailoverType.FullServer;
                if (_managementobjectsnapshot != null)
                {
                    _options.FailoverMode = FailoverMode.Snapshot;
                    _options.SnapshotId = Guid.Parse(_managementobjectsnapshot.snapshotmoid);
                    _task.progress(String.Format("Failing over from snapshot created on {0}", _managementobjectsnapshot.timestamp), ReportProgress.Progress(_start_progress, _end_progress, 12));
                }
                else
                {
                    _options.FailoverMode = FailoverMode.Live;
                    _task.progress("Failing over from live replication stream", ReportProgress.Progress(_start_progress, _end_progress, 12));
                }
                _task.progress("Applying remaining datablocks to target server", ReportProgress.Progress(_start_progress, _end_progress, 13));
                _options.FailoverDataAction = FailoverDataAction.Apply;

                JobInfoModel jobinfo = null;
                try
                {
                    jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                    if (jobinfo.Status.CanFailover)
                    {
                        _task.progress(_managementobject.moname + " is able to failover", ReportProgress.Progress(_start_progress, _end_progress, 20));
                    }
                    else
                    {
                        if (_group_task)
                        {
                            _task.progress(String.Format("{0} cannot be failed over: {1}", _managementobject.moname, jobinfo.Status.HighLevelState), ReportProgress.Progress(_start_progress, _end_progress, 20));
                            return;
                        }
                        else
                        {
                            throw new Exception(String.Format("{0} cannot be failed over: {1}", _managementobject.moname, jobinfo.Status.HighLevelState));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_group_task)
                    {
                        _task.progress(String.Format("{0} cannot be migrated: {1}", _managementobject.moname, ex.GetBaseException().Message), ReportProgress.Progress(_start_progress, _end_progress, 20));
                        return;
                    }
                    else
                    {
                        throw new Exception(String.Format("{0} cannot be migrated: {1}", _managementobject.moname, ex.GetBaseException().Message));
                    }
                }
                //ApiResponse<RecommendedFailoverOptionsModel> recommendedOptions = jobApi.RecommendFailoverOptionsAsync(jobId).Result;
                //var recommendedFailoverOptions.FailoverOptions.FailoverMode = FailoverMode.Test;
                //var result = jobApi.FailoverJobAsync(jobId, options, new Progress<ActivityStatusModel>(s => SimpleLog.LogVerbose(Options, s.Status.ToString())));


                ActivityStatusModel _status = _dt.job().FailoverJob(Guid.Parse(_managementobject.moid), _options);

                _task.progress("Failover process started for " + _managementobject.moname, ReportProgress.Progress(_start_progress, _end_progress, 25));
                jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                int _wait_times = 1;
                while (jobinfo.Status.HighLevelState != HighLevelState.FailingOver)
                {
                    _task.progress("Waiting for job to start the failover for " + _managementobject.moname, ReportProgress.Progress(_start_progress, _end_progress, 30));
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    _wait_times++;
                    jobinfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                }
                bool _switched_personalities = false;

                //switched target workload


                //the source might be switched off during the failover process, so we need exclude it from the connection object
                Doubletake _temp_dt = new Doubletake(null, _target_workload);

                int percentcomplete = 0;
                while (jobinfo.Status.HighLevelState == HighLevelState.FailingOver)
                {
                    try
                    {
                        if (jobinfo.Statistics.FullServerJobDetails != null)
                        {
                            percentcomplete = jobinfo.Statistics.FullServerJobDetails.CutoverDetails.PercentComplete;
                        }
                        else if (jobinfo.Statistics.ImageRecoveryJobDetails != null)
                        {
                            percentcomplete = jobinfo.Statistics.ImageRecoveryJobDetails.CutoverDetails.PercentComplete;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    String progress = String.Format("{0}% complete", percentcomplete);
                    _task.progress(progress, ReportProgress.Progress(_start_progress, _end_progress, ReportProgress.Progress(35, 90, percentcomplete)));
                    DTJobPoller.PollerDo(_managementobject);

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    DateTime timeoutTime = DateTime.UtcNow.AddMinutes(15);
                    while (true)
                    {
                        if (DateTime.UtcNow > timeoutTime)
                        {
                            _task.progress(String.Format("Timeout waiting for target workload {0} to become available", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));
                            throw new Exception(String.Format("Timeout waiting for target workload {0} to become available", _target_workload.hostname));
                        }
                        try
                        {
                            if (_switched_personalities)
                            {
                                MRMPWorkloadBaseType _new_target_workload = _target_workload;
                                _new_target_workload.credential = _source_workload.credential;
                                Doubletake _switched_dt = new Doubletake(null, _new_target_workload);
                                _switched_dt.management().UnAuthorizationAsync();
                                jobinfo = _switched_dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                                _task.progress(String.Format("{0} became available, finalizing failover", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 94));
                            }
                            else
                            {
                                jobinfo = _temp_dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                            }
                            break;
                        }
                        catch (Exception)
                        {
                            //we now know the target server is now offline and the source personality will now move to the target
                            //we need to switch the source and target personalities to make sure we can connect to the new target server
                            if (!_switched_personalities)
                            {
                                if (_firedrill_failover)
                                {
                                    _task.progress("Switching source and target credentials for " + _managementobject.moname, ReportProgress.Progress(_start_progress, _end_progress, 90));

                                    MRMPWorkloadBaseType _target_update_workload = new MRMPWorkloadBaseType();
                                    _target_update_workload.credential_id = _source_workload.credential_id;
                                    MRMPServiceBase._mrmp_api.workload().updateworkload(_target_update_workload);
                                }
                                else
                                {
                                    _task.progress("Setting source workload to disabled and switching source and target credentials for " + _managementobject.moname, ReportProgress.Progress(_start_progress, _end_progress, 90));

                                    MRMPWorkloadBaseType _source_update_workload = new MRMPWorkloadBaseType();
                                    MRMPWorkloadBaseType _target_update_workload = new MRMPWorkloadBaseType();
                                    MRPManagementobjectType _update_managementobject = new MRPManagementobjectType();

                                    _source_update_workload.id = _source_workload.id;
                                    _source_update_workload.enabled = false;
                                    _source_update_workload.dt_installed = false;
                                    MRMPServiceBase._mrmp_api.workload().updateworkload(_source_update_workload);

                                    _target_update_workload.id = _target_workload.id;
                                    _target_update_workload.enabled = true;
                                    _target_update_workload.dt_installed = true;
                                    _target_update_workload.credential_id = _source_workload.credential_id;
                                    _target_update_workload.hostname = _source_workload.hostname;
                                    if (!jobinfo.Options.SystemStateOptions.IsWanFailover)
                                    {
                                        _target_update_workload.iplist = _source_workload.iplist;
                                    }
                                    MRMPServiceBase._mrmp_api.workload().updateworkload(_target_update_workload);
                                }
                                _switched_personalities = true;
                            }
                            _task.progress(String.Format("Waiting for target workload {0} to become available", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 93));
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                    }
                }
                if (jobinfo.Status.HighLevelState == HighLevelState.FailedOver)
                {
                    MRPManagementobjectType _udated_managementobject = MRMPServiceBase._mrmp_api.managementobject().getmanagementobject_id(_managementobject.id);
                    DTJobPoller.PollerDo(_udated_managementobject);
                    _task.progress(String.Format("Successfully moved {0} to {1}", _source_workload.hostname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));
                }
                else if (jobinfo.Status.HighLevelState == HighLevelState.FailoverFailed)
                {
                    _task.progress(String.Format("Error moving {0} to {1}", _source_workload.hostname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));
                    throw new Exception(String.Format("Error moving {0} to {1}", _source_workload.hostname, _target_workload.hostname));
                }
                else if (jobinfo.Status.HighLevelState == HighLevelState.TargetInfoNotAvailable)
                {
                    _task.progress(String.Format("Cannot contact {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));
                    throw new Exception(String.Format("Cannot contact {0}", _target_workload.hostname));
                }
            }
        }
    }
}