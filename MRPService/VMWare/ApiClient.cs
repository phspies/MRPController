using System;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class ApiClient
    {
        public VimClient _virtualcenter;
        private String _apiBase, _username, _password;

        public ApiClient(String apibase, String username, String password)
        {
            _apiBase = apibase;
            _username = username;
            _password = password;
        }
        public Datacenters datacenter()
        {
            return new Datacenters(this);
        }


        public String ApiBase
        {
            get { return _apiBase; }
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
