﻿using MRMPService.MRMPAPI.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;
using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;

namespace MRMPService.Tasks.DoubleTake
{
    public class Availability
    {
        public static async void CreateJob(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, MRPManagementobjectType _managementobject, float _start_progress, float _end_progress)
        {
            using (MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                try
                {

                    using (Doubletake _dt = new Doubletake(_source_workload, _target_workload))
                    {
                        _mrp_api.task().progress(_task_id, "Verifying license status on both source and target workloads", 2);
                        if (!_dt.management().CheckLicense(DT_JobTypes.HA_Full_Failover, _protectiongroup.organization_id))
                        {
                            _mrp_api.task().failcomplete(_task_id, String.Format("Invalid license detected on workloads."));
                            return;
                        }

                        List<JobInfoModel> _jobs = await _dt.job().GetJobs();
                        String[] _source_ips = _source_workload.iplist.Split(',');
                        String[] _target_ips = _target_workload.iplist.Split(',');

                        _mrp_api.task().progress(_task_id, "Deleting current jobs associated to the source and target workloads", 11);
                        int _count = 1;
                        foreach (JobInfoModel _delete_job in _jobs.Where(x => x.JobType == DT_JobTypes.HA_Full_Failover && _source_ips.Any(x.SourceServer.Host.Contains) && _target_ips.Any(x.TargetServer.Host.Contains)))
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Deleting existing HA jobs between {0} and {1}", _source_ips[0], _target_ips[0]), _count + 15);
                            _dt.job().DeleteJob(_delete_job.Id).Wait();
                            _count += 1;
                        }

                        var workloadId = Guid.Empty;
                        WorkloadModel wkld = (WorkloadModel)null;

                        workloadId = (await _dt.workload().CreateWorkload(DT_JobTypes.HA_Full_Failover)).Id;
                        wkld = await _dt.workload().GetWorkload(workloadId);


                        JobCredentialsModel jobCreds = _dt.job().CreateJobCredentials();

                        _mrp_api.task().progress(_task_id, "Fetching recommended job options", 20);

                        CreateOptionsModel jobInfo = await _dt.job().GetJobOptions(
                            wkld,
                            jobCreds,
                            DT_JobTypes.HA_Full_Failover);

                        jobInfo.JobOptions.FullServerFailoverOptions = new FullServerFailoverOptionsModel() { CreateBackupConnection = false };
                        //jobInfo.JobOptions.Name = _task_id.target_id;

                        _mrp_api.task().progress(_task_id, "Setting job options", 50);
                        jobInfo = SetOptions.set_job_options(_task_id, _source_workload, _target_workload, _protectiongroup, jobInfo, 51, 54);

                        _mrp_api.task().progress(_task_id, "Verifying job options and settings", 55);

                        //bool job_status;
                        //JobOptionsModel _job_model;
                        //List<VerificationStepModel> _failed_steps;
                        //Tuple< _dt.job().VerifyAndFixJobOptions(jobCreds, jobInfo.JobOptions, DT_JobTypes.HA_Full_Failover).Item2;

                        //JobOptionsModel _job_model =

                        _mrp_api.task().progress(_task_id, "Creating new job", 56);
                        Guid jobId = await _dt.job().CreateJob((new CreateOptionsModel
                        {
                            JobOptions = jobInfo.JobOptions,
                            JobCredentials = jobCreds,
                            JobType = DT_JobTypes.HA_Full_Failover
                        }));

                        _mrp_api.task().progress(_task_id, String.Format("Job created successfully. Starting job id ?", jobId), 57);
                        await _dt.job().StartJob(jobId);

                        _mrp_api.task().progress(_task_id, "Registering job with portal", 60);
                        _mrp_api.managementobject().createmanagementobject(new MRPManagementobjectType()
                        {
                            moid = jobId,
                            motype = DT_JobTypes.HA_Full_Failover,
                            target_workload_id = _target_workload.id,
                            source_workload_id = _source_workload.id,
                            protectiongroup_id = _protectiongroup.id
                        });


                        _mrp_api.task().progress(_task_id, "Waiting for sync process to start", 65);

                        JobInfoModel jobinfo = await _dt.job().GetJob(jobId);
                        while (jobinfo.Statistics.CoreConnectionDetails.TargetState == TargetStates.Unknown)
                        {
                            Thread.Sleep(5000);
                            jobinfo = await _dt.job().GetJob(jobId);
                        }
                        Thread.Sleep(5000);
                        jobinfo = await _dt.job().GetJob(jobId);

                        _mrp_api.task().progress(_task_id, "Sync process started", 70);
                        while (jobinfo.Statistics.CoreConnectionDetails.MirrorState != MirrorState.Idle)
                        {
                            if (jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining != 0)
                            {
                                long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                                long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                                if ((totalcomplete > 0) && (totalstorage > 0))
                                {
                                    double percentage = (((double)totalcomplete / (double)totalstorage) * 20);
                                    int _lastreport = 0;
                                    if (percentage.ToString().Length > 1 && _lastreport != percentage.ToString()[0])
                                    {
                                        _lastreport = percentage.ToString()[0];
                                        String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                                        _mrp_api.task().progress(_task_id, progress, percentage);
                                    }
                                }
                            }
                            Thread.Sleep(TimeSpan.FromMinutes(5));
                            jobinfo = await _dt.job().GetJob(jobId);
                        }
                        _mrp_api.task().progress(_task_id, String.Format("Successfully synchronized {0} to {1}", _source_workload.hostname, _target_workload.hostname), 95);

                        _mrp_api.task().successcomplete(_task_id, JsonConvert.SerializeObject(jobinfo));
                    }

                }
                catch (Exception e)
                {                   
                    Logger.log(String.Format("Error creating availbility sync job: {0}", e.Message), Logger.Severity.Error);
                    throw new Exception(String.Format("Create sync process failed: {0}", e.Message));
                }
            }
        }
        //public static void dt_failover_ha(dynamic request)
        //{
        //    try
        //    {
        //        Guid _jobId = request._task_id.dt.jobId;

        //        MRP.task().progress(request, "Creating JobManager process", 5);
        //        IJobManager iJobMgr = JobManager().CreateChannel();

        //        MRP.task().progress(request, "Creating WorkloadManager process", 7);
        //        IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();

        //        MRP.task().progress(request, "Creating ManagementService process", 9);
        //        IManagementService iMgrSrc = ManagementService(CMWorkloadType.Source).CreateChannel();

        //        MRP.task().progress(request, "Creating JobConfigurationVerifier process", 11);
        //        IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

        //        JobInfo _jobInfo = iJobMgr.GetJob(_jobId);
        //        FailoverOptions _failoverOptions = new FailoverOptions();
        //        _failoverOptions.FailoverMode = FailoverMode.Live;
        //        _failoverOptions.FailoverType = FailoverType.Manual;
        //        ActivityToken _token = iJobMgr.Failover(_jobId, _failoverOptions);


        //        String jobTypeConstant = @"FullWorkloadFailover";
        //        Guid workloadId = Guid.Empty;
        //        Workload wkld = (Workload)null;
        //        try
        //        {
        //            workloadId = workloadMgr.Create(jobTypeConstant);
        //            wkld = workloadMgr.GetWorkload(workloadId);
        //        }
        //        finally
        //        {
        //            workloadMgr.Close(workloadId);
        //        }

        //        JobCredentials jobCreds = new JobCredentials
        //        {
        //            SourceConnectionParameters = DTConnectionParams(_source_workload),
        //            TargetConnectionParameters = DTConnectionParams(_target_workload)
        //        };

        //        RecommendedJobOptions jobInfo = VerifierFactory.GetRecommendedJobOptions(
        //            jobTypeConstant,
        //            wkld,
        //            jobCreds);
        //        //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request._task_id;
        //        List<ImageVhdInfo> vhd = new List<ImageVhdInfo>();

        //        jobInfo.JobOptions.FullWorkloadFailoverOptions.CreateBackupConnection = false;
        //        jobInfo.JobOptions.Name = (String)request.target_id;
        //        jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)request.target_id;

        //        ActivityToken activityToken = VerifierFactory.VerifyJobOptions(
        //            jobTypeConstant,
        //            jobInfo.JobOptions,
        //            jobCreds);

        //        List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
        //        DoubleTake.Jobs.Contract1.VerificationTaskStatus status = VerifierFactory.GetVerificationStatus(activityToken);
        //        while (
        //            status.Task.Status != ActivityCompletionStatus.Canceled &&
        //            status.Task.Status != ActivityCompletionStatus.Completed &&
        //            status.Task.Status != ActivityCompletionStatus.Faulted)
        //        {
        //            Thread.Sleep(1000);
        //            status = VerifierFactory.GetVerificationStatus(activityToken);
        //        }

        //        var failedSteps = status.Steps.Where(s => s.Status == VerificationStatus.Error);

        //        if (failedSteps.Any())
        //        {
        //            MRP.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
        //        }

        //        Guid jobId = iJobMgr.Create(new CreateOptions
        //        {
        //            JobOptions = jobInfo.JobOptions,
        //            JobCredentials = jobCreds,
        //            JobType = jobTypeConstant
        //        }, Guid.NewGuid());
        //        iJobMgr.Start(jobId);

        //        MRP.task().progress(request, "Waiting for sync process to start", 11);

        //        JobInfo jobinfo = iJobMgr.GetJob(jobId);
        //        while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails == null)
        //        {
        //            Thread.Sleep(1000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState == DoubleTake.Core.Contract.Connection.MirrorState.Unknown)
        //        {
        //            Thread.Sleep(5000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        Thread.Sleep(5000);
        //        jobinfo = iJobMgr.GetJob(jobId);
        //        MRP.task().progress(request, "Sync process started", 12);
        //        while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState != MirrorState.Idle)
        //        {
        //            if (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorBytesRemaining != null)
        //            {
        //                long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //                long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //                if ((totalcomplete > 0) && (totalstorage > 0))
        //                {
        //                    double percentage = (((double)totalcomplete / (double)totalstorage) * 88);
        //                    int _lastreport = 0;
        //                    if (percentage.ToString().Length > 1 && _lastreport != percentage.ToString()[0])
        //                    {
        //                        _lastreport = percentage.ToString()[0];
        //                        String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
        //                        MRP.task().progress(request, progress, percentage);
        //                    }
        //                }
        //            }
        //            Thread.Sleep(10000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        MRP.task().progress(request, "Successfully synchronized workload to " + (String)request._task_id.dt.recoverypolicy.repositorypath, 99);

        //        MRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
        //    }
        //    catch (Exception e)
        //    {
        //        MRP.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
        //    }
        //}
    }
}
