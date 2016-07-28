using MRMPService.Utiliies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRMPService.MRMPAPI.Types.API
{
    public class MRPCredentialListType
    {
        [JsonProperty("credentials")]
        public List<MRPCredentialType> credentials { get; set; }
    }

    public class MRPCredentialType
    {
        [JsonProperty("_encrypted_password")]
        private string _encrypted_password {get; set;}
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("credential_type")]
        public string credential_type { get;  set;}
        [JsonProperty("domain")]
        public string domain { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("encrypted_password")]
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
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("default_credential")]
        public bool default_credential {get; set;}
    }
}
