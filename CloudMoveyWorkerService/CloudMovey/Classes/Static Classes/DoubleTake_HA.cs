using CloudMoveyWorkerService.CloudMovey.Types.API;
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

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class DT_HA
    {
         public static void dt_create_ha_syncjob(MoveyTaskType request)
        {
            CloudMovey CloudMovey = new CloudMovey();
            CloudMovey.task().progress(request, "Creating sync process", 5);
            try
            {
                bool _delete_current_job = (bool)request.submitpayload.dt.delete_current_dt_job;
                bool _reuse_dt_images = (bool)request.submitpayload.dt.reuse_dt_images;

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


                JobInfo[] _jobs = iJobMgr.GetJobs();
                String[] _source_ips = ((string)request.submitpayload.dt.source.ipaddress).Split(',');
                String[] _target_ips = ((string)request.submitpayload.dt.target.ipaddress).Split(',');
                String jobTypeConstant = @"FullWorkloadFailover";

                if (_delete_current_job)
                {
                    foreach (JobInfo _delete_job in _jobs.Where(x => x.JobType == jobTypeConstant &&  _source_ips.Any(x.SourceHostUri.Host.Contains) && _target_ips.Any(x.TargetHostUri.Host.Contains)))
                    {
                        CloudMovey.task().progress(request, String.Format("Deleting existing HA jobs between {0} and {1}",_source_ips[0], _target_ips[0]), 10);

                        DoubleTake.Jobs.Contract.DeleteOptions _delete_options = new DoubleTake.Jobs.Contract.DeleteOptions();
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
                CloudMovey.task().progress(request, "Successfully synchronized workload to " + (String)request.submitpayload.dt.recoverypolicy.repositorypath,99);

                CloudMovey.task().successcomplete(request, JsonConvert.SerializeObject(jobinfo));
            }
            catch (Exception e)
            {
                CloudMovey.task().failcomplete(request, String.Format("Create sync process failed: {0}", e.Message));
            }
        }
        public static void dt_failover_ha(dynamic request)
        {
            CloudMovey CloudMovey = new CloudMovey();
            try
            {
                Guid _jobId = request.payload.dt.jobId;

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

                jobInfo.JobOptions.FullWorkloadFailoverOptions.CreateBackupConnection = false;
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
                String _workingip = workingip;
                _workingip = _workingip.Replace(":", "-");
                _workingip = _workingip.Replace("%", "s");
                _workingip = _workingip + ".ipv6-literal.net";
                workingip = _workingip;
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
