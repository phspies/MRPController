using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudMoveyNotifier.Models
{
    class CredentialTypes
    {
        public CredentialTypes()
        {
            CredentialTypeList = new List<credentials>();
            CredentialTypeList.Add(new credentials() { id = "0", credentialtype = "Platform" });
            CredentialTypeList.Add(new credentials() { id = "1", credentialtype = "Workload" });
        }
        public List<credentials> CredentialTypeList {get; set;}
    }

    class credentials
    {
        public string credentialtype { get; set; }
        public string id { get; set; }
    }
}
