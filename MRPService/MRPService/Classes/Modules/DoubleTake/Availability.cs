using MRPService.API;
using MRPService.API.Types.API;
using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Core.Contract;
using DoubleTake.Core.Contract.Connection;
using DoubleTake.Jobs.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MRPService.MRPService.Types.API;
using DoubleTake.Jobs.Contract1;
using MRPService.MRPService.Log;
using MRPService.Utilities;

namespace MRPService.DoubleTake
{
    public class Availability 
    {
        public static void dt_create_ha_syncjob(MRPTaskType payload)
        {
            API.ApiClient MRP = new API.ApiClient();
            try
            {
                MRPTaskWorkloadType _source_workload = payload.submitpayload.source;
                MRPTaskWorkloadType _target_workload = payload.submitpayload.target;
                MRPTaskRecoverypolicyType _recovery_policy = payload.submitpayload.servicestack.recoverypolicy;
                MRPTaskServicestackType _service_stack = payload.submitpayload.servicestack;

                MRP_DoubleTake _dt_endpoint = new MRP_DoubleTake(_source_workload.id, _target_workload.id);

                MRP.task().progress(payload, "Verifying license status on both source and target workloads", 2);
                if (!_dt_endpoint.ManagementService().check_license_status())
                {
                    MRP.task().failcomplete(payload, String.Format("Invalid license detected on workloads."));
                    return;
                }
                MRP.task().progress(payload, "Creating sync process", 4);


                    
                MRP.task().progress(payload, "Creating JobManager process", 5);
                IJobManager iJobMgr = _dt_endpoint.Common().JobManager();

                MRP.task().progress(payload, "Creating WorkloadManager process", 6);
                IWorkloadManager workloadMgr = _dt_endpoint.Common().WorkloadManager(DT_WorkloadType.Source);

                MRP.task().progress(payload, "Creating JobConfigurationVerifier process", 7);
                IJobConfigurationVerifier VerifierFactory = _dt_endpoint.Common().ConfigurationVerifier();



                JobInfo[] _jobs = iJobMgr.GetJobs();
                String[] _source_ips = _source_workload.iplist.Split(',');
                String[] _target_ips = _target_workload.iplist.Split(',');

                MRP.task().progress(payload, "Deteling current jobs associated to the source and target workloads", 11);
                int _count = 1;
                foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == DT_JobTypes.HA_Full_Failover && _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
                {
                    MRP.task().progress(payload, String.Format("Deleting existing HA jobs between {0} and {1}", _source_ips[0], _target_ips[0]), _count + 15);
                    DeleteOptions _delete_options = new DeleteOptions();
                    _delete_options.DeleteReplica = true;
                    _delete_options.DiscardTargetQueue = true;
                    ImageDeleteInfo _delete_info = new ImageDeleteInfo();
                    _delete_info.VhdDeleteAction = VhdDeleteActionType.DeleteAll;
                    _delete_info.DeleteImage = true;
                    _delete_options.ImageOptions = _delete_info;
                    iJobMgr.Stop(_delete_job.Id);
                    iJobMgr.Delete(_delete_job.Id, _delete_options);
                    try
                    {
                        while (true)
                        {
                            iJobMgr.GetJob(_delete_job.Id);
                            Thread.Sleep(2000);
                        }
                    }
                    catch (Exception) { }
                    _count += 1;
                }

                var workloadId = Guid.Empty;
                Workload wkld = (Workload)null;
                try
                {
                    workloadId = workloadMgr.Create(DT_JobTypes.HA_Full_Failover);
                    wkld = workloadMgr.GetWorkload(workloadId);
                }
                finally
                {
                    workloadMgr.Close(workloadId);
                }

                JobCredentials jobCreds = _dt_endpoint.Common().DTJobCredentials();

                RecommendedJobOptions jobInfo = VerifierFactory.GetRecommendedJobOptions(
                    DT_JobTypes.HA_Full_Failover,
                    wkld,
                    jobCreds);

                jobInfo.JobOptions.FullWorkloadFailoverOptions = new FullWorkloadFailoverOptions() { CreateBackupConnection = false };
                //jobInfo.JobOptions.Name = payload.target_id;

                MRP.task().progress(payload, "Setting job options", 50);

                //DNSOptions _dnsOptions = new DNSOptions();
                //DnsDomainDetails _dnsDomainDetails = new DnsDomainDetails();
                //_dnsOptions.Domains = new Array[0](_dnsDomainDetails);
                //jobInfo.JobOptions.DnsOptions = _dnsOptions;
                // level = 2 and algorithm = 31 Compression is enabled at high level
                if (_recovery_policy.enablecompression)
                {
                    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
                }
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Newer;

                if (_recovery_policy.enablesnapshots)
                {
                    TimeSpan _snapshot_timespan = new TimeSpan();
                    switch(_recovery_policy.snapshotinterval)
                    {
                        case "Hours":
                            _snapshot_timespan = new TimeSpan(_recovery_policy.snapshotincrement, 0, 0);
                            break;
                        case "Minutes":
                            _snapshot_timespan = new TimeSpan(0, _recovery_policy.snapshotincrement, 0);
                            break;

                    }
                    SnapshotSchedule _snapshot = new SnapshotSchedule();
                    _snapshot.Interval = _snapshot_timespan;
                    _snapshot.IsEnabled = true;
                    _snapshot.MaxNumberOfSnapshots = _recovery_policy.snapshotcount;
                    _snapshot.StartTime = new DateTime();
                    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;
                }
                //if (_recovery_policy.enablebandwidthlimit)
                //{
                //    BandwidthSchedule _bandwidth = new BandwidthSchedule();

                //    _bandwidth.Current.
                //    BandwidthSpecification _bandwidth_specification = new BandwidthSpecification();
                //    _bandwidth_specification
                //    _bandwidth.Mode = BandwidthScheduleMode.Fixed;
                //    _bandwidth.Specifications
                //    jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.Schedule.Bandwidth
                //}
                jobInfo.JobOptions.CoreConnectionOptions.TargetAddress = Connection.find_working_ip(_target_workload.iplist, true);
                

                MRP.task().progress(payload, "Deteling current jobs associated to the source and target workloads", 55);

                ActivityToken activityToken = VerifierFactory.VerifyJobOptions(
                    DT_JobTypes.HA_Full_Failover,
                    jobInfo.JobOptions,
                    jobCreds);

                List<VerificationStep> steps = new List<VerificationStep>();
                VerificationTaskStatus status = VerifierFactory.GetVerificationStatus(activityToken);
                while (
                    status.Task.Status != ActivityCompletionStatus.Canceled &&
                    status.Task.Status != ActivityCompletionStatus.Completed &&
                    status.Task.Status != ActivityCompletionStatus.Faulted)
                {
                    Thread.Sleep(1000);
                    status = VerifierFactory.GetVerificationStatus(activityToken);
                }

                var failedSteps = status.Steps.Where(s => s.Status == VerificationStatus.Error);

                if (failedSteps.Any())
                {
                    MRP.task().failcomplete(payload, JsonConvert.SerializeObject(failedSteps));
                }

                Guid jobId = iJobMgr.Create(new CreateOptions
                {
                    JobOptions = jobInfo.JobOptions,
                    JobCredentials = jobCreds,
                    JobType = DT_JobTypes.HA_Full_Failover
                }, Guid.NewGuid());
                iJobMgr.Start(jobId);

                MRP.task().progress(payload, "Registering job with portal", 60);
                API.ApiClient _mrp = new API.ApiClient();
                //create job on portal
                _mrp.job().createjob(new MRPJobType()
                {
                    dt_job_id = jobId.ToString(),
                    job_type = DT_JobTypes.HA_Full_Failover,
                    workload_id = _target_workload.id,
                    source_workload_id = _source_workload.id,
                    servicestack_id = _service_stack.id 
                });

                MRP.task().progress(payload, "Waiting for sync process to start", 65);

                JobInfo jobinfo = iJobMgr.GetJob(jobId);
                while (jobinfo.Statistics.FullWorkloadJobDetails.ProtectionConnectionDetails == null)
                {
                    Thread.Sleep(1000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                while (jobinfo.Statistics.FullWorkloadJobDetails.ProtectionConnectionDetails.MirrorState == MirrorState.Unknown)
                {
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                Thread.Sleep(5000);
                jobinfo = iJobMgr.GetJob(jobId);
                MRP.task().progress(payload, "Sync process started", 70);
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState != MirrorState.Idle)
                {
                    if (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorBytesRemaining != 0)
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
                                MRP.task().progress(payload, progress, percentage);
                            }
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                MRP.task().progress(payload, String.Format("Successfully synchronized {0} to {1}",_source_workload.hostname, _target_workload.hostname), 95);

                MRP.task().successcomplete(payload, JsonConvert.SerializeObject(jobinfo));
            }
            catch (Exception e)
            {
                MRP.task().failcomplete(payload, String.Format("Create sync process failed: {0}", e.Message));
                Logger.log(String.Format("Error creating availbility sync job: {0}", e.Message), Logger.Severity.Error);
            }
        }
        //public static void dt_failover_ha(dynamic request)
        //{
        //    try
        //    {
        //        Guid _jobId = request.payload.dt.jobId;

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
        //        //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request.payload;
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
        //        MRP.task().progress(request, "Successfully synchronized workload to " + (String)request.payload.dt.recoverypolicy.repositorypath, 99);

        //        MRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
        //    }
        //    catch (Exception e)
        //    {
        //        MRP.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
        //    }
        //}
    }
}
