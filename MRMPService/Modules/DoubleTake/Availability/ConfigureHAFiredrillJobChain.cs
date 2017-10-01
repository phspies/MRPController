using DoubleTake.Web.Models;
using MRMPService.Exceptions;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;

namespace MRMPService.Modules.DoubleTake.Availability
{
    partial class Availability
    {
        public static void ConfigureHAFiredrillJobChain(MRPTaskType _task, MRMPWorkloadBaseType _source_workload, MRMPWorkloadBaseType _target_workload, MRMPWorkloadBaseType _firedrill_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
            {
                _task.progress("Confirming job state", ReportProgress.Progress(_start_progress, _end_progress, 2));
                JobInfoModel _jobInfo = new JobInfoModel();
                try
                {
                    _jobInfo = _dt.job().GetJob(new Guid(_managementobject.moid));
                    if (!_jobInfo.Status.CanFailover)
                    {
                        new MRMPDatamoverException(String.Format("Job is not in clean state {0} : {1}", _jobInfo.Id, _jobInfo.Options.Name));
                    }
                }
                catch (Exception ex)
                {
                    new MRMPDatamoverException(String.Format("Unable to connect to firedill datamove engine {0} : {1}", _target_workload.hostname, ex.GetBaseException().Message));
                }
                String _firedrill_uniq_id;
                using (Doubletake _uniq_dt = new Doubletake(null, _firedrill_workload))
                {
                    _firedrill_uniq_id = _uniq_dt.management().GetWorkloadUniqID();
                }
                FullServerTestFailoverOptionsModel _failover_options = new FullServerTestFailoverOptionsModel()
                {
                    FailoverName = _firedrill_workload.hostname,
                    TestFailoverServerAddress = _firedrill_workload.GetContactibleIP(true),
                    TestFailoverServerCredential = new TestFailoverServerCredentialModel()
                    {
                        TestFailoverServerHostUri = new Uri(String.Format("dtms://{0}:{1}@{2}:6325", _firedrill_workload.GetCredentialUsername(), _firedrill_workload.GetCredentialPassword(), _firedrill_workload.GetContactibleIP())),
                        TestFailoverServerHardwareId = _firedrill_uniq_id
                    },
                    DeleteSnapshots = false
                };
                _jobInfo.Options.FullServerTestFailoverOptions = _failover_options;

                JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();
                JobOptionsModel _job_model = new JobOptionsModel();
                _task.progress("Verifying job options and settings", ReportProgress.Progress(_start_progress, _end_progress, 60));
                var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, _jobInfo.Options, _jobInfo.JobType);
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
                    Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                    new MRMPDatamoverException(string.Format("Cannot update job"));

                }
                _task.progress("Updating Datamover Information", ReportProgress.Progress(_start_progress, _end_progress, 71));
                _dt.job().UpdateJob(_jobInfo.Id, _jobInfo.Options);

                DTJobPoller.PollerDo(_managementobject);

                _task.progress(String.Format("Successfully created firedrill operation between {0} to {1}", _target_workload.hostname, _firedrill_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));

            }
        }
    }
}

