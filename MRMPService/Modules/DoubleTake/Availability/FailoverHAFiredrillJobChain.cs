using DoubleTake.Web.Models;
using MRMPService.Exceptions;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;

namespace MRMPService.Modules.DoubleTake.Availability
{
    partial class Availability
    {
        public static void FailoverHAFiredrillJobChain(MRPTaskType _task, MRMPWorkloadBaseType _source_workload, MRMPWorkloadBaseType _target_workload, MRMPWorkloadBaseType _firedrill_workload, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
            {
                _task.progress($"Confirming job state for {_managementobject.moname} after change", ReportProgress.Progress(_start_progress, _end_progress, 2));
                JobInfoModel _jobInfo = new JobInfoModel();
                try
                {
                    _jobInfo = _dt.job().GetJob(new Guid(_managementobject.moid));
                    if (!_jobInfo.Status.CanFailover)
                    {
                        throw new MRMPDatamoverException(String.Format("Job is not in clean state {0} : {1}", _jobInfo.Id, _jobInfo.Options.Name));
                    }
                }
                catch (Exception ex)
                {
                    throw new MRMPDatamoverException(String.Format("Unable to connect to target workload datamover engine {0} : {1}", _target_workload.hostname, ex.GetBaseException().Message));
                }
                FailoverOptionsModel _failover_options = new FailoverOptionsModel()
                {
                    FailoverMode = FailoverMode.Test,
                    FailoverType = FailoverType.FullServer,
                    FailoverDataAction = FailoverDataAction.Apply
                };
                _dt.job().FailoverJob(_jobInfo.Id, _failover_options);
                Thread.Sleep(TimeSpan.FromSeconds(10));
                _jobInfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                while (_jobInfo.Status.ExtendedLowLevelStates.Count() > 0)
                {
                    if (_jobInfo.Status.ExtendedLowLevelStates.Any(x => x.MessageId == "LowLevelStateTestFailoverConnectionSynchronizing"))
                    {
                        var _state = _jobInfo.Status.ExtendedLowLevelStates.FirstOrDefault(x => x.MessageId == "LowLevelStateTestFailoverConnectionSynchronizing");
                        if (_state.MessageIdFormatParameters.Length != 0)
                        {
                            float _percentage = float.Parse(_state.MessageIdFormatParameters[0]);
                            _task.progress($"Synchronizing {_firedrill_workload.hostname} : {_percentage}%", ReportProgress.Progress(_start_progress, _end_progress, ReportProgress.Progress(15, 40, _percentage)));
                        }
                    }
                    else if (_jobInfo.Status.ExtendedLowLevelStates.Any(x => x.MessageId == "LowLevelStateTestFailoverCutoverWithProgress"))
                    {
                        var _state = _jobInfo.Status.ExtendedLowLevelStates.FirstOrDefault(x => x.MessageId == "LowLevelStateTestFailoverCutoverWithProgress");
                        if (_state.MessageIdFormatParameters.Length != 0)
                        {
                            float _percentage = float.Parse(_state.MessageIdFormatParameters[0]);
                            _task.progress($"Failing over {_firedrill_workload.hostname} : {_percentage}%", ReportProgress.Progress(_start_progress, _end_progress, ReportProgress.Progress(41, 70, _percentage)));
                        }
                    }
                    else if (_jobInfo.Status.ExtendedLowLevelStates.Any(x => x.MessageId == "LowLevelStateTestFailoverRestart3rdServer"))
                    {
                        _task.progress($"Rebooting {_firedrill_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 80));
                        break;
                    }
                    if (_jobInfo.Status.ExtendedLowLevelStates.Any(x => x.Health == Health.Error))
                    {
                        throw new MRMPDatamoverException($"Failover to firedrill workload : {_jobInfo.Status.ExtendedLowLevelStates.First().MessageId}");
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    _jobInfo = _dt.job().GetJob(Guid.Parse(_managementobject.moid.ToString()));
                }
                _task.progress($"Waiting for {_firedrill_workload.hostname} to became available", ReportProgress.Progress(_start_progress, _end_progress, 90));
                MRMPWorkloadBaseType _firedrill_update_workload = new MRMPWorkloadBaseType();
                _firedrill_update_workload.id = _firedrill_workload.id;
                _firedrill_update_workload.enabled = false;
                _firedrill_update_workload.dt_installed = false;
                _firedrill_update_workload.credential_id = _source_workload.credential_id;
                MRMPServiceBase._mrmp_api.workload().updateworkload(_firedrill_update_workload);
                _firedrill_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_firedrill_update_workload.id);
                DateTime timeoutTime = DateTime.UtcNow.AddMinutes(15);
                while (true)
                {
                    if (DateTime.UtcNow > timeoutTime)
                    {
                        throw new Exception(String.Format("Timeout waiting for target workload {0} to become available", _firedrill_workload.hostname));
                    }
                    try
                    {
                        using (Doubletake _switched_dt = new Doubletake(null, _firedrill_workload))
                        {
                            _switched_dt.management().UnAuthorizationAsync();
                            var _mgt = _switched_dt.management().GetWorkloadUniqID();
                            _task.progress($"{_firedrill_workload.hostname} became available : {_mgt}", ReportProgress.Progress(_start_progress, _end_progress, 94));
                        }
                        break;
                    }
                    catch (Exception _ex) { }
                }
                _task.progress($"Completed firedrill failover between {_source_workload.hostname} and {_firedrill_workload.hostname}", ReportProgress.Progress(_start_progress, _end_progress, 95));

            }
        }
    }
}

