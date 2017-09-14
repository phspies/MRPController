using DoubleTake.Web.Client;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.MRMPService.Log;
using System.Net;
using System.Net.Sockets;

namespace MRMPService.MRMPDoubleTake
{
    class Core
    {
        public JobsApi jobApi = null;
        public WorkloadsApi workloadApi = null;
        public string _source_address, _target_address;
        public MRPCredentialType _source_credentials, _target_credentials;
        public ManagementConnection _source_connection, _target_connection;
        public MRPDeploymentpolicyType _source_deployment_policy, _target_deployment_policy;

        public Core(Doubletake _doubletake)
        {
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            if (_doubletake._source_workload != null)
            {
                _source_credentials = _doubletake._source_workload.GetCredentials();
                _source_deployment_policy = _doubletake._source_workload.deploymentpolicy;

                //Find working IP
                _source_address = _doubletake._source_workload.GetContactibleIP(true);
                _source_credentials = _doubletake._source_workload.GetCredentials();
                //setup source connection 
                Logger.log(string.Format("Opening Source DT conneciton to {0} using {1}", _doubletake._source_workload.hostname, _source_address), Logger.Severity.Info);

                _source_connection = ManagementService.GetConnectionAsync(_source_address).Result;
                if (!_source_connection.CheckAuthorizationAsync().Result)
                {
                    _source_connection.AuthorizeAsync(_source_credentials.username, _source_credentials.decrypted_password).Wait();
                }
            }

            if (_doubletake._target_workload == null)
            {
                throw new System.ArgumentException("Target workload ID cannot be null");
            }
            _target_address = _doubletake._target_workload.GetContactibleIP(true);
            _target_credentials = _doubletake._target_workload.GetCredentials();
            _target_deployment_policy = _doubletake._target_workload.deploymentpolicy;
            Logger.log(string.Format("Opening Target DT conneciton to {0} using {1}", _doubletake._target_workload.hostname, _target_address), Logger.Severity.Info);
            _target_connection = ManagementService.GetConnectionAsync(_target_address).Result;
            if (!_target_connection.CheckAuthorizationAsync().Result)
            {
                _target_connection.AuthorizeAsync(_target_credentials.username, _target_credentials.decrypted_password).Wait();
            }


        }
    }
}
