using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudMoveyWorkerService.CaaS.Models;

namespace CloudMoveyWorkerService.CaaS
{
    class AccountObject : Core
    {
        public AccountObject(DimensionData _dimensiondata) : base(_dimensiondata) { }
        public Object myaccount() {
            simpleendpoint("/myaccount");
            return get<Account>(null, true);
        }
    }
}
