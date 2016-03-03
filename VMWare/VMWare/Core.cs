using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Core
    {
        public VimClientImpl _vmwarecontext;

        public Core(ApiClient _api_client)
        {
            _vmwarecontext = new VimClientImpl();

            try
            {
                ServiceContent sc = _vmwarecontext.Connect(_api_client.URL);
                UserSession us = _vmwarecontext.Login(_api_client.Username, _api_client.Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error connecting to Virtual Center Server {0} : {1}", _api_client.URL, ex.ToString()));
            }
        }
    }
}
