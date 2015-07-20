using CloudMoveyWorkerService.CaaS1;
using CloudMoveyWorkerService.CaaS2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CaaS
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
        public ServerObject server()
        {
            return new ServerObject(this);
        }
        public SoftwareObject software()
        {
            return new SoftwareObject(this);
        }
        public ServerImageObject serverimage()
        {
            return new ServerImageObject(this);
        }
        public MCP2VLANObject mcp2vlans()
        {
            return new MCP2VLANObject(this);
        }
        public MCP2ServerObject mcp2servers()
        {
            return new MCP2ServerObject(this);
        }
        public MCP2NetworkDomainObject mcp2networkdomain()
        {
            return new MCP2NetworkDomainObject(this);
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
