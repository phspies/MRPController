using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.DoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class DisasterRecovery
    {
        public static void CreateDRServerProtectionJob(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
                {
                    _mrp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", 2);
                    if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Protection))
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads."), 3);
                        _mrp_api.task().progress(_task_id, String.Format("Attempting to configure license in deployment policy."), 4);
                        _dt.management().InstallLicense(_target_workload.deploymentpolicy.source_activation_code, _source_workload.deploymentpolicy.target_activation_code);
                        if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Protection))
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads after trying to fix license"), 5);
                        }
                    }
                    else
                    {
                        _mrp_api.task().progress(_task_id, String.Format("License valid on workloads."), 3);
                    }

                    List<JobInfoModel> _jobs = _dt.job().GetJobs().Result;

                    _mrp_api.task().progress(_task_id, "Looking for DR Server jobs on target workload", 11);
                    int _count = 1;
                    //This is a migration server migration job, so we need to remove all jobs from target server
                    foreach (JobInfoModel _delete_job in _jobs.Where(x => x.JobType == DT_JobTypes.DR_Full_Protection))
                    {
                        _mrp_api.task().progress(_task_id, String.Format("{0} - Deleting existing DR Server job between {1} and {2}", _count, _source_workload.hostname, _target_workload.hostname), _count + 15);
                        _dt.job().DeleteJob(_delete_job.Id).Wait();
                        _count += 1;
                    }

                    var workloadId = Guid.Empty;
                    WorkloadModel wkld = (WorkloadModel)null;

                    workloadId = (_dt.workload().CreateWorkload(DT_JobTypes.DR_Full_Protection).Result).Id;
                    wkld = _dt.workload().GetWorkload(workloadId).Result;


                    JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                    _mrp_api.task().progress(_task_id, "Fetching recommended job options", 20);

                    CreateOptionsModel jobInfo = _dt.job().GetJobOptions(
                        wkld,
                        jobCreds,
                        DT_JobTypes.DR_Full_Protection).Result;

                    _mrp_api.task().progress(_task_id, "Setting job options", 50);
                    jobInfo = SetOptions.set_job_options(_task_id, _source_workload, _target_workload, _protectiongroup, jobInfo, 51, 54);

                    _mrp_api.task().progress(_task_id, "Verifying job options and settings", 60);

                    JobOptionsModel _job_model = new JobOptionsModel();
                    var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.DR_Full_Protection);
                    if (_fix_result.Item1)
                    {
                        _job_model = _fix_result.Item2;
                    }
                    else
                    {
                        int _fix_count = 0;
                        foreach (var _failed_item in _fix_result.Item3)
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), _fix_count + 61);

                            _fix_count++;
                        }
                        Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                        throw new System.ArgumentException(string.Format("Cannot create job"));

                    }
                    _mrp_api.task().progress(_task_id, "Creating new job", 65);
                    Guid jobId = _dt.job().CreateJob((new CreateOptionsModel
                    {
                        JobOptions = _job_model,
                        JobCredentials = jobCreds,
                        JobType = DT_JobTypes.DR_Full_Protection
                    })).Result;

                    _mrp_api.task().progress(_task_id, String.Format("Job created. Starting job id {0}", jobId), 66);
                    _dt.job().StartJob(jobId).Wait();

                    _mrp_api.task().progress(_task_id, String.Format("Registering job {0} with portal", jobId), 67);
                    _mrp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _managementobject.id,
                        moid = jobId,
                        motype = DT_JobTypes.DR_Full_Protection,
                        target_workload_id = _target_workload.id,
                        source_workload_id = _source_workload.id,
                        protectiongroup_id = _protectiongroup.id
                    });
                    _mrp_api.task().progress(_task_id, "Waiting for sync process to start", 71);

                    JobInfoModel jobinfo = _dt.job().GetJob(jobId).Result;
                    while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Mirror)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 30));
                        jobinfo = _dt.job().GetJob(jobId).Result;
                    }

                    _mrp_api.task().progress(_task_id, String.Format("Sync process started at {0}", jobinfo.Statistics.CoreConnectionDetails.StartTime), 75);

                    _mrp_api.task().progress(_task_id, String.Format("Successfully created disaster recover protection job between {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);
                }

            }
        }
    }
}

