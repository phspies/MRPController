using System;


namespace MRPService.VirtualCenter
{
    public class VirtualCenter
    {
        private String _apiBase, _username, _password;
        private VirtualCenter _virtualcenter;
        public VirtualCenter(String apibase, String username, String password)
        {
            _apiBase = apibase;
            _username = username;
            _password = password;

        }


        public object datacenter()
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
