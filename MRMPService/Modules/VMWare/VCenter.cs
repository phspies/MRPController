using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRMPService.VMWare
{
    public class vCenter : Core
    {
        public vCenter(VimApiClient _virtualcenter) : base(_virtualcenter) { }

        public AboutInfo GetvCenterAbout()
        {
            return _service_content.About;
        }
    }
}
