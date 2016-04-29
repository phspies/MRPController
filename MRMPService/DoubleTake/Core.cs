using DoubleTake.Web.Client;
using MRMPService.LocalDatabase;
using MRMPService.Utilities;
using System;

namespace MRMPService.DoubleTake
{
    class Core
    {
        public JobsApi jobApi = null;
        public WorkloadsApi workloadApi = null;
        public static LocalDatabase.Workload _source_workload, _target_workload;
        public string _source_address, _target_address;
        public Credential _source_credentials, _target_credentials;
        public ManagementConnection _source_connection, _target_connection;

        public Core(Doubletake _doubletake)
        {
            if (_doubletake._target_workload_id == null)
            {
                throw new System.ArgumentException("Target workload ID cannot be null");
            }
            using (WorkloadSet _db_workload = new WorkloadSet())
            {
                _target_workload = _db_workload.ModelRepository.GetById(_doubletake._target_workload_id);
            }
            if (_target_workload == null)
            {
                throw new System.ArgumentException("Cannot find target workload in database");
            }
            //Find working IP
            using (Connection _connection = new Connection())
            {
                _target_address = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (_target_address == null)
            {
                throw new System.ArgumentException(string.Format("Cannot contact workload {0}", _target_workload.hostname));
            }

            using (CredentialSet _db_credential = new CredentialSet())
            {
                _target_credentials = _db_credential.ModelRepository.GetById(_target_workload.credential_id);
                _target_connection = ManagementService.GetConnectionAsync(_target_address).Result;
                AccessLevelApi api = new AccessLevelApi(_target_connection);
                if (!_target_connection.CheckAuthorizationAsync().Result)
                {
                    _target_connection.AuthorizeAsync(_target_credentials.username, _target_credentials.password).Wait();
                }
            }

            //source could be empty in certian instances
            if (_doubletake._source_workload_id != null)
            {
                using (WorkloadSet _db_workload = new WorkloadSet())
                {
                    _source_workload = _db_workload.ModelRepository.GetById(_doubletake._source_workload_id);
                }
                if (_source_workload == null)
                {
                    throw new System.ArgumentException("Cannot find source workload in database");
                }

                //Find working IP
                using (Connection _connection = new Connection())
                {
                    _source_address = _connection.FindConnection(_source_workload.iplist, true);
                }
                if (_source_address == null)
                {
                    throw new System.ArgumentException(string.Format("Cannot contact workload {0}", _source_workload.hostname));
                }

                using (CredentialSet _db_credential = new CredentialSet())
                {
                    _source_credentials = _db_credential.ModelRepository.GetById(_source_workload.credential_id);
                    _source_connection = ManagementService.GetConnectionAsync(_source_address).Result;
                    AccessLevelApi api = new AccessLevelApi(_source_connection);
                    if (!_source_connection.CheckAuthorizationAsync().Result)
                    {
                        _source_connection.AuthorizeAsync(_source_credentials.username, _source_credentials.password).Wait();
                    }
                }
            }
        }
    }
}
