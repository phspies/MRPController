using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CladesWorkerService.DimensionData.Models;

namespace CladesWorkerService.DimensionData.API
{
    class AccountObject : Core
    {
        public AccountObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public Account myaccount() {
            endpoint = "/myaccount";
            Account account = get<Account>(null, true) as Account;
            return account;
        }
    }
}
