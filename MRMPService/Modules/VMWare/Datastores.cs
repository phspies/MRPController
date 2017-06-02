using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRMPService.VMWare
{
    public class Datastores : Core
    {
        public Datastores(VimApiClient _virtualcenter) : base(_virtualcenter) { }

        public List<Datastore> DatastoreList(Datacenter selectedDC = null)
        {
            ManagedObjectReference DcMoRef = new ManagedObjectReference();
            List<Datastore> lstDatastores = new List<Datastore>();
            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }
            List<EntityViewBase> _entitylist_datastores = _vmwarecontext.FindEntityViews(typeof(Datastore), DcMoRef, null, null);

            try
            {
                if (_entitylist_datastores != null)
                {
                    foreach (EntityViewBase datastore in _entitylist_datastores)
                    {
                        Datastore dc = datastore as Datastore;
                        lstDatastores.Add(dc);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error retrieving datastores from {0} : {1}", selectedDC.MoRef, ex.GetBaseException().Message), Logger.Severity.Error);
            }
            return lstDatastores;
        }
    }

}
