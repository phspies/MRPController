using System;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class ApiClient
    {
        public String _url, _username, _password;
        public VimClientImpl _vmwarecontext;

        public ApiClient(String url, String username, String password)
        {
            _url = url;
            _username = username;
            _password = password;
        }
        public Datastores datastore()
        {
            return new Datastores(this);
        }
        public Datacenters datacenter()
        {
            return new Datacenters(this);
        }
        public Networks networks()
        {
            return new Networks(this);
        }
        public Workloads workload()
        {
            return new Workloads(this);
        }
        public String URL
        {
            get { return _url; }
        }
        public String Username
        {
            get { return _username; }
        }
        public String Password
        {
            get { return _password; }
        }


    }
}
