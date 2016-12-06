using DoubleTake.Web.Models;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.Scheduler.DTPollerCollection;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake.Availability
{
    partial class Availability
    {
        public static async Task CreateHAServerProtectionJob(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress, String _job_type)
        {
            using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
            {
                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", ReportProgress.Progress(_start_progress, _end_progress, 2));
                if (!_dt.management().CheckLicense(_job_type.ToString(), _protectiongroup.organization_id))
                {
                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads. Trying to fix the licenses."), 3);
                    if (!_dt.management().CheckLicense(_job_type.ToString(), _protectiongroup.organization_id))
                    {
                        throw new Exception(String.Format("Invalid license detected on workloads after trying to fix licenses"));
                    }
                }
                else
                {
                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("License valid on workloads."), ReportProgress.Progress(_start_progress, _end_progress, 3));
                }

                List<JobInfoModel> _jobs = await _dt.job().GetJobs();

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Looking for DR Server jobs on target workload", ReportProgress.Progress(_start_progress, _end_progress, 11));
                int _count = 1;

                //This is a migration server migration job, so we need to remove all jobs from target server
                foreach (JobInfoModel _delete_job in _jobs.Where(x => x.SourceServer.Host.Contains(_source_workload.hostname) && x.TargetServer.Host.Contains(_target_workload.hostname)))
                {
                    await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("{0} - Deleting existing DR Server job between {1} and {2}", _count, _delete_job.SourceServer.Host, _delete_job.TargetServer.Host), ReportProgress.Progress(_start_progress, _end_progress, _count + 15));
                    _dt.job().DeleteJob(_delete_job.Id).Wait();
                    _count += 1;
                }

                var workloadId = Guid.Empty;
                WorkloadModel wkld = (WorkloadModel)null;

                workloadId = (_dt.workload().CreateWorkload(_job_type.ToString()).Result).Id;
                wkld = await _dt.workload().GetWorkload(workloadId);


                JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Fetching recommended job options", ReportProgress.Progress(_start_progress, _end_progress, 20));

                CreateOptionsModel jobInfo = await _dt.job().GetJobOptions(
                    wkld,
                    jobCreds,
                    _job_type.ToString());

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Setting job options", ReportProgress.Progress(_start_progress, _end_progress, 30));
                jobInfo = await SetOptions.set_job_options(_task_id, _source_workload, _target_workload, _protectiongroup, jobInfo, 40, 59);

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Verifying job options and settings", ReportProgress.Progress(_start_progress, _end_progress, 60));

                JobOptionsModel _job_model = new JobOptionsModel();
                var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, _job_type.ToString());
                if (_fix_result.Item1)
                {
                    _job_model = _fix_result.Item2;
                }
                else
                {
                    int _fix_count = 0;
                    foreach (var _failed_item in _fix_result.Item3)
                    {
                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), ReportProgress.Progress(_start_progress, _end_progress, _fix_count + 61));

                        _fix_count++;
                    }
                    Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                    throw new System.ArgumentException(string.Format("Cannot create job"));

                }
                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Creating new job", ReportProgress.Progress(_start_progress, _end_progress, 65));
                Guid jobId = await _dt.job().CreateJob((new CreateOptionsModel
                {
                    JobOptions = _job_model,
                    JobCredentials = jobCreds,
                    JobType = _job_type.ToString()
                }));

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Job created. Starting job id {0}", jobId), ReportProgress.Progress(_start_progress, _end_progress, 66));
                await _dt.job().StartJob(jobId);

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Registering job {0} with portal", jobId), ReportProgress.Progress(_start_progress, _end_progress, 67));
                await MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                {
                    id = _managementobject.id,
                    moid = jobId.ToString(),
                    moname = jobInfo.JobOptions.Name,
                    motype = _job_type.ToString()
                });
                await MRMPServiceBase._mrmp_api.task().progress(_task_id, "Waiting for sync process to start", ReportProgress.Progress(_start_progress, _end_progress, 71));

                JobInfoModel jobinfo = await _dt.job().GetJob(jobId);
                while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Mirror)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    jobinfo = await _dt.job().GetJob(jobId);
                }

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Sync process started at {0}", jobinfo.Statistics.CoreConnectionDetails.StartTime), 75);

                await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Successfully created disaster recover protection job between {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);

                MRPWorkloadType _update_workload = new MRPWorkloadType();
                _update_workload.id = _target_workload.id;
                _update_workload.dt_installed = true;
                await MRMPServiceBase._mrmp_api.workload().updateworkload(_update_workload);

                await DTJobPoller.PollerDo(_managementobject);
            }
        }
    }
}

