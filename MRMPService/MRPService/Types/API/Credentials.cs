using System.Collections.Generic;

namespace MRMPService.API.Types.API
{
    public class MRPCredentialListType
    {
        public List<MRPCredentialType> credentials { get; set; }
    }

    public class MRPCredentialType
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public string credential_type { get; set; }
        public bool enabled { get; set; }
        public string description { get; set; }
        public string organization_id { get; set; }
        public bool default_credential {get; set;} 
    }
}
