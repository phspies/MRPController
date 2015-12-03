using CloudMoveyWorkerService.CMDoubleTake.Types;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Sqlite.Models;
using DoubleTake.Communication;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;

namespace CloudMoveyWorkerService.CMDoubleTake
{
    class CMDoubleTake_Core
    {
        private static CloudMoveyEntities dbcontext = new CloudMoveyEntities();
        public static Workload _source_workload, _target_workload;
        static CloudMoveyPortal CloudMovey = null;

        public CMDoubleTake_Core(CMDoubleTake _cmdoubletake)
        {
            _source_workload = _cmdoubletake._source_workload;
            _target_workload = _cmdoubletake._target_workload;
        }

        public static ChannelFactory<IJobManager> JobManager()
        {
            Workload _current_workload = _target_workload;
            String joburl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobManager");           
            var jobMgrFactory = new ChannelFactory<IJobManager>("DefaultBinding_IJobManager_IJobManager", new EndpointAddress(joburl));
            jobMgrFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            jobMgrFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return jobMgrFactory;
        }
        public static ChannelFactory<IWorkloadManager> WorkloadManager()
        {
            Workload _current_workload = _source_workload;
            String workloadurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/WorkloadManager");
            var workloadFactory = new ChannelFactory<IWorkloadManager>("DefaultBinding_IWorkloadManager_IWorkloadManager", new EndpointAddress(workloadurl));
            workloadFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            workloadFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return workloadFactory;
        }
        public static ChannelFactory<IManagementService> ManagementService(CMWorkloadType type)
        {
            Workload _current_workload = (type == CMWorkloadType.Source ? _source_workload : _target_workload);
            String url = BuildConnectionUrl(_current_workload, "/DoubleTake/Common/Contract/ManagementService");
            ChannelFactory<IManagementService> MgtServiceFactory = new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(url));
            MgtServiceFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return MgtServiceFactory;
        }
        public static ChannelFactory<IJobConfigurationVerifier> ConfigurationVerifier()
        {
            Workload _current_workload = _target_workload;
            String configurl = BuildConnectionUrl(_current_workload, "/DoubleTake/Jobs/JobConfigurationVerifier");
            ChannelFactory<IJobConfigurationVerifier> configurationVerifierFactory = new ChannelFactory<IJobConfigurationVerifier>("DefaultBinding_IJobConfigurationVerifier_IJobConfigurationVerifier", new EndpointAddress(configurl));
            configurationVerifierFactory.Credentials.Windows.ClientCredential = GetCredentials(_current_workload);
            configurationVerifierFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            return configurationVerifierFactory;
        }


        private static string find_working_ip(Workload _workload)
        {
            String ipaddresslist = _workload.iplist;
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
            if (_check_ip.AddressFamily.ToString() == System.Net.Sockets.AddressFamily.InterNetworkV6.ToString())
            {
                String _workingip = workingip;
                _workingip = _workingip.Replace(":", "-");
                _workingip = _workingip.Replace("%", "s");
                _workingip = _workingip + ".ipv6-literal.net";
                workingip = _workingip;
            }
            return workingip;
        }

        private static String BuildConnectionUrl(Workload workload, string method)
        {
            int portNumber = 6325;
            string bindingScheme = "http://";
            return new UriBuilder(bindingScheme, find_working_ip(workload), portNumber, method).ToString();
        }
        private static NetworkCredential GetCredentials(Workload _workload)
        {
            NetworkCredential credentials = new NetworkCredential();
            Credential _credential = dbcontext.Credentials.Find(_workload.credential_id);
            credentials.UserName = Uri.EscapeDataString((String)_credential.username);
            credentials.Password = Uri.EscapeDataString((String)_credential.password);
            credentials.Domain = Uri.EscapeDataString((String)_credential.domain);
            return credentials;
        }
        public static ServiceConnectionParameters DTConnectionParams(Workload _workload)
        {
            ServiceConnectionParameters connparams = new ServiceConnectionParameters();
            Credential _credential = dbcontext.Credentials.Find(_workload.credential_id);
            connparams.UserName = Uri.EscapeDataString((String)_credential.username);
            connparams.Password = Uri.EscapeDataString((String)_credential.password);
            connparams.Domain = Uri.EscapeDataString((String)_credential.domain);
            return connparams;
        }

    }
}
