using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Datacenters : Core
    {
        public Datacenters(ApiClient _virtualcenter) : base(_virtualcenter) {}

        public List<Datacenter> DatacenterList()
        {
            List<Datacenter> datacenters = new List<Datacenter>();
            foreach (EntityViewBase datacenter in _vmwarecontext.FindEntityViews(typeof(Datacenter), null, null, null))
            {
                Datacenter dc = datacenter as Datacenter;
                datacenters.Add(dc);
            }
            return datacenters;
        }
        public Datacenter GetDataCenter(string morefMoRef)
        {
            List<EntityViewBase> appDatacenters = new List<EntityViewBase>();

            NameValueCollection dcFilter = new NameValueCollection();
            dcFilter.Add("moid", moid);
            appDatacenters = _vmwarecontext.FindEntityViews(typeof(Datacenter), null, dcFilter, null);

            return appDatacenters[0] as Datacenter;
        }
    }

}
