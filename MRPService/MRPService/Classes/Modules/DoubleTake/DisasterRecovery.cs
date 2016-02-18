using MRPService.Portal;
using MRPService.MRPService.Types.API;

namespace MRPService.DoubleTake
{
    public class MRPDoubleTake_DisasterRecovery : Core
    {
        public MRPDoubleTake_DisasterRecovery(MRP_DoubleTake cmdoubletake) : base(cmdoubletake) { }

        static CloudMRPPortal CloudMRP = null;
        static MRPTaskListType tasks = null;
        static dynamic _payload = null;

        //public static void dt_create_dr_syncjob(MRPTaskType request)
        //{
        //    CloudMRP.task().progress(request, "Creating sync process", 5);
        //    try
        //    {
        //        bool _delete_current_job = (bool)request.submitpayload.dt.delete_current_dt_job;
        //        bool _reuse_dt_images = (bool)request.submitpayload.dt.reuse_dt_images;

        //        IJobManager iJobMgr = JobManager().CreateChannel();
        //        IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();
        //        IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

        //        JobInfo[] _jobs = iJobMgr.GetJobs();
        //        String[] _source_ips = _source_workload.iplist.Split(',');
        //        String[] _target_ips = _target_workload.iplist.Split(',');
        //        String jobTypeConstant = @"FullWorkloadImageProtection";

        //        if (_delete_current_job)
        //        {
        //            foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == jobTypeConstant && _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
        //            {
        //                CloudMRP.task().progress(request, String.Format("Deleting existing full workload protection jobs between {0} and {1}", _source_ips[0], _target_ips[0]), 10);

        //                DTJobs.DeleteOptions _delete_options = new DTJobs.DeleteOptions();
        //                _delete_options.DeleteReplica = true;
        //                _delete_options.DiscardTargetQueue = true;
        //                ImageDeleteInfo _delete_info = new ImageDeleteInfo();
        //                _delete_info.VhdDeleteAction = VhdDeleteActionType.DeleteAll;
        //                _delete_info.DeleteImage = true;
        //                _delete_options.ImageOptions = _delete_info;
        //                ImageVhdInfo[] _vhdinfo = _delete_job.Options.ImageProtectionOptions.VhdInfo;
        //                int _index = 0;
        //                String[] _vhdfullpaths = new string[_vhdinfo.Count()];
        //                foreach (ImageVhdInfo _vhd in _vhdinfo)
        //                {
        //                    _vhdfullpaths[_index] = _vhd.FilePath;
        //                    _index++;
        //                }
        //                _delete_options.ImageOptions.VhdsToDelete = _vhdfullpaths;
        //                iJobMgr.Stop(_delete_job.Id);
        //                iJobMgr.Delete(_delete_job.Id, _delete_options);
        //                try
        //                {
        //                    while (true)
        //                    {
        //                        iJobMgr.GetJob(_delete_job.Id);
        //                        Thread.Sleep(2000);
        //                    }
        //                }
        //                catch (Exception) { }
        //            }
        //        }


        //        var workloadId = Guid.Empty;
        //        var wkld = (Workload)null;
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
        //        int i = 0;
        //        foreach (dynamic volume in request.submitpayload.dt.source.volumes)
        //        {
        //            String _repositorypath = request.submitpayload.dt.recoverypolicy.repositorypath;
        //            String _original_id = request.submitpayload.dt.original.id;
        //            String _volume = volume.driveletter;
        //            Int16 _disksize = volume.disksize;
        //            Char _shortvolume = _volume[0];
        //            String _filename = _original_id + "_" + _shortvolume + ".vhdx";
        //            String _failovergroup = (String)(request.submitpayload.dt.failovergroup.group);
        //            string absfilename = Path.Combine(_repositorypath, _failovergroup.ToLower().Replace(" ", "_"), _original_id, _filename);
        //            vhd.Add(new ImageVhdInfo() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = _reuse_dt_images, FilePath = absfilename, SizeInMB = (_disksize * 1024) });
        //            i += 1;
        //        }

        //        jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
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
        //            CloudMRP.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
        //        }

        //        Guid jobId = iJobMgr.Create(new CreateOptions
        //        {
        //            JobOptions = jobInfo.JobOptions,
        //            JobCredentials = jobCreds,
        //            JobType = jobTypeConstant
        //        }, Guid.NewGuid());
        //        iJobMgr.Start(jobId);

        //        CloudMRP.task().progress(request, "Waiting for sync process to start", 11);

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
        //        CloudMRP.task().progress(request, "Sync process started", 12);
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
        //                        CloudMRP.task().progress(request, progress, percentage);
        //                    }
        //                }
        //            }
        //            Thread.Sleep(10000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        CloudMRP.task().progress(request, "Successfully synchronized workload to " + (String)request.submitpayload.dt.recoverypolicy.repositorypath, 99);
        //        CloudMRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
        //    }
        //    catch (Exception e)
        //    {
        //        CloudMRP.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
        //    }
        //}
        //public static void dt_create_dr_seedjob(dynamic request)
        //{
        //    CloudMRP.task().progress(request, "Creating seed process", 5);
        //    try
        //    {
        //        bool _delete_current_job = (bool)request.payload.dt.delete_current_dt_job;

        //        IJobManager iJobMgr = JobManager().CreateChannel();
        //        IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();
        //        IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();
        //        JobInfo[] _jobs = iJobMgr.GetJobs();
        //        String[] _source_ips = _source_workload.iplist.Split(',');
        //        String[] _target_ips = _target_workload.iplist.Split(',');
        //        String jobTypeConstant = @"FullWorkloadImageProtection";

        //        if (_delete_current_job)
        //        {
        //            foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == jobTypeConstant && _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
        //            {
        //                CloudMRP.task().progress(request, String.Format("Deleting existing full workload protection jobs between {0} and {1}", _source_ips[0], _target_ips[0]), 10);

        //                DoubleTake.Jobs.Contract.DeleteOptions _delete_options = new DoubleTake.Jobs.Contract.DeleteOptions();
        //                _delete_options.DeleteReplica = true;
        //                _delete_options.DiscardTargetQueue = true;
        //                ImageDeleteInfo _delete_info = new ImageDeleteInfo();
        //                _delete_info.VhdDeleteAction = VhdDeleteActionType.DeleteAll;
        //                _delete_info.DeleteImage = true;
        //                _delete_options.ImageOptions = _delete_info;
        //                ImageVhdInfo[] _vhdinfo = _delete_job.Options.ImageProtectionOptions.VhdInfo;
        //                int _index = 0;
        //                String[] _vhdfullpaths = new string[_vhdinfo.Count()];
        //                foreach (ImageVhdInfo _vhd in _vhdinfo)
        //                {
        //                    _vhdfullpaths[_index] = _vhd.FilePath;
        //                    _index++;
        //                }
        //                _delete_options.ImageOptions.VhdsToDelete = _vhdfullpaths;
        //                iJobMgr.Stop(_delete_job.Id);
        //                iJobMgr.Delete(_delete_job.Id, _delete_options);
        //                try
        //                {
        //                    while (true)
        //                    {
        //                        iJobMgr.GetJob(_delete_job.Id);
        //                        Thread.Sleep(2000);
        //                    }
        //                }
        //                catch (Exception) { }
        //            }
        //        }
        //        var workloadId = Guid.Empty;
        //        var wkld = (Workload)null;
        //        try
        //        {
        //            workloadId = workloadMgr.Create(jobTypeConstant);
        //            wkld = workloadMgr.GetWorkload(workloadId);
        //        }
        //        finally
        //        {
        //            workloadMgr.Close(workloadId);
        //        }

        //        var jobCreds = new JobCredentials
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
        //        int i = 0;
        //        foreach (dynamic volume in request.payload.dt.source.volumes)
        //        {
        //            String _repositorypath = request.payload.dt.recoverypolicy.repositorypath;
        //            String _target_id = request.target_id;
        //            String _volume = volume.driveletter;
        //            Int16 _disksize = volume.disksize;
        //            Char _shortvolume = _volume[0];
        //            String _filename = _target_id + "_" + _shortvolume + ".vhdx";
        //            String _failovergroup = (String)(request.payload.dt.failovergroup.group);
        //            string absfilename = Path.Combine(_repositorypath, _failovergroup.ToLower().Replace(" ", "_"), _target_id, _filename);
        //            vhd.Add(new ImageVhdInfo() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = false, FilePath = absfilename, SizeInMB = (_disksize * 1024) });
        //            i += 1;
        //        }

        //        jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
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
        //            CloudMRP.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
        //        }

        //        Guid jobId = iJobMgr.Create(new CreateOptions
        //        {
        //            JobOptions = jobInfo.JobOptions,
        //            JobCredentials = jobCreds,
        //            JobType = jobTypeConstant
        //        }, Guid.NewGuid());
        //        iJobMgr.Start(jobId);
        //        Thread.Sleep(5000);

        //        CloudMRP.task().progress(request, "Waiting for seeding process to start", 6);

        //        JobInfo jobinfo = iJobMgr.GetJob(jobId);
        //        while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails == null)
        //        {
        //            Thread.Sleep(1000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState == MirrorState.Unknown)
        //        {
        //            Thread.Sleep(5000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        while (!jobinfo.Status.CanCreateImageRecovery)
        //        {
        //            long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //            long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //            if ((totalcomplete > 0) && (totalstorage > 0))
        //            {
        //                double percentage = (((double)totalcomplete / (double)totalstorage) * 88);
        //                int _lastreport = 0;
        //                if (percentage.ToString().Length >= 1 && _lastreport != percentage.ToString()[0])
        //                {
        //                    _lastreport = percentage.ToString()[0];
        //                    String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
        //                    CloudMRP.task().progress(request, progress, percentage);
        //                }
        //            }
        //            Thread.Sleep(10000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }

        //        //iJobMgr.Stop(jobId);
        //        jobinfo = iJobMgr.GetJob(jobId);
        //        while (!jobinfo.Status.CanDelete)
        //        {
        //            CloudMRP.task().progress(request, "Waiting for process to be deletable", 98);
        //            Thread.Sleep(5000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        DTJobs.DeleteOptions jobdelete = new DTJobs.DeleteOptions();
        //        jobdelete.DiscardTargetQueue = false;
        //        ImageDeleteInfo imagedeleteinfo = new ImageDeleteInfo();
        //        imagedeleteinfo.DeleteImage = true;
        //        imagedeleteinfo.VhdDeleteAction = VhdDeleteActionType.KeepAll;
        //        jobdelete.ImageOptions = imagedeleteinfo;

        //        CloudMRP.task().progress(request, "Destroying seeding process from DT engine", 98);

        //        iJobMgr.Delete(jobId, jobdelete);
        //        CloudMRP.task().progress(request, "Successfully seeded workload to " + (String)request.payload.dt.recoverypolicy.repositorypath, 99);

        //        CloudMRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
        //    }
        //    catch (Exception e)
        //    {
        //        CloudMRP.task().failcomplete(request, String.Format("Create seed process failed: {0}", e.Message));
        //    }
        //}
        //public static void dt_create_dr_restorejob(dynamic request)
        //{
        //    try
        //    {
        //        CloudMRP.task().progress(request, "Creating JobManager process", 5);
        //        IJobManager iJobMgr = JobManager().CreateChannel();

        //        CloudMRP.task().progress(request, "Creating WorkloadManager process", 7);
        //        IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();

        //        CloudMRP.task().progress(request, "Creating ManagementService process", 9);
        //        IManagementService iMgrSrc = ManagementService(CMWorkloadType.Source).CreateChannel();

        //        CloudMRP.task().progress(request, "Creating JobConfigurationVerifier process", 11);
        //        IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

        //        String jobTypeConstant = @"FullWorkloadImageRecovery";
        //        //Get all images on the repository workload
        //        ImageInfo[] imageInfos = iMgrSrc.GetImages(null);

        //        //Get the image of source workload
        //        Guid sourceWorkloadImageID = Guid.Empty;
        //        String sourceWorkloadName = request.payload.dt.original.hostname;
        //        sourceWorkloadImageID = imageInfos.First(x => x.ImageType == ImageType.FullWorkload && x.SourceName == sourceWorkloadName).Id;
        //        if (sourceWorkloadImageID == Guid.Empty)
        //        {
        //            CloudMRP.task().failcomplete(request, String.Format("Source Workload image not found on Repository Workload: {0}", sourceWorkloadName));
        //            return;
        //        }
        //        CloudMRP.task().progress(request, String.Format("Found workload image: {0}", sourceWorkloadImageID), 13);


        //        // First step to creating a job is creating a Workload on the source
        //        Workload workload = (Workload)null;
        //        try
        //        {

        //            Guid workloadId = Guid.Empty;
        //            try
        //            {
        //                // Create a handle to a DR Recovery workload on the Repository Workload that has the source image
        //                workloadId = workloadMgr.CreateUsingImage(jobTypeConstant, sourceWorkloadImageID, Guid.Empty /*Snapshot id*/);

        //                //// Get the workload for use when creating the job
        //                workload = workloadMgr.GetWorkload(workloadId);

        //            }
        //            finally
        //            {
        //                // Close workload on the source
        //                workloadMgr.Close(workloadId);
        //            }
        //        }
        //        finally
        //        {
        //            // Close the client channel
        //            if (workloadMgr != null)
        //                ((IDisposable)workloadMgr).Dispose();
        //        }


        //        CloudMRP.task().progress(request, String.Format("Verifying restore job information"), 15);

        //        var jobCreds = new JobCredentials
        //        {
        //            SourceConnectionParameters = DTConnectionParams(_source_workload),
        //            TargetConnectionParameters = DTConnectionParams(_target_workload)
        //        };

        //        RecommendedJobOptions jobInfo = VerifierFactory.GetRecommendedJobOptions(
        //            jobTypeConstant,
        //            workload,
        //            jobCreds);

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
        //            CloudMRP.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
        //        }

        //        Guid jobId = iJobMgr.Create(new CreateOptions
        //        {
        //            JobOptions = jobInfo.JobOptions,
        //            JobCredentials = jobCreds,
        //            JobType = jobTypeConstant
        //        }, Guid.NewGuid());
        //        iJobMgr.Start(jobId);
        //        Thread.Sleep(5000);

        //        CloudMRP.task().progress(request, "Waiting for restore process to start", 17);

        //        JobInfo jobinfo = iJobMgr.GetJob(jobId);
        //        while (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails == null)
        //        {
        //            Thread.Sleep(1000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        while (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorState == DoubleTake.Core.Contract.Connection.MirrorState.Unknown)
        //        {
        //            Thread.Sleep(5000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        while (!jobinfo.Status.CanCreateImageRecovery)
        //        {
        //            if (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesRemaining != null)
        //            {
        //                long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //                long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
        //                if ((totalcomplete > 0) && (totalstorage > 0))
        //                {
        //                    double percentage = (((double)totalcomplete / (double)totalstorage) * 88);
        //                    int _lastreport = 0;
        //                    if (percentage.ToString().Length >= 1 && _lastreport != percentage.ToString()[0])
        //                    {
        //                        _lastreport = percentage.ToString()[0];
        //                        String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
        //                        CloudMRP.task().progress(request, progress, percentage);
        //                    }
        //                }
        //            }
        //            Thread.Sleep(10000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        CloudMRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo = iJobMgr.GetJob(jobId)));
        //    }
        //    catch (Exception e)
        //    {
        //        CloudMRP.task().failcomplete(request, String.Format("Create restore process failed: {0}", e.Message));
        //    }
        //}
        //public static void dt_failover_dr(dynamic request)
        //{
        //    try
        //    {
        //        Guid _jobId = request.payload.dt.jobId;

        //        CloudMRP.task().progress(request, "Creating JobManager process", 5);
        //        IJobManager iJobMgr = JobManager().CreateChannel();

        //        CloudMRP.task().progress(request, "Creating WorkloadManager process", 7);
        //        IWorkloadManager workloadMgr = WorkloadManager().CreateChannel();

        //        CloudMRP.task().progress(request, "Creating ManagementService process", 9);
        //        IManagementService iMgrSrc = ManagementService(CMWorkloadType.Source).CreateChannel();

        //        CloudMRP.task().progress(request, "Creating JobConfigurationVerifier process", 11);
        //        IJobConfigurationVerifier VerifierFactory = ConfigurationVerifier().CreateChannel();

        //        JobInfo _jobInfo = iJobMgr.GetJob(_jobId);
        //        FailoverOptions _failoverOptions = new FailoverOptions();
        //        _failoverOptions.FailoverMode = FailoverMode.Live;
        //        _failoverOptions.FailoverType = FailoverType.Manual;
        //        ActivityToken _token = iJobMgr.Failover(_jobId, _failoverOptions);


        //        String jobTypeConstant = @"FullWorkloadImageRecovery";
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
        //            CloudMRP.task().failcomplete(request, JsonConvert.SerializeObject(failedSteps));
        //        }

        //        Guid jobId = iJobMgr.Create(new CreateOptions
        //        {
        //            JobOptions = jobInfo.JobOptions,
        //            JobCredentials = jobCreds,
        //            JobType = jobTypeConstant
        //        }, Guid.NewGuid());
        //        iJobMgr.Start(jobId);

        //        CloudMRP.task().progress(request, "Waiting for sync process to start", 11);

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
        //        CloudMRP.task().progress(request, "Sync process started", 12);
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
        //                        CloudMRP.task().progress(request, progress, percentage);
        //                    }
        //                }
        //            }
        //            Thread.Sleep(10000);
        //            jobinfo = iJobMgr.GetJob(jobId);
        //        }
        //        CloudMRP.task().progress(request, "Successfully synchronized workload to " + (String)request.payload.dt.recoverypolicy.repositorypath, 99);

        //        CloudMRP.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
        //    }
        //    catch (Exception e)
        //    {
        //        CloudMRP.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
        //    }
        //}

    }
}
