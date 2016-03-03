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
            NameValueCollection filter = new NameValueCollection();
            foreach (EntityViewBase datacenter in _vmwarecontext.FindEntityViews(typeof(Datacenter), null, null, null))
            {
                Datacenter dc = datacenter as Datacenter;
                datacenters.Add(dc);
            }
            return datacenters;
        }
        public Datacenter GetDataCenter(string _dc_moref)
        {
            List<EntityViewBase> appDatacenters = new List<EntityViewBase>();

            ManagedObjectReference dcobj = new ManagedObjectReference() { Type = "Datacenter", Value = _dc_moref };
            appDatacenters = _vmwarecontext.FindEntityViews(typeof(Datacenter), dcobj, null, null);

            return appDatacenters[0] as Datacenter;
        }
    }

}
