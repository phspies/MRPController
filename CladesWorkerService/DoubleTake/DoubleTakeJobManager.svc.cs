using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using Newtonsoft.Json;
using DoubleTakeRestProxy;
using DoubleTakeManagementServiceRestProxy;
using DoubleTake.Jobs.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Core.Contract;
using DoubleTake.Common.Contract;
using DoubleTake.Jobs.Contract1;

namespace DoubleTakeJobManagerRestProxy
{
    public class DoubleTakeJobManager : IDoubleTakeJobManager
    {
        public System.Diagnostics.EventLog DoubleTakeProxyLog = new System.Diagnostics.EventLog("Application", ".", "Double-Take JSON Service");

        public String GetJobs(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }
            ChannelFactory<IJobManager> jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager iJobMgr = jobMgrFactory.CreateChannel();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(iJobMgr.GetJobs());
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }
            return json;
        }
        public String GetJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            ChannelFactory<IJobManager>jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager iJobMgr = jobMgrFactory.CreateChannel();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(iJobMgr.GetJob(request.jobid));
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }

            return json;
        }
        public String StartJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            ChannelFactory<IJobManager> jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager iJobMgr = jobMgrFactory.CreateChannel();

            var activityToken = iJobMgr.Start(request.jobid);
            var status = iJobMgr.GetActionStatus(activityToken);
            while (
                status.Status != ActivityCompletionStatus.Canceled &&
                status.Status != ActivityCompletionStatus.Completed &&
                status.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = iJobMgr.GetActionStatus(activityToken);
            }
            string json = JsonConvert.SerializeObject(new { state = JsonConvert.SerializeObject(status), jobId = request.jobid });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }

            return json;
        }
        public String FailoverJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            ChannelFactory<IJobManager> jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager iJobMgr = jobMgrFactory.CreateChannel();

            RecommendedFailoverOptions iJobFailoverOptions = iJobMgr.GetRecommendedFailoverOptions(request.jobid);
            iJobFailoverOptions.FailoverOptions.FailoverMode = FailoverMode.Live;

            ActivityToken activityToken = iJobMgr.Failover(request.jobid, iJobFailoverOptions.FailoverOptions);
            ActivityStatusEntry status = iJobMgr.GetActionStatus(activityToken);
            while (
                status.Status != ActivityCompletionStatus.Canceled &&
                status.Status != ActivityCompletionStatus.Completed &&
                status.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = iJobMgr.GetActionStatus(activityToken);
            }
            string json = JsonConvert.SerializeObject(new { state = JsonConvert.SerializeObject(status), jobId = request.jobid });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }
            return json;
        }
        public String FailbackJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }
            ChannelFactory<IJobManager> jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel =
            System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager iJobMgr = jobMgrFactory.CreateChannel();

            RecommendedFailbackOptions iJobFailbackOptions = iJobMgr.GetRecommendedFailbackOptions(request.jobid);

            ActivityToken activityToken = iJobMgr.Failback(request.jobid, iJobFailbackOptions.FailbackOptions);
            ActivityStatusEntry status = iJobMgr.GetActionStatus(activityToken);
            while (
                status.Status != ActivityCompletionStatus.Canceled &&
                status.Status != ActivityCompletionStatus.Completed &&
                status.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = iJobMgr.GetActionStatus(activityToken);
            }
            string json = JsonConvert.SerializeObject(new { state = JsonConvert.SerializeObject(status), jobId = request.jobid });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }
            return json;
        }
        public String CreateFailoverDRJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            String jobTypeConstant = @"FullServerImageRecovery";

            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));

            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var iJobMgr = jobMgrFactory.CreateChannel();

            var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Common/WorkloadManager").targetUri.Uri));

            workloadFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var workloadMgr = workloadFactory.CreateChannel();

            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));
            MgtServiceFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();

            if (request.imageid == Guid.Empty)
            {
                return JsonConvert.SerializeObject(new { status = "Failed", error = "Source server image not found on repository server"}); ;
            }

            // First step to creating a job is creating a Workload on the source
            Workload workload = (Workload)null;
            try
            {
                Guid workloadId = Guid.Empty;
                try
                {
                    // Create a handle to a DR Recovery workload on the Repository Server that has the source image
                    workloadId = workloadMgr.CreateUsingImage(jobTypeConstant, request.imageid, Guid.Empty /*Snapshot id*/);
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
            ChannelFactory<IJobConfigurationVerifier> configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>(
                "DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier",
                new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobConfigurationVerifier").targetUri.Uri));

            configurationVerifierFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            IJobConfigurationVerifier jobConfigurationVerifier = configurationVerifierFactory.CreateChannel();
            JobCredentials jobCreds = new JobCredentials
            {
                SourceHostUri = BuildUri(request, null).sourceShortUri.Uri,
                TargetHostUri = BuildUri(request, null).targetShortUri.Uri
            };
            RecommendedJobOptions recommendedOptions;
            try
            {
                // Get a job configuration verifier from the target
                // Once you have a source/target ready to go we can give you some "recommended" job options that will create a working job with default settings
                recommendedOptions = jobConfigurationVerifier.GetRecommendedJobOptions(jobTypeConstant, workload, jobCreds);
                //
                // This is where you would change the job options to look the way you want
                //
                // The verification process is asynchronous, so we start it off using VerifyJobOptions
                ActivityToken activityToken = jobConfigurationVerifier.VerifyJobOptions(
                    jobTypeConstant,
                    recommendedOptions.JobOptions,
                    jobCreds);

                // Then poll the status of the verification process by calling GetVerificationStatus
                List<VerificationStep> steps = new List<VerificationStep>();
                VerificationTaskStatus status = jobConfigurationVerifier.GetVerificationStatus(activityToken);
                while (
                    status.Task.Status != ActivityCompletionStatus.Canceled &&
                    status.Task.Status != ActivityCompletionStatus.Completed &&
                    status.Task.Status != ActivityCompletionStatus.Faulted)
                {
                    Thread.Sleep(1000);
                    status = jobConfigurationVerifier.GetVerificationStatus(activityToken);
                }
                Console.WriteLine();

                // Did any of the steps fail?
                IEnumerable<VerificationStep> failedSteps = status
                    .Steps
                    .Where(s => s.Status == VerificationStatus.Error);

                // If any verification step fails you cannot create the job
                if (failedSteps.Any())
                {
                    foreach (var step in failedSteps)
                    {
                        Console.WriteLine(step.Status + " -- " + step.MessageKey);
                    }
                }
                Console.WriteLine("Verification successful");
            }
            finally
            {
                // Close the client channel
                if (jobConfigurationVerifier != null)
                    ((IDisposable)jobConfigurationVerifier).Dispose();
            }

            jobMgrFactory = new ChannelFactory<IJobManager>(
                "DefaultBinding_IJobManager_IJobManager",
                new EndpointAddress(new Uri("http://172.31.204.175:6325/DoubleTake/Jobs/JobManager"))
                );

            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            }; 
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            iJobMgr = jobMgrFactory.CreateChannel();
            Guid jobId = Guid.Empty;
            try
            {
                // Create the job
                jobId = iJobMgr.Create(
                    new CreateOptions
                    {
                        JobOptions = recommendedOptions.JobOptions,
                        JobCredentials = jobCreds,
                        JobType = jobTypeConstant
                    },
                    Guid.Empty      // The Guid parameter is reserved but not currently used
                    );

                // You have to manually start the job after creating it
                // Controlling the job is an async process, much like verification
                // You can either fire and forget about it (monitor the job state elsewhere) or react to the job state programmatically
                // Here I'm just going to start the job and forget about it
                iJobMgr.Start(jobId);
            }
            finally
            {
                // Close the client channel
                if (iJobMgr != null)
                    ((IDisposable)iJobMgr).Dispose();
            }
            string json = JsonConvert.SerializeObject(new { state = "success", jobId = jobId });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }

            return json;
        }
        public String CreateDRJob(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));

            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential 
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var iJobMgr = jobMgrFactory.CreateChannel();


            var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Common/WorkloadManager").targetUri.Uri));

            workloadFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var workloadMgr = workloadFactory.CreateChannel();

            var configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobConfigurationVerifier").targetUri.Uri));

            configurationVerifierFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var workloadId = Guid.Empty;
            var wkld = (Workload)null;
            try
            {
                workloadId = workloadMgr.Create(request.jobtype);
                wkld = workloadMgr.GetWorkload(workloadId);
            }
            finally
            {
                workloadMgr.Close(workloadId);
            }

            var iJobCfgVerifier = configurationVerifierFactory.CreateChannel();
            var jobCreds = new JobCredentials
            {
                //SourceConnectionParameters = new ServiceConnectionParameters() { Address = "aaa"},                
                SourceHostUri = BuildUri(request,null).sourceShortUri.Uri,
                TargetHostUri = BuildUri(request,null).targetShortUri.Uri
            };

            var jobInfo = iJobCfgVerifier.GetRecommendedJobOptions(
                request.jobtype,
                wkld,
                jobCreds);

            var activityToken = iJobCfgVerifier.VerifyJobOptions(
                request.jobtype,
                jobInfo.JobOptions,
                jobCreds);

            var steps = new List<VerificationStep>();
            var status = iJobCfgVerifier.GetVerificationStatus(activityToken);
            while (
                status.Task.Status != ActivityCompletionStatus.Canceled &&
                status.Task.Status != ActivityCompletionStatus.Completed &&
                status.Task.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = iJobCfgVerifier.GetVerificationStatus(activityToken);
            }

            var failedSteps = status.Steps
                .Where(s => s.Status == VerificationStatus.Error);

            if (failedSteps.Any())
            {
                foreach (var step in failedSteps)
                {
                    Console.WriteLine(step.Status + " -- " + step.MessageKey);
                }
                Console.ReadKey(true);
                return JsonConvert.SerializeObject(new { state = status.Task.Status, failedSteps = failedSteps.Select(x => x.MessageKey) });
            }

            var jobId = iJobMgr.Create(new CreateOptions
            {
                JobOptions = jobInfo.JobOptions,
                JobCredentials = jobCreds,
                JobType = request.jobtype
            }, Guid.NewGuid());
            //iJobMgr.Start(jobId);
            string json = JsonConvert.SerializeObject(new { state = status.Task.Status, failedSteps = failedSteps.Select(x => x.MessageKey), jobId = jobId });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }

            return json;
        }
        public String UpdateCredentials(JobManagerRequest request)
        {
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + Newtonsoft.Json.JsonConvert.SerializeObject(request), EventLogEntryType.Information, 1); }

            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobManager").targetUri.Uri));

            jobMgrFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            var configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(BuildUri(request, "/DoubleTake/Jobs/JobConfigurationVerifier").targetUri.Uri));

            configurationVerifierFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = request.credentials.targetUserDomain,
                UserName = request.credentials.targetUserAccount,
                Password = request.credentials.targetUserPassword
            };
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            var iJobCfgVerifier = configurationVerifierFactory.CreateChannel();
            var iJobMgr = jobMgrFactory.CreateChannel();

            JobCredentials jobCreds = new JobCredentials
            {
                SourceHostUri = BuildUri(request, null).sourceShortUri.Uri,
                TargetHostUri = BuildUri(request, null).targetShortUri.Uri
            };

            var activityToken = iJobMgr.UpdateCredentials(request.jobid, jobCreds);
                                     
            var steps = new List<VerificationStep>();
            var status = iJobCfgVerifier.GetVerificationStatus(activityToken);
            while (
                status.Task.Status != ActivityCompletionStatus.Canceled &&
                status.Task.Status != ActivityCompletionStatus.Completed &&
                status.Task.Status != ActivityCompletionStatus.Faulted)
            {
                Thread.Sleep(1000);
                status = iJobCfgVerifier.GetVerificationStatus(activityToken);
            }

            var failedSteps = status.Steps
                .Where(s => s.Status == VerificationStatus.Error);

            if (failedSteps.Any())
            {
                return JsonConvert.SerializeObject(new { state = status.Task.Status, failedSteps = failedSteps.Select(x => x.MessageKey) });
            }
   
            string json = JsonConvert.SerializeObject(new { state = status.Task.Status, failedSteps = failedSteps.Select(x => x.MessageKey), jobId = request.jobid });
            if (Global.Debug) { DoubleTakeProxyLog.WriteEntry(System.Reflection.MethodBase.GetCurrentMethod().Name + " => " + json, EventLogEntryType.Information, 2); }
            return json;
        }
        private UriModel BuildUri(JobManagerRequest request, String method)
        {
            string portNumber = "6325";
            string bindingScheme = "http://";
            UriModel UriObject = new UriModel();

            if (request.credentials.sourceIPAddress != null)
            {
                UriObject.sourceShortUri = new UriBuilder(bindingScheme + request.credentials.sourceIPAddress + ":" + portNumber);
                UriObject.sourceShortUri.UserName = Uri.EscapeDataString(request.credentials.sourceUserAccount);
                UriObject.sourceShortUri.Password = Uri.EscapeDataString(request.credentials.sourceUserPassword);
                UriObject.sourceUri = new UriBuilder(bindingScheme + request.credentials.sourceIPAddress + ":" + portNumber + method);

            }
            if (request.credentials.targetIPAddress != null)
            {
                UriObject.targetShortUri = new UriBuilder(bindingScheme + request.credentials.targetIPAddress + ":" + portNumber);
                UriObject.targetShortUri.UserName = Uri.EscapeDataString(request.credentials.targetUserAccount);
                UriObject.targetShortUri.Password = Uri.EscapeDataString(request.credentials.targetUserPassword);
                UriObject.targetUri = new UriBuilder(bindingScheme + request.credentials.targetIPAddress + ":" + portNumber + method);
                Debug.WriteLine("ShortURI:" + UriObject.targetShortUri);
                Debug.WriteLine("LongURI:" + UriObject.targetUri);
            }
            return UriObject;
        }
    }
}
