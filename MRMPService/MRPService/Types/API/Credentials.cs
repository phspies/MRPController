using MRMPService.Utiliies;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPCredentialListType
    {
        public List<MRPCredentialType> credentials { get; set; }
    }

    public class MRPCredentialType
    {
        private string _encrypted_password {get; set;}

        public string id { get; set; }
        public string username { get; set; }
        public string credential_type { get;  set;}
        public string domain { get; set; }
        public string password { get; set; }
        public string encrypted_password
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(_encrypted_password))
                {
                    CryptLib _crypt = new CryptLib();
                    byte[] data = Convert.FromBase64String(_encrypted_password);
                    string decodedString = _crypt.decrypt(data, Global.organization_id, password);
                    return decodedString;
                }
                else
                {
                    return password;
                }
            }
            set { _encrypted_password = value; }
        }
        public bool enabled { get; set; }
        public string description { get; set; }
        public string organization_id { get; set; }
        public bool default_credential {get; set;} 
    }
}
