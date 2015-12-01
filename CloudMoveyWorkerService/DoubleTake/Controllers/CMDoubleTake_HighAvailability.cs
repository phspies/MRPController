using CloudMoveyWorkerService.CMDoubleTake.Types;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Types.API;
using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Communication;
using DoubleTake.Core.Contract;
using DoubleTake.Core.Contract.Connection;
using DoubleTake.Jobs.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;
using DTJobs = DoubleTake.Jobs.Contract;


namespace CloudMoveyWorkerService.CMDoubleTake
{
    class CMDoubleTake_HighAvailability : Core
    {
        public CMDoubleTake_HighAvailability(CMDoubleTake cmdoubletake) : base(cmdoubletake) { }

        static CloudMoveyPortal CloudMovey = null;

        public static void dt_create_ha_syncjob(MoveyTaskType request)
        {
            CloudMoveyPortal CloudMovey = new CloudMoveyPortal();
            CloudMovey.task().progress(request, "Creating sync process", 5);
            try
            {
                bool _delete_current_job = (bool)request.submitpayload.dt.delete_current_dt_job;
                bool _reuse_dt_images = (bool)request.submitpayload.dt.reuse_dt_images;

                CloudMovey.task().progress(request, "Creating JobManager process", 5);
                IJobManager iJobMgr = JobManager().CreateChannel();

                CloudMovey.task().progress(request, "Creating WorkloadManager process", 7);
                IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();

                CloudMovey.task().progress(request, "Creating ManagementService process", 9);
                IManagementService iMgrSrc = ManagementService(CMWorkloadType.Source).CreateChannel();

                CloudMovey.task().progress(request, "Creating JobConfigurationVerifier process", 11);
                IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

                JobInfo[] _jobs = iJobMgr.GetJobs();
                String[] _source_ips = ((string)request.submitpayload.dt.source.ipaddress).Split(',');
                String[] _target_ips = ((string)request.submitpayload.dt.target.ipaddress).Split(',');
                String jobTypeConstant = @"FullWorkloadFailover";

                if (_delete_current_job)
                {
                    foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == jobTypeConstant && _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
                    {
                        CloudMovey.task().progress(request, String.Format("Deleting existing HA jobs between {0} and {1}", _source_ips[0], _target_ips[0]), 10);

                        DoubleTake.Jobs.Contract.DeleteOptions _delete_options = new DoubleTakeNS.Jobs.Contract.DeleteOptions();
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
                    }
                }

                var workloadId = Guid.Empty;
                Workload wkld = (Workload)null;
                try
                {
                    workloadId = workloadMgr.Create(jobTypeConstant);
                    wkld = workloadMgr.GetWorkload(workloadId);
                }
                finally
                {
                    workloadMgr.Close(workloadId);
                }

                JobCredentials jobCreds = new JobCredentials
                {
                    SourceConnectionParameters = DTConnectionParams(_source_workload),
                    TargetConnectionParameters = DTConnectionParams(_target_workload)
                };

                RecommendedJobOptions jobInfo = VerifierFactory.GetRecommendedJobOptions(
                    jobTypeConstant,
                    wkld,
                    jobCreds);
                //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request.payload;
                List<ImageVhdInfo> vhd = new List<ImageVhdInfo>();

                jobInfo.JobOptions.FullWorkloadFailoverOptions.CreateBackupConnection = false;
                jobInfo.JobOptions.Name = (String)request.target_id;
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)request.target_id;


                //DNSOptions _dnsOptions = new DNSOptions();
                //DnsDomainDetails _dnsDomainDetails = new DnsDomainDetails();
                //_dnsOptions.Domains = new Array[0](_dnsDomainDetails);
                //jobInfo.JobOptions.DnsOptions = _dnsOptions;
                // level = 2 and algorithm = 31 Compression is enabled at high level
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Level = 1;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.CompressionLevel.Algorithm = 21;
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters.ComparisonCriteria = MirrorComparisonCriteria.Newer;

                SnapshotSchedule _snapshot = new SnapshotSchedule();
                _snapshot.Interval = TimeSpan.Zero;
                _snapshot.IsEnabled = true;
                _snapshot.MaxNumberOfSnapshots = 10;
                _snapshot.StartTime = new DateTime();
                jobInfo.JobOptions.CoreConnectionOptions.ConnectionStartParameters.SnapshotSchedule = _snapshot;

                ActivityToken activityToken = VerifierFactory.VerifyJobOptions(
                    jobTypeConstant,
                    jobInfo.JobOptions,
                    jobCreds);

                List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
                DoubleTake.Jobs.Contract1.VerificationTaskStatus status = VerifierFactory.GetVerificationStatus(activityToken);
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
                    CloudMovey.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
                }

                Guid jobId = iJobMgr.Create(new CreateOptions
                {
                    JobOptions = jobInfo.JobOptions,
                    JobCredentials = jobCreds,
                    JobType = jobTypeConstant
                }, Guid.NewGuid());
                iJobMgr.Start(jobId);

                CloudMovey.task().progress(request, "Waiting for sync process to start", 11);

                JobInfo jobinfo = iJobMgr.GetJob(jobId);
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails == null)
                {
                    Thread.Sleep(1000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState == DoubleTakeNS.Core.Contract.Connection.MirrorState.Unknown)
                {
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                Thread.Sleep(5000);
                jobinfo = iJobMgr.GetJob(jobId);
                CloudMovey.task().progress(request, "Sync process started", 12);
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState != MirrorState.Idle)
                {
                    if (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorBytesRemaining != null)
                    {
                        long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        if ((totalcomplete > 0) && (totalstorage > 0))
                        {
                            double percentage = (((double)totalcomplete / (double)totalstorage) * 88);
                            int _lastreport = 0;
                            if (percentage.ToString().Length > 1 && _lastreport != percentage.ToString()[0])
                            {
                                _lastreport = percentage.ToString()[0];
                                String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                                CloudMovey.task().progress(request, progress, percentage);
                            }
                        }
                    }
                    Thread.Sleep(10000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                CloudMovey.task().progress(request, "Successfully synchronized workload to " + (String)request.submitpayload.dt.recoverypolicy.repositorypath, 99);

                CloudMovey.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
            }
        }
        public static void dt_failover_ha(dynamic request)
        {
            try
            {
                Guid _jobId = request.payload.dt.jobId;

                CloudMovey.task().progress(request, "Creating JobManager process", 5);
                IJobManager iJobMgr = JobManager().CreateChannel();

                CloudMovey.task().progress(request, "Creating WorkloadManager process", 7);
                IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();

                CloudMovey.task().progress(request, "Creating ManagementService process", 9);
                IManagementService iMgrSrc = ManagementService(CMWorkloadType.Source).CreateChannel();

                CloudMovey.task().progress(request, "Creating JobConfigurationVerifier process", 11);
                IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

                JobInfo _jobInfo = iJobMgr.GetJob(_jobId);
                FailoverOptions _failoverOptions = new FailoverOptions();
                _failoverOptions.FailoverMode = FailoverMode.Live;
                _failoverOptions.FailoverType = FailoverType.Manual;
                ActivityToken _token = iJobMgr.Failover(_jobId, _failoverOptions);


                String jobTypeConstant = @"FullWorkloadFailover";
                Guid workloadId = Guid.Empty;
                Workload wkld = (Workload)null;
                try
                {
                    workloadId = workloadMgr.Create(jobTypeConstant);
                    wkld = workloadMgr.GetWorkload(workloadId);
                }
                finally
                {
                    workloadMgr.Close(workloadId);
                }

                JobCredentials jobCreds = new JobCredentials
                {
                    SourceConnectionParameters = DTConnectionParams(_source_workload),
                    TargetConnectionParameters = DTConnectionParams(_target_workload)
                };

                RecommendedJobOptions jobInfo = VerifierFactory.GetRecommendedJobOptions(
                    jobTypeConstant,
                    wkld,
                    jobCreds);
                //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request.payload;
                List<ImageVhdInfo> vhd = new List<ImageVhdInfo>();

                jobInfo.JobOptions.FullWorkloadFailoverOptions.CreateBackupConnection = false;
                jobInfo.JobOptions.Name = (String)request.target_id;
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)request.target_id;

                ActivityToken activityToken = VerifierFactory.VerifyJobOptions(
                    jobTypeConstant,
                    jobInfo.JobOptions,
                    jobCreds);

                List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
                DoubleTake.Jobs.Contract1.VerificationTaskStatus status = VerifierFactory.GetVerificationStatus(activityToken);
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
                    CloudMovey.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
                }

                Guid jobId = iJobMgr.Create(new CreateOptions
                {
                    JobOptions = jobInfo.JobOptions,
                    JobCredentials = jobCreds,
                    JobType = jobTypeConstant
                }, Guid.NewGuid());
                iJobMgr.Start(jobId);

                CloudMovey.task().progress(request, "Waiting for sync process to start", 11);

                JobInfo jobinfo = iJobMgr.GetJob(jobId);
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails == null)
                {
                    Thread.Sleep(1000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState == DoubleTake.Core.Contract.Connection.MirrorState.Unknown)
                {
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                Thread.Sleep(5000);
                jobinfo = iJobMgr.GetJob(jobId);
                CloudMovey.task().progress(request, "Sync process started", 12);
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState != MirrorState.Idle)
                {
                    if (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorBytesRemaining != null)
                    {
                        long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        if ((totalcomplete > 0) && (totalstorage > 0))
                        {
                            double percentage = (((double)totalcomplete / (double)totalstorage) * 88);
                            int _lastreport = 0;
                            if (percentage.ToString().Length > 1 && _lastreport != percentage.ToString()[0])
                            {
                                _lastreport = percentage.ToString()[0];
                                String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                                CloudMovey.task().progress(request, progress, percentage);
                            }
                        }
                    }
                    Thread.Sleep(10000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                CloudMovey.task().progress(request, "Successfully synchronized workload to " + (String)request.payload.dt.recoverypolicy.repositorypath, 99);

                CloudMovey.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
            }
        }
    }
}
