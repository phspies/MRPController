using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace CloudMoveyWorkerService.CloudMovey.Controllers
{
    class VirtualCenter
    {
        public static void vmware_getdatacenters(dynamic request)
        {
            CloudMoveyWorkerService.CloudMovey.CloudMovey CloudMovey = new CloudMoveyWorkerService.CloudMovey.CloudMovey(Global.apiBase, null, null);
            CloudMovey.task().progress(request, "Creating VMware Object", 5);

            VimClient vmware = null;
            ServiceContent sc = vmware.Connect("test");
            UserSession us = vmware.Login("usernameHere", "passwordHere");

            CloudMovey.task().progress(request, "Getting Datacenter Information", 10);
            IList<VMware.Vim.EntityViewBase> datacenters = vmware.FindEntityViews(typeof(VMware.Vim.Datacenter), null, null, null);
            foreach (VMware.Vim.EntityViewBase tmp in datacenters)
            {
                VMware.Vim.Datacenter dc = tmp as VMware.Vim.Datacenter;
                
            }
            vmware.Logout();
        }
    }
}
