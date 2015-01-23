using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CladesWorkerService.DimensionData.Models;
using CladesWorkerService.DimensionData.API;

namespace CladesWorkerService.DimensionData
{
    class DimensionData
    {
        private String _apiBase, _username, _password, _datacenter, _organizationId;

        public DimensionData(String apibase, String username, String password, String datacenter=null)
        {
            _apiBase = apibase;
            _username = username;
            _password = password;
            _datacenter = datacenter;
        }

        public AccountObject account() {
            return new AccountObject(this);
        }
        public DatacenterObject datacenter()
        {
            return new DatacenterObject(this);
        }
        public SoftwareObject software()
        {
            return new SoftwareObject(this);
        }
        public ServerImageObject serverimage()
        {
            return new ServerImageObject(this);
        }
        public NetworkObject network()
        {
            return new NetworkObject(this);
        }

        public ACLObject acl()
        {
            return new ACLObject(this);
        }
        public NATObject nat()
        {
            return new NATObject(this);
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
