using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{

    public class Account : Core
    {

        public Account(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public AccountState getAccountState_Method()
        {
            endpoint = "/account/state";
            mediatype = "application/json";
            return get<AccountState>();
        }

        public AccountSettings getAccountSettings_Method()
        {
            endpoint = "/account/settings";
            mediatype = "application/json";
            return get<AccountSettings>();
        }
    }
}
