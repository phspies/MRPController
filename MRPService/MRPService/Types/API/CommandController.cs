using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.API.Types.API
{
    class MRPCommandControllerType
    { 
        public string controller_id
        {
            get
            {
                return Global.agent_id;
            }
        }
    }
}
