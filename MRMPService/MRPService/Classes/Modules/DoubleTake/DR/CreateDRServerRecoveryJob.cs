using DoubleTake.Web.Models;
using MRMPService.DTPollerCollection;
using MRMPService.MRMPAPI;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MRMPService.Tasks.DoubleTake
{
    partial class DisasterRecovery
    {
        public static void CreateDRServerRecoveryJob(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPWorkloadType _original_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
                {
                    _mrp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", ReportProgress.Progress(_start_progress, _end_progress, 2));
                    if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Recovery, _protectiongroup.organization_id))
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Invalid license detected on workloads. Trying to fix the licenses."), ReportProgress.Progress(_start_progress, _end_progress, 3));
                        if (!_dt.management().CheckLicense(DT_JobTypes.DR_Full_Recovery, _protectiongroup.organization_id))
                        {
                            throw new Exception(String.Format("Invalid license detected on workloads after trying to fix licenses"));
                        }
                    }
                    else
                    {
                        _mrp_api.task().progress(_task_id, String.Format("License valid on workloads."), ReportProgress.Progress(_start_progress, _end_progress, 3));
                    }

                    List<JobInfoModel> _jobs = _dt.job().GetJobs().Result;

                    _mrp_api.task().progress(_task_id, "Looking for DR Server jobs on target workload", ReportProgress.Progress(_start_progress, _end_progress, 11));
                    int _count = 1;
                    //This is a migration server migration job, so we need to remove all jobs from target server
                    foreach (JobInfoModel _delete_job in _jobs.Where(x => x.JobType == DT_JobTypes.DR_Full_Recovery))
                    {
                        _mrp_api.task().progress(_task_id, String.Format("{0} - Deleting existing DR Server job between {1} and {2}", _count, _source_workload.hostname, _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _count + 15));
                        _dt.job().DeleteJob(_delete_job.Id).Wait();
                        _count += 1;
                    }

                    WorkloadModel wkld = (WorkloadModel)null;

                    if (_managementobject.managementobjectsnapshot != null)
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Recovering data from snapshot taken at {0}", _managementobject.managementobjectsnapshot.timestamp), ReportProgress.Progress(_start_progress, _end_progress, 19));

                        wkld = _dt.workload().CreateWorkloadDRRecovery(Guid.Parse(_managementobject.managementobjectsnapshot.imagemoid), Guid.Parse(_managementobject.managementobjectsnapshot.snapshotmoid)).Result;
                    }
                    else
                    {
                        IEnumerable<ImageInfoModel> images = _dt.image().GetAllImagesFromSource().Result;
                        ImageInfoModel image = images.Where(i => i.SourceName == _original_workload.hostname && i.ImageType == ImageType.FullServer).First();
                        if (image == null)
                        {
                            throw new Exception(String.Format("Cannot find image on repository server with source host of {0}", _managementobject.source_workload.hostname));
                        }
                        wkld = _dt.workload().CreateWorkloadDRRecovery(image.Id, Guid.Empty).Result;
                    }


                    JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                    _mrp_api.task().progress(_task_id, "Fetching recommended job options", ReportProgress.Progress(_start_progress, _end_progress, 20));

                    CreateOptionsModel jobInfo = _dt.job().GetJobOptions(
                        wkld,
                        jobCreds,
                        DT_JobTypes.DR_Full_Recovery).Result;

                    _mrp_api.task().progress(_task_id, "Setting job options", ReportProgress.Progress(_start_progress, _end_progress, 30));
                    jobInfo = SetOptions.set_job_options(_task_id, _source_workload, _target_workload, _protectiongroup, jobInfo, 40, 59, _managementobject);

                    _mrp_api.task().progress(_task_id, "Verifying job options and settings", ReportProgress.Progress(_start_progress, _end_progress, 60));

                    JobOptionsModel _job_model = new JobOptionsModel();
                    var _fix_result = _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.DR_Full_Recovery);
                    if (_fix_result.Item1)
                    {
                        _job_model = _fix_result.Item2;
                    }
                    else
                    {
                        int _fix_count = 0;
                        foreach (var _failed_item in _fix_result.Item3)
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Error creating job: {0} : {1}", _failed_item.TitleKey, _failed_item.MessageKey), ReportProgress.Progress(_start_progress, _end_progress, _fix_count + 61));

                            _fix_count++;
                        }
                        Logger.log(String.Format("Job Model Information {0}", JsonConvert.SerializeObject(_job_model)), Logger.Severity.Error);

                        throw new System.ArgumentException(string.Format("Cannot create job"));

                    }
                    _mrp_api.task().progress(_task_id, "Creating new job", ReportProgress.Progress(_start_progress, _end_progress, 65));
                    Guid jobId = _dt.job().CreateJob((new CreateOptionsModel
                    {
                        JobOptions = _job_model,
                        JobCredentials = jobCreds,
                        JobType = DT_JobTypes.DR_Full_Recovery
                    })).Result;

                    _mrp_api.task().progress(_task_id, String.Format("Job created. Starting job id {0}", jobId), ReportProgress.Progress(_start_progress, _end_progress, 66));
                    _dt.job().StartJob(jobId).Wait();

                    _mrp_api.task().progress(_task_id, String.Format("Registering job {0} with portal", jobId), ReportProgress.Progress(_start_progress, _end_progress, 67));
                    _mrp_api.managementobject().updatemanagementobject(new MRPManagementobjectType()
                    {
                        id = _managementobject.id,
                        moid = jobId.ToString(),
                        moname = jobInfo.JobOptions.Name,
                        motype = DT_JobTypes.DR_Full_Recovery
                    });
                    _mrp_api.task().progress(_task_id, "Waiting for sync process to start", ReportProgress.Progress(_start_progress, _end_progress, 71));

                    JobInfoModel jobinfo = _dt.job().GetJob(jobId).Result;
                    while (true)
                    {
                        if (jobinfo.Statistics.CoreConnectionDetails != null)
                        {
                            while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Mirror)
                            {
                                Thread.Sleep(new TimeSpan(0, 0, 30));
                                jobinfo = _dt.job().GetJob(jobId).Result;
                            }
                            break;
                        }
                        else
                        {
                            Thread.Sleep(5000);
                            jobinfo = _dt.job().GetJob(jobId).Result;
                            
                        }
                    }
                    _mrp_api.task().progress(_task_id, String.Format("Sync process started at {0}", jobinfo.Statistics.CoreConnectionDetails.StartTime), 75);

                    _mrp_api.task().progress(_task_id, String.Format("Successfully created disaster recovery job between {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);

                    MRPWorkloadType _update_workload = new MRPWorkloadType();
                    _update_workload.id = _target_workload.id;
                    _update_workload.dt_installed = true;
                    _mrp_api.workload().updateworkload(_update_workload);

                    DTJobPoller.PollerDo(_managementobject);


                }

            }
        }
    }
}

