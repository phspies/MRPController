using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyWorkerService.CloudMovey.Types.API
{
    public class MoveyCredentialsCRUDType
    {
        public string worker_id { get; set; }
        public string worker_hostname { get; set; }
        public MoveyCredentialCRUDType credential { get; set; }
    }
    public class MoveyCredentialCRUDType
    {
        public string id { get; set; }
        public string description { get; set; }
        public bool default_credential { get; set; }
        public byte? credential_type { get; set; }
        public bool standalone { get; set; }
        public string hash_value { get; set; }

    }
    public class MoveyCredentialListType
    {
        public List<MoveyCredentialType> credentials { get; set; }
    }

    public class MoveyCredentialType
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public bool default_credential { get; set; }
        public byte? credential_type { get; set; }
        public bool enabled { get; set; }
        public string description { get; set; }
        public bool standalone { get; set; }
        public string organization_id { get; set; }
        public string hash_value { get; set; }

    }

}
