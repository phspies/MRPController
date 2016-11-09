using DoubleTake.Web.Client;
using MRMPService.MRMPAPI.Contracts;
using MRMPService.MRMPService.Types.API;
using MRMPService.Utilities;

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
            

            if (_doubletake._target_workload == null)
            {
                throw new System.ArgumentException("Target workload ID cannot be null");
            }
            //Find working IP
            using (Connection _connection = new Connection())
            {
                _target_address = _connection.FindConnection(_doubletake._target_workload.iplist, true);
            }
            if (_target_address == null)
            {
                throw new System.ArgumentException(string.Format("Cannot contact workload {0}", _doubletake._target_workload.hostname));
            }
            _target_credentials = _doubletake._target_workload.credential;
            _target_deployment_policy = _doubletake._target_workload.deploymentpolicy;

            //setup target connection 
            _target_connection = ManagementService.GetConnectionAsync(_target_address).Result;
            if (!_target_connection.CheckAuthorizationAsync().Result)
            {
                _target_connection.AuthorizeAsync(_target_credentials.username, _target_credentials.encrypted_password).Wait();
               
            }

            //source could be empty in certian instances
            if (_doubletake._source_workload != null)
            {
                _source_credentials = _doubletake._source_workload.credential;
                _source_deployment_policy = _doubletake._source_workload.deploymentpolicy;

                //Find working IP
                using (Connection _connection = new Connection())
                {
                    _source_address = _connection.FindConnection(_doubletake._source_workload.iplist, true);
                }
                if (_source_address == null)
                {
                    throw new System.ArgumentException(string.Format("Cannot contact workload {0}", _doubletake._source_workload.hostname));
                }

                _source_credentials = _doubletake._source_workload.credential;
                //setup source connection 
                _source_connection = ManagementService.GetConnectionAsync(_source_address).Result;
                if (!_source_connection.CheckAuthorizationAsync().Result)
                {
                    _source_connection.AuthorizeAsync(_source_credentials.username, _source_credentials.encrypted_password).Wait();
                }
            }
        }
    }
}
