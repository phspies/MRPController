using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Communication;
using DoubleTake.Core.Contract;
using DoubleTake.Core.Contract.Connection;
using DoubleTake.Jobs.Contract;
using Microsoft.Win32;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
using System.ServiceModel;
using System.Threading;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class DT_DR
    {
        static String server = "";
        static string username = "";
        static string password = "";
        static string domain = "";
        static string remoteInstallFiles = @"C:\DTSetup";
        static private DateTime installEndTime;
        static public int InstallWaitTimeoutInSeconds { get { return _installWaitTimeoutInSeconds; } set { _installWaitTimeoutInSeconds = value; } }
        static private int _installWaitTimeoutInSeconds = 2700;
        static CloudMovey CloudMovey = null;
        static TasksObject tasks = null;
        static dynamic _payload = null;

        public static void dt_create_dr_syncjob(dynamic request)
        {
            CloudMovey CloudMovey = new CloudMovey(Global.apiBase, null, null);
            CloudMovey.task().progress(request, "Creating sync process", 5);
            try
            {
                bool _delete_current_job = (bool)request.payload.dt.delete_current_dt_job;
                bool _reuse_dt_images = (bool)request.payload.dt.reuse_dt_images;
                String joburl = BuildUrl(request, "/DoubleTake/Jobs/JobManager", 2) ;
                var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
                jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                IJobManager iJobMgr = jobMgrFactory.CreateChannel();

                String workloadurl = BuildUrl(request, "/DoubleTake/Common/WorkloadManager", 1);
                var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
                workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 1);
                workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                var workloadMgr = workloadFactory.CreateChannel();

                String configurl = BuildUrl(request, "/DoubleTake/Jobs/JobConfigurationVerifier", 2);
                var configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
                configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                JobInfo[] _jobs = iJobMgr.GetJobs();
                String[] _source_ips = ((string)request.payload.dt.source.ipaddress).Split(',');
                String[] _target_ips = ((string)request.payload.dt.target.ipaddress).Split(',');
                String jobTypeConstant = @"FullServerImageProtection";

                if (_delete_current_job)
                {
                    foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == jobTypeConstant &&  _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
                    { 
                        DoubleTake.Jobs.Contract.DeleteOptions _delete_options = new DoubleTake.Jobs.Contract.DeleteOptions();
                        _delete_options.DeleteReplica = true;
                        _delete_options.DiscardTargetQueue = true;
                        ImageDeleteInfo _delete_info = new ImageDeleteInfo();
                        _delete_info.VhdDeleteAction = VhdDeleteActionType.DeleteAll;
                        _delete_info.DeleteImage = true;
                        _delete_options.ImageOptions = _delete_info;
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
                var wkld = (Workload)null;
                try
                {
                    workloadId = workloadMgr.Create(jobTypeConstant);
                    wkld = workloadMgr.GetWorkload(workloadId);
                }
                finally
                {
                    workloadMgr.Close(workloadId);
                }

                IJobConfigurationVerifier iJobCfgVerifier = configurationVerifierFactory.CreateChannel();
                JobCredentials jobCreds = new JobCredentials
                {
                    SourceConnectionParameters = BuildConnectionParams(request, 1),
                    TargetConnectionParameters = BuildConnectionParams(request, 2)
                };

                RecommendedJobOptions jobInfo = iJobCfgVerifier.GetRecommendedJobOptions(
                    jobTypeConstant,
                    wkld,
                    jobCreds);
                //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request.payload;
                List<ImageVhdInfo> vhd = new List<ImageVhdInfo>();
                int i = 0;
                foreach (dynamic volume in request.payload.dt.source.volumes)
                {
                    String _repositorypath = request.payload.dt.recoverypolicy.repositorypath;
                    String _original_id = request.payload.dt.original.id;
                    String _volume = volume.driveletter;
                    Int16 _disksize = volume.disksize;
                    Char _shortvolume = _volume[0];
                    String _filename = _original_id + "_" + _shortvolume + ".vhdx";
                    String _failovergroup = (String)(request.payload.dt.failovergroup.group);
                    string absfilename = Path.Combine(_repositorypath, _failovergroup.ToLower().Replace(" ", "_"), _original_id, _filename);
                    vhd.Add(new ImageVhdInfo() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = _reuse_dt_images, FilePath = absfilename, SizeInMB = (_disksize * 1024) });
                    i += 1;
                }

                jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
                jobInfo.JobOptions.Name = (String)request.target_id;
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)request.target_id;

                ActivityToken activityToken = iJobCfgVerifier.VerifyJobOptions(
                    jobTypeConstant,
                    jobInfo.JobOptions,
                    jobCreds);

                List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
                DoubleTake.Jobs.Contract1.VerificationTaskStatus status = iJobCfgVerifier.GetVerificationStatus(activityToken);
                while (
                    status.Task.Status != ActivityCompletionStatus.Canceled &&
                    status.Task.Status != ActivityCompletionStatus.Completed &&
                    status.Task.Status != ActivityCompletionStatus.Faulted)
                {
                    Thread.Sleep(1000);
                    status = iJobCfgVerifier.GetVerificationStatus(activityToken);
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

                CloudMovey.task().progress(request, "Waiting for sync process to start", 6);

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
                while (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorState != MirrorState.Idle)
                {
                    if (jobinfo.Statistics.ImageProtectionJobDetails.ProtectionConnectionDetails.MirrorBytesRemaining != null)
                    {
                        long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                        if ((totalcomplete > 0) && (totalstorage > 0))
                        {
                            double percentage = (((double)totalcomplete / (double)totalstorage) * 100) + 6;
                            CloudMovey.task().progress(request, progress, percentage);
                        }
                    }
                    Thread.Sleep(30000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                CloudMovey.task().successcomplete(request, "Successfully synchronized workload to " + (String)request.payload.dt.recoverypolicy.repositorypath);
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
            }
        }
        public static void dt_create_dr_seedjob(dynamic request)
        {
            CloudMovey CloudMovey = new CloudMovey(Global.apiBase, null, null);
            CloudMovey.task().progress(request, "Creating seed process", 5);
            try
            {
                String joburl = BuildUrl(request, "/DoubleTake/Jobs/JobManager",2);
                var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
                jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                IJobManager iJobMgr = jobMgrFactory.CreateChannel();

                String workloadurl = BuildUrl(request, "/DoubleTake/Common/WorkloadManager",1);
                var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
                workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 1);
                workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                var workloadMgr = workloadFactory.CreateChannel();

                String configurl = BuildUrl(request, "/DoubleTake/Jobs/JobConfigurationVerifier",2);
                var configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
                configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                String jobTypeConstant = @"FullServerImageProtection";
                var workloadId = Guid.Empty;
                var wkld = (Workload)null;
                try
                {
                    workloadId = workloadMgr.Create(jobTypeConstant);
                    wkld = workloadMgr.GetWorkload(workloadId);
                }
                finally
                {
                    workloadMgr.Close(workloadId);
                }

                var iJobCfgVerifier = configurationVerifierFactory.CreateChannel();
                var jobCreds = new JobCredentials
                {
                    SourceConnectionParameters = BuildConnectionParams(request, 1),
                    TargetConnectionParameters = BuildConnectionParams(request, 2)
                };

                RecommendedJobOptions jobInfo = iJobCfgVerifier.GetRecommendedJobOptions(
                    jobTypeConstant,
                    wkld,
                    jobCreds);
                //jobInfo.JobOptions.ImageProtectionOptions.ImageName = request.payload;
                List<ImageVhdInfo> vhd = new List<ImageVhdInfo>();
                int i = 0;
                foreach(dynamic volume in request.payload.dt.source.volumes)
                {
                    String _repositorypath = request.payload.dt.recoverypolicy.repositorypath;
                    String _target_id = request.target_id;
                    String _volume = volume.driveletter;
                    Int16 _disksize = volume.disksize;
                    Char _shortvolume = _volume[0];
                    String _filename = _target_id + "_" + _shortvolume + ".vhdx";
                    String _failovergroup = (String)(request.payload.dt.failovergroup.group);
                    string absfilename = Path.Combine(_repositorypath, _failovergroup.ToLower().Replace(" ", "_"), _target_id, _filename);
                    vhd.Add(new ImageVhdInfo() { FormatType = "ntfs", VolumeLetter = _shortvolume.ToString(), UseExistingVhd = false, FilePath = absfilename, SizeInMB = (_disksize * 1024)});
                    i += 1;
                }

                jobInfo.JobOptions.ImageProtectionOptions.VhdInfo = vhd.ToArray();
                jobInfo.JobOptions.Name = (String)request.target_id;
                jobInfo.JobOptions.ImageProtectionOptions.ImageName = (String)request.target_id;

                ActivityToken activityToken = iJobCfgVerifier.VerifyJobOptions(
                    jobTypeConstant,
                    jobInfo.JobOptions,
                    jobCreds);

                List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
                DoubleTake.Jobs.Contract1.VerificationTaskStatus status = iJobCfgVerifier.GetVerificationStatus(activityToken);
                while (
                    status.Task.Status != ActivityCompletionStatus.Canceled &&
                    status.Task.Status != ActivityCompletionStatus.Completed &&
                    status.Task.Status != ActivityCompletionStatus.Faulted)
                {
                    Thread.Sleep(1000);
                    status = iJobCfgVerifier.GetVerificationStatus(activityToken);
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
                Thread.Sleep(5000);

                CloudMovey.task().progress(request, "Waiting for seeding process to start", 6);

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
                while (!jobinfo.Status.CanCreateImageRecovery)
                {
                    if (jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining != null)
                    {
                        long totalstorage = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        long totalcomplete = ((long)jobinfo.Statistics.CoreConnectionDetails.MirrorBytesSent) / 1024 / 1024 ;
                        String progress = String.Format("{0}MB of {1}MB seeded", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                        if ((totalcomplete > 0) && (totalstorage > 0))
                        {
                            double percentage = (((double)totalcomplete / (double)totalstorage) * 100) + 6;
                            CloudMovey.task().progress(request, progress, percentage);
                        }
                    }
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }

                //iJobMgr.Stop(jobId);
                jobinfo = iJobMgr.GetJob(jobId);
                while (!jobinfo.Status.CanDelete)
                {
                    CloudMovey.task().progress(request, "Waiting for process to be deletable", 98);
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                DoubleTake.Jobs.Contract.DeleteOptions jobdelete = new DoubleTake.Jobs.Contract.DeleteOptions();
                jobdelete.DiscardTargetQueue = false;
                ImageDeleteInfo imagedeleteinfo = new ImageDeleteInfo();
                imagedeleteinfo.DeleteImage = true;
                imagedeleteinfo.VhdDeleteAction = VhdDeleteActionType.KeepAll;
                jobdelete.ImageOptions = imagedeleteinfo;

                CloudMovey.task().progress(request, "Destroying seeding process from DT engine", 99);
                iJobMgr.Delete(jobId, jobdelete);
                CloudMovey.task().successcomplete(request, "Successfully seeded workload to " + (String)request.payload.dt.recoverypolicy.repositorypath);
            }
            catch(Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create seed process failed: {0}", e.Message));
            }
        }
        public static void dt_create_dr_restorejob(dynamic request)
        {
            CloudMovey CloudMovey = new CloudMovey(Global.apiBase, null, null);
            try
            {
                CloudMovey.task().progress(request, "Creating JobManager process", 5);
                String joburl = BuildUrl(request, "/DoubleTake/Jobs/JobManager", 2);
                var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
                jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                IJobManager iJobMgr = jobMgrFactory.CreateChannel();

                CloudMovey.task().progress(request, "Creating WorkloadManager process", 7);
                String workloadurl = BuildUrl(request, "/DoubleTake/Common/WorkloadManager", 1);
                var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
                workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 1);
                workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                IWorkloadManager workloadMgr = workloadFactory.CreateChannel();

                CloudMovey.task().progress(request, "Creating ManagementService process", 9);
                String url = BuildUrl(request, "/DoubleTake/Common/Contract/ManagementService", 1);
                ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(url));
                MgtServiceFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 1);
                MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();

                CloudMovey.task().progress(request, "Creating JobConfigurationVerifier process", 11);
                String configurl = BuildUrl(request, "/DoubleTake/Jobs/JobConfigurationVerifier", 2);
                var configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
                configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(request, 2);
                configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;


                String jobTypeConstant = @"FullServerImageRecovery";
                //Get all images on the repository server
                ImageInfo[] imageInfos = iMgrSrc.GetImages(null);

                //Get the image of source server
                Guid sourceServerImageID = Guid.Empty;
                String sourceServerName = request.payload.dt.original.hostname;
                sourceServerImageID = imageInfos.First(x => x.ImageType == ImageType.FullServer && x.SourceName == sourceServerName).Id;
                if (sourceServerImageID == Guid.Empty)
                {
                    CloudMovey.task().failcomplete(request, String.Format("Source Server image not found on Repository Server: {0}",sourceServerName));
                    return;
                }
                CloudMovey.task().progress(request, String.Format("Found server image: {0}",sourceServerImageID), 13);


                // First step to creating a job is creating a Workload on the source
                Workload workload = (Workload)null;
                try
                {

                    Guid workloadId = Guid.Empty;
                    try
                    {
                        // Create a handle to a DR Recovery workload on the Repository Server that has the source image
                        workloadId = workloadMgr.CreateUsingImage(jobTypeConstant, sourceServerImageID, Guid.Empty /*Snapshot id*/);

                        //// Get the workload for use when creating the job
                        workload = workloadMgr.GetWorkload(workloadId);

                    }
                    finally
                    {
                        // Close workload on the source
                        workloadMgr.Close(workloadId);
                    }
                }
                finally
                {
                    // Close the client channel
                    if (workloadMgr != null)
                        ((IDisposable)workloadMgr).Dispose();
                }


                CloudMovey.task().progress(request, String.Format("Verifying restore job information"), 15);

                var iJobCfgVerifier = configurationVerifierFactory.CreateChannel();
                var jobCreds = new JobCredentials
                {
                    SourceConnectionParameters = BuildConnectionParams(request, 1),
                    TargetConnectionParameters = BuildConnectionParams(request, 2)
                };

                RecommendedJobOptions jobInfo = iJobCfgVerifier.GetRecommendedJobOptions(
                    jobTypeConstant,
                    workload,
                    jobCreds);

                ActivityToken activityToken = iJobCfgVerifier.VerifyJobOptions(
                    jobTypeConstant,
                    jobInfo.JobOptions,
                    jobCreds);

                List<DoubleTake.Jobs.Contract1.VerificationStep> steps = new List<DoubleTake.Jobs.Contract1.VerificationStep>();
                DoubleTake.Jobs.Contract1.VerificationTaskStatus status = iJobCfgVerifier.GetVerificationStatus(activityToken);
                while (
                    status.Task.Status != ActivityCompletionStatus.Canceled &&
                    status.Task.Status != ActivityCompletionStatus.Completed &&
                    status.Task.Status != ActivityCompletionStatus.Faulted)
                {
                    Thread.Sleep(1000);
                    status = iJobCfgVerifier.GetVerificationStatus(activityToken);
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
                Thread.Sleep(5000);

                CloudMovey.task().progress(request, "Waiting for restore process to start", 17);

                JobInfo jobinfo = iJobMgr.GetJob(jobId);
                while (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails == null)
                {
                    Thread.Sleep(1000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                while (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorState == DoubleTake.Core.Contract.Connection.MirrorState.Unknown)
                {
                    Thread.Sleep(5000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                while (!jobinfo.Status.CanCreateImageRecovery)
                {
                    if (jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesRemaining != null)
                    {
                        long totalstorage = ((long)jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesRemaining + (long)jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesSent) / 1024 / 1024;
                        long totalcomplete = ((long)jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesSent + (long)jobinfo.Statistics.ImageRecoveryJobDetails.RecoveryConnectionDetails.MirrorBytesSkipped) / 1024 / 1024;
                        String progress = String.Format("{0}MB of {1}MB restored", totalcomplete.ToString("N1", CultureInfo.InvariantCulture), totalstorage.ToString("N1", CultureInfo.InvariantCulture));
                        if ((totalcomplete > 0) && (totalstorage > 0))
                        {
                            double percentage = (((double)totalcomplete / (double)totalstorage) * 100) - 17;
                            CloudMovey.task().progress(request, progress, percentage);
                        }
                    }
                    Thread.Sleep(10000);
                    jobinfo = iJobMgr.GetJob(jobId);
                }
                CloudMovey.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo = iJobMgr.GetJob(jobId)));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create restore process failed: {0}", e.Message));
            }
        }

        private static string find_working_ip(dynamic payload, int type)
        {
            ConnectionOptions connection = new ConnectionOptions();

            String ipaddresslist = null;
            if (type==0){
                ipaddresslist = payload.payload.dt.ipaddress;
            } else if (type==1) {
                ipaddresslist = payload.payload.dt.source.ipaddress;
            } else if (type==2) {
                ipaddresslist = payload.payload.dt.target.ipaddress;
            }
            String workingip = null;
            Ping testPing = new Ping();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                PingReply reply = testPing.Send(ip, 1000);
                if (reply != null)
                {
                    workingip = ip;
                    break;
                }
            }
            testPing.Dispose();
            //check for IPv6 address
            IPAddress _check_ip = IPAddress.Parse(workingip);
#pragma warning disable CS0436 // Type conflicts with imported type
            if (_check_ip.AddressFamily.ToString() == AddressFamily.InterNetworkV6.ToString())
#pragma warning restore CS0436 // Type conflicts with imported type
            {
                String _server = server;
                _server = _server.Replace(":", "-");
                _server = _server.Replace("%", "s");
                _server = _server + ".ipv6-literal.net";
                workingip = _server;
            }
            return workingip;
        }

        private static String BuildUrl(dynamic request, String method, int type)
        {
            int portNumber = 6325;
            string bindingScheme = "http://";
            return new UriBuilder(bindingScheme, find_working_ip(request, type) ,portNumber, method).ToString();
        }
        private static ServiceConnectionParameters BuildConnectionParams(dynamic request, int type)
        {
            ServiceConnectionParameters connection = new ServiceConnectionParameters();
            switch (type)
            {
                case 1:
                    connection.Address = find_working_ip(request, 1);
                    connection.UserName = Uri.EscapeDataString((String)request.payload.dt.source.username);
                    connection.Password = Uri.EscapeDataString((String)request.payload.dt.source.password);
                    connection.Domain = Uri.EscapeDataString((String)request.payload.dt.source.domain);
                    break;
                case 2:
                    connection.Address = find_working_ip(request, 2);
                    connection.UserName = Uri.EscapeDataString((String)request.payload.dt.target.username);
                    connection.Password = Uri.EscapeDataString((String)request.payload.dt.target.password);
                    connection.Domain = Uri.EscapeDataString((String)request.payload.dt.target.domain);
                    break;
            }
            connection.Port = 6325;
            return connection;
            
        }
        private static NetworkCredential GetCredentials(dynamic payload, int type)
        {
            NetworkCredential credentials = null;
            switch (type)
            {
                case 0:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.username, Password = payload.payload.dt.password, Domain = payload.payload.dt.domain };
                    break;
                case 1:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.source.username, Password = payload.payload.dt.source.password, Domain = payload.payload.dt.source.domain };
                    break;
                case 2:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.target.username, Password = payload.payload.dt.target.password, Domain = payload.payload.dt.target.domain };
                    break;
            }
            return credentials;
        }
    }
}
