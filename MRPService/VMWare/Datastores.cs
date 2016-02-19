using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Datastores : Core
    {
        public Datastores(ApiClient _virtualcenter) : base(_virtualcenter) {}

        public List<Datastore> DatastoreList(string _dc_moref)
        {
            List<Datastore> datastores = new List<Datastore>();
            ManagedObjectReference _dc = new ManagedObjectReference() { Type = "Datacenter", Value = _dc_moref };
            foreach (EntityViewBase datastore in _vmwarecontext.FindEntityViews(typeof(Datastores), _dc, null, null))
            {
                Datastore dc = datastore as Datastore;
                datastores.Add(dc);
            }
            return datastores;
        }
    }

}
