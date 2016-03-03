using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Datastores : Core
    {
        public Datastores(ApiClient _virtualcenter) : base(_virtualcenter) {}

        public List<Datastore> DatastoreList(Datacenter selectedDC = null)
        {
            ManagedObjectReference DcMoRef = new ManagedObjectReference();
            List<Datastore> datastores = new List<Datastore>();
            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }
            foreach (EntityViewBase datastore in _vmwarecontext.FindEntityViews(typeof(Datastore), DcMoRef, null, null))
            {
                Datastore dc = datastore as Datastore;
                datastores.Add(dc);
            }
            return datastores;
        }
    }

}
