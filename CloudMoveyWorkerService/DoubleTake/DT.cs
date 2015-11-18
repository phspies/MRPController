using CloudMoveyWorkerService.CaaS1;
using CloudMoveyWorkerService.CaaS2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.DoubleTakeNS
{
    class DT
    {
        private String _apiBase, _username, _password, _datacenter, _organizationId;

        public DT(String apibase, String username, String password, String datacenter=null)
        {
            _apiBase = apibase;
            _username = username;
            _password = password;
            _datacenter = datacenter;
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

        public String OrganizationId
        {
            set { _organizationId = value; }
            get { return _organizationId; }
        }
        public String Datacenter
        {
            get { return _datacenter; }
        }
    }
}
