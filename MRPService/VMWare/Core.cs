using MRPService.MRPService.Log;
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
        public global::VMware.Vim.VimClient _vmwarecontext;

        public Core(ApiClient _virtualcenter)
        {
            ServiceContent vimServiceContent = new ServiceContent();
            UserSession vimSession = new UserSession();

            try
            {
                _vmwarecontext.Connect("https://" + _virtualcenter.ApiBase.Trim() + "/sdk");
                vimSession = _vmwarecontext.Login(_virtualcenter.Username, _virtualcenter.Password);
                vimServiceContent = _vmwarecontext.ServiceContent;
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error connecting to Virtual Center Server {0} : {1}", _virtualcenter.ApiBase, ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
