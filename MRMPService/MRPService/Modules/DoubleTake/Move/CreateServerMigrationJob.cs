using DoubleTake.Web.Models;
using MRMPService.DTPollerCollection;
using MRMPService.MRMPAPI;
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

namespace MRMPService.Modules.DoubleTake.Move
{
    partial class Migration
    {
        public static void CreateServerMigrationJob(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
            {
                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", ReportProgress.Progress(_start_progress, _end_progress, 2));
                if (!_dt.management().CheckLicense(DT_JobTypes.Move_Server_Migration, _protectiongroup.organization_id))
                {
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads. Trying to fix the licenses."), 3);
                    if (!_dt.management().CheckLicense(DT_JobTypes.Move_Server_Migration, _protectiongroup.organization_id))
                    {
                        throw new Exception(String.Format("Invalid license detected on workloads after trying to fix licenses"));
                    }
                }
                else
                {
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("License valid on workloads."), ReportProgress.Progress(_start_progress, _end_progress, 3));
                }

                List<JobInfoModel> _jobs = _dt.job().GetJobs();

                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Looking for migration jobs on target workload", ReportProgress.Progress(_start_progress, _end_progress, 11));
                int _count = 1;
                //This is a migration server migration job, so we need to remove all jobs from target server
                foreach (JobInfoModel _delete_job in _jobs.Where(x => x.JobType == "MoveServerMigration"))
                {
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("{0} - Deleting existing migration job between {1} and {2}", _count, _source_workload.hostname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _count + 15));
                    _dt.job().DeleteJob(_delete_job.Id);
                    _count += 1;
                }

                var workloadId = Guid.Empty;
                WorkloadModel wkld = (WorkloadModel)null;

                workloadId = (_dt.workload().CreateWorkload(DT_JobTypes.Move_Server_Migration)).Id;
                wkld = _dt.workload().GetWorkload(workloadId);


                JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Fetching recommended job options", ReportProgress.Progress(_start_progress, _end_progress, 20));

                CreateOptionsModel jobInfo = _dt.job().GetJobOptions(
                    wkld,
                    jobCreds,
                    DT_JobTypes.Move_Server_Migration);

                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Setting job options", 50);
                jobInfo = SetOptions.set_job_options(_task_id, _source_workload, _target_workload, _protectiongroup, jobInfo, 51, 54);

                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Verifying job options and settings", 55);

                JobOptionsModel _job_model = new JobOptionsModel();
                var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.Move_Server_Migration);
                if (_fix_result.Item1)
                {
                    _job_model = _fix_result.Item2;
                }
                else
                {
                    int _fix_count = 0;
                    foreach (var _failed_item in _fix_result.Item3)
                    {
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), ReportProgress.Progress(_start_progress, _end_progress, _fix_count + 55));

                        _fix_count++;
                    }
                    Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                    throw new System.ArgumentException(string.Format("Cannot create job"));

                }
                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Creating new job", ReportProgress.Progress(_start_progress, _end_progress, 56));
                Guid jobId = _dt.job().CreateJob((new CreateOptionsModel
                {
                    JobOptions = _job_model,
                    JobCredentials = jobCreds,
                    JobType = DT_JobTypes.Move_Server_Migration
                }));

                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Job created. Starting job id {0}", jobId), ReportProgress.Progress(_start_progress, _end_progress, 57));
                _dt.job().StartJob(jobId);

                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Registering job {0} with portal", jobId), ReportProgress.Progress(_start_progress, _end_progress, 60));
                MRMPServiceBase._mrmp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                {
                    moid = jobId.ToString(),
                    id = _managementobject.id,
                    moname = jobInfo.JobOptions.Name,
                    motype = DT_JobTypes.Move_Server_Migration
                });
                _managementobject = MRMPServiceBase._mrmp_api.managementobject().getmanagementobject_id(_managementobject.id);


                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Waiting for sync process to start", ReportProgress.Progress(_start_progress, _end_progress, 65));

                JobInfoModel jobinfo = _dt.job().GetJob(jobId);
                while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Mirror)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    jobinfo = _dt.job().GetJob(jobId);
                }

                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Sync process started at {0}", jobinfo.Statistics.CoreConnectionDetails.StartTime), ReportProgress.Progress(_start_progress, _end_progress, 70));

                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Successfully created move synchronization job between {0} to {1}", _source_workload.hostname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 95));


                MRPWorkloadType _update_workload = new MRPWorkloadType();
                _update_workload.id = _target_workload.id;
                _update_workload.dt_installed = true;
                _update_workload.dt_collection_enabled = true;
                MRMPServiceBase._mrmp_api.workload().updateworkload(_update_workload);

                DTJobPoller.PollerDo(_managementobject);
            }
        }
    }
}
