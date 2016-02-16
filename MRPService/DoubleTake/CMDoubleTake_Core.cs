using MRPService.MRPDoubleTake.Types;
using MRPService.Portal;
using DoubleTake.Communication;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;
using MRPService.LocalDatabase;
using MRPService.CloudMRP.Classes.Static_Classes;

namespace MRPService.MRPDoubleTake
{
    public class MRPDoubleTake_Core
    {
        public static Workload _source_workload, _target_workload;
        static CloudMRPPortal CloudMRP = null;

        public MRPDoubleTake_Core(MRPDoubleTake _cmdoubletake)
        {
            _source_workload = _cmdoubletake._source_workload;
            _target_workload = _cmdoubletake._target_workload;
        }

        public ChannelFactory<IJobManager> JobManager()
        {
            Workload _current_workload = _target_workload;
            String joburl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobManager");           
            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
            jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return jobMgrFactory;
        }
        public IWorkloadManager WorkloadManager()
        {
            Workload _current_workload = _source_workload;
            String workloadurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/WorkloadManager");
            var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
            workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IWorkloadManager workloadMgr = workloadFactory.CreateChannel();
            return workloadMgr;
        }
        public IManagementService ManagementService(CMWorkloadType type)
        {
            Workload _current_workload = (type == CMWorkloadType.Source ? _source_workload : _target_workload);
            String url = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/Contract/ManagementService");
            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(url));
            MgtServiceFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService mgmtServiceMgr = MgtServiceFactory.CreateChannel();
            return mgmtServiceMgr;
        }
        public ChannelFactory<IJobConfigurationVerifier> ConfigurationVerifier()
        {
            Workload _current_workload = _target_workload;
            String configurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobConfigurationVerifier");
            ChannelFactory<IJobConfigurationVerifier> configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
            configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return configurationVerifierFactory;
        }

        private static String BuildConnectionUrl(Workload workload, string method)
        {
            int portNumber = 6325;
            string bindingScheme = "http://";
            return new UriBuilder(bindingScheme, Connection.find_working_ip(workload.iplist), portNumber, method).ToString();
        }
        private static NetworkCredential GetCredentials(Workload _workload)
        {
            LocalDB db = new LocalDB();

            NetworkCredential credentials = new NetworkCredential();
            Credential _credential = db.Credentials.Find(_workload.credential_id);
            credentials.UserName = Uri.EscapeDataString((String)_credential.username);
            credentials.Password = Uri.EscapeDataString((String)_credential.password);
            credentials.Domain = Uri.EscapeDataString((String)_credential.domain);
            return credentials;
        }
        public static ServiceConnectionParameters DTConnectionParams(Workload _workload)
        {
            LocalDB db = new LocalDB();

            ServiceConnectionParameters connparams = new ServiceConnectionParameters();
            Credential _credential = db.Credentials.Find(_workload.credential_id);
            connparams.UserName = Uri.EscapeDataString((String)_credential.username);
            connparams.Password = Uri.EscapeDataString((String)_credential.password);
            connparams.Domain = Uri.EscapeDataString((String)_credential.domain);
            return connparams;
        }

    }
}
