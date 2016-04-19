using VMware.Vim;

namespace MRMPService.VMWare
{
    public class Core
    {
        public VimClientImpl _vmwarecontext;
        
        public Core(VimApiClient _api_client)
        {
            _vmwarecontext = new VimClientImpl();

            ServiceContent sc = _vmwarecontext.Connect(_api_client.URL);
            UserSession us = _vmwarecontext.Login(_api_client.Username, _api_client.Password);
        }
    }
}

