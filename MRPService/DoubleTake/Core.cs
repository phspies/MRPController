using MRPService.Portal;
using System;
using System.Net;
using System.ServiceModel;
using MRPService.LocalDatabase;
using MRPService.CloudMRP.Classes.Static_Classes;
using DoubleTake.Jobs.Contract;

namespace MRPService.DoubleTake
{
    public class Core
    {
        public static Workload _source_workload, _target_workload;
        public Core(MRP_DoubleTake _doubletake)
        {
            _source_workload = _doubletake._source_workload;
            _target_workload = _doubletake._target_workload;
        }

        public IJobManager JobManager()
        {
            Workload _current_workload = _target_workload;
            Uri joburl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobManager").Uri;           
            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
            jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobManager jobMgr = jobMgrFactory.CreateChannel();
            return jobMgr;
        }
        public IWorkloadManager WorkloadManager(DT_WorkloadType type)
        {
            Workload _current_workload = (type == DT_WorkloadType.Source ? _source_workload : _target_workload);
            Uri workloadurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/WorkloadManager").Uri;
            var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
            workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IWorkloadManager workloadMgr = workloadFactory.CreateChannel();
            return workloadMgr;
        }
        public IManagementService ManagementService(DT_WorkloadType type)
        {
            Workload _current_workload = (type == DT_WorkloadType.Source ? _source_workload : _target_workload);
            Uri url = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/Contract/ManagementService").Uri;
            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(url));
            MgtServiceFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService mgmtServiceMgr = MgtServiceFactory.CreateChannel();
            return mgmtServiceMgr;
        }

        public IJobConfigurationVerifier ConfigurationVerifier()
        {
            Workload _current_workload = _target_workload;
            Uri configurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobConfigurationVerifier").Uri;
            ChannelFactory<IJobConfigurationVerifier> configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
            configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IJobConfigurationVerifier configurationVrf = configurationVerifierFactory.CreateChannel();
            return configurationVrf;
        }
        public JobCredentials DTJobCredentials()
        {
            LocalDB db = new LocalDB();
            Credential _source_credential = db.Credentials.Find(_source_workload.credential_id);
            Credential _target_credential = db.Credentials.Find(_target_workload.credential_id);

            JobCredentials _creds = new JobCredentials()
            {
                SourceHostUri = new Uri("http://" + _source_credential.username + ":" + _source_credential.password + "@" + Connection.find_working_ip(_source_workload.iplist, true) + ":" + "6325"),
                TargetHostUri = new Uri("http://" + _target_credential.username + ":" + _target_credential.password + "@" + Connection.find_working_ip(_target_workload.iplist, true) + ":" + "6325")
            };
            return _creds;

        }
        private static UriBuilder BuildConnectionUrl(Workload workload, string method)
        {
            LocalDB db = new LocalDB();
            Credential _credential = db.Credentials.Find(workload.credential_id);


            UriBuilder _uri = new UriBuilder();
            int portNumber = 6325;
            string bindingScheme = "http://";
            _uri = new UriBuilder(bindingScheme, Connection.find_working_ip(workload.iplist, true), portNumber, method);
            _uri.UserName = Uri.EscapeDataString(_credential.username);
            _uri.Password = Uri.EscapeDataString(_credential.password);
            return _uri;
        }
        private static NetworkCredential GetCredentials(Workload _workload)
        {
            LocalDB db = new LocalDB();

            NetworkCredential credentials = new NetworkCredential();
            Credential _credential = db.Credentials.Find(_workload.credential_id);
            credentials.UserName = Uri.EscapeDataString(_credential.username);
            credentials.Password = Uri.EscapeDataString(_credential.password);
            credentials.Domain = Uri.EscapeDataString(String.IsNullOrEmpty(_credential.domain) ? "." : _credential.domain);
            return credentials;
        }
    }
}
