using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace CloudMoveyWorkerService.VirtualCenter
{
    class Core
    {
        private String _apibase, _username, _password;
        private VirtualCenter _vc;
        public VimClient _vmwarecontext;

        public Core(VirtualCenter _virtualcenter)
        {
            _apibase = _virtualcenter.ApiBase;
            _username = _virtualcenter.Username;
            _password = _virtualcenter.Password;
            _vc = _virtualcenter;
            try
            {
                ServiceContent sc = _vmwarecontext.Connect(_virtualcenter.ApiBase);
                UserSession us = _vmwarecontext.Login(_virtualcenter.Username, _virtualcenter.Password);
            }
            catch (Exception ex)
            {
                Global.event_log.WriteEntry(ex.ToString());
            }
        }
        public void connect()
        {

        }
    }
}
