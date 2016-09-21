using MRMPService.RP4VMTypes;
using System;

namespace MRMPService.RP4VMAPI
{
    public class RP4VM_ApiClient : IDisposable
    {
        public string url, username, password;
        public RP4VM_ApiClient(String _url, String _username, String _password)
        {
            url = _url;
            username = _username;
            password = _password;
        }

        public Arrays arrays()
        {
            return new Arrays(this);
        }

        public RepositoryVolumes reparrays()
        {
            return new RepositoryVolumes(this);
        }
        public vCenterServers vcenters()
        {
            return new vCenterServers(this);
        }
        public Settings settings()
        {
            return new Settings(this);
        }
        public RP4VMSystem system()
        {
            return new RP4VMSystem(this);
        }
        public Groups groups()
        {
            return new Groups(this);
        }
        public Events events()
        {
            return new Events(this);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}
