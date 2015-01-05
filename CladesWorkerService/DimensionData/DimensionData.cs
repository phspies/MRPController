using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubleTakeProxyService.DimensionData.API;

namespace DoubleTakeProxyService.DimensionData
{
    class DimensionData
    {
        public Guid OrganizationId;
        public String ApiBase, Username, Password, Datacenter;

        public DimensionData(String _apibase, String _username, String _password, String datacenter)
        {

        }

        Account account() {
            return new Account { _dimensiondata = this };
        }
   
    }
}
