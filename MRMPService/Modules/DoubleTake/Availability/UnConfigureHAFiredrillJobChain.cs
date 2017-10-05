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

namespace MRMPService.Modules.DoubleTake.Availability
{
    partial class Availability
    {
        public static void UnConfigureHAFiredrillJobChain(MRPTaskType _task, MRMPWorkloadBaseType _target_workload, MRMPWorkloadBaseType _firedrill_workload,  MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (Doubletake _dt = new Doubletake(null, _target_workload))
            {
                _task.progress("Confirming job state", ReportProgress.Progress(_start_progress, _end_progress, 2));
                JobInfoModel _jobInfo = new JobInfoModel();
                try
                {
                    _jobInfo = _dt.job().GetJob(new Guid(_managementobject.moid));
                    if (_jobInfo.Status.IsInError)
                    {
                        throw new MRMPDatamoverException(String.Format("Job is not in clean state {0} : {1}", _jobInfo.Id, _jobInfo.Options.Name));
                    }
                }
                catch (Exception ex)
                {
                    throw new MRMPDatamoverException(String.Format("Unable to connect to datamover engine {0} : {1}", _target_workload.hostname, ex.GetBaseException().Message));
                }


                FullServerTestFailoverOptionsModel _failover_options = new FullServerTestFailoverOptionsModel();
                _jobInfo.Options.FullServerTestFailoverOptions = _failover_options;

                JobOptionsModel _job_model = new JobOptionsModel();
                _task.progress("Verifying options and settings", ReportProgress.Progress(_start_progress, _end_progress, 60));
                var _fix_result = _dt.job().VerifyAndFixExistingJobOptions(_jobInfo.Id, _jobInfo.Options);
                if (_fix_result.Item1)
                {
                    _job_model = _fix_result.Item2;
                }
                else
                {
                    int _fix_count = 0;
                    foreach (var _failed_item in _fix_result.Item3)
                    {
                        _task.progress(String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), ReportProgress.Progress(_start_progress, _end_progress, _fix_count + 61));

                        _fix_count++;
                    }
                    throw ExceptionFactory.JobOptionsVerify();
                }
                _task.progress("Removing firedill configuration information", ReportProgress.Progress(_start_progress, _end_progress, 71));
                _dt.job().UpdateJob(_jobInfo.Id, _jobInfo.Options);
                DTJobPoller.PollerDo(_managementobject);
                _task.progress(String.Format("Successfully removed firedrill configuration between {0} to {1}", _target_workload.hostname, _firedrill_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));
            }
        }
    }
}

