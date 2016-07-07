using MRMPService.RP4VM;
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
