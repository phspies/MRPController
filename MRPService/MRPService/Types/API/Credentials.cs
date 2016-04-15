using System.Collections.Generic;

namespace MRPService.API.Types.API
{
    public class MRPCredentialsCRUDType
    {
        public string controller_id
        {
            get
            {
                return Global.manager_id;
            }
        }
        public MRPCredentialCRUDType credential { get; set; }
    }
    public class MRPCredentialCRUDType
    {
        public string id { get; set; }
        public string description { get; set; }
        public bool default_credential { get; set; }
        public int credential_type { get; set; }
        public bool standalone { get; set; }
        public string hash_value { get; set; }
        public bool deleted { get; set; }
    }
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
        public bool default_credential { get; set; }
        public int credential_type { get; set; }
        public bool enabled { get; set; }
        public string description { get; set; }
        public bool standalone { get; set; }
        public string organization_id { get; set; }
        public string hash_value { get; set; }
        public bool deleted { get; set; }
    }
}
