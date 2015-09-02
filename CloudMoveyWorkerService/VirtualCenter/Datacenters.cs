using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace CloudMoveyWorkerService.VirtualCenter
{
    class Datacenters : Core
    {
        public Datacenters(VirtualCenter _virtualcenter) : base(_virtualcenter) {}
        public bool DatacenterList()
        {


            IList<EntityViewBase> datacenters = _vmwarecontext.FindEntityViews(typeof(Datacenter), null, null, null);
            foreach (EntityViewBase tmp in datacenters)
            {
                Datacenter dc = tmp as Datacenter;
            }
            _vmwarecontext.Logout();
            return true;
        }
    }

}
