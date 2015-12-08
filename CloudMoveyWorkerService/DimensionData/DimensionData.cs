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
        public WorkloadObject workload()
        {
            return new WorkloadObject(this);
        }
        public VLANObject vlans()
        {
            return new VLANObject(this);
        }
        public WorkloadObject workloads()
        {
            return new WorkloadObject(this);
        }
        public TemplateObject templates()
        {
            return new TemplateObject(this);
        }
        public NetworkDomainObject networkdomain()
        {
            return new NetworkDomainObject(this);
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
