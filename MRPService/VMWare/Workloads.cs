using System.Collections.Generic;
using System.Collections.Specialized;
using VMware.Vim;

namespace MRPService.VMWare
{
    public class Workloads : Core
    {
        public Workloads(VimApiClient _virtualcenter) : base(_virtualcenter) {}

        public List<VirtualMachine> GetWorkloads(Datacenter selectedDC, NameValueCollection vmfilter)
        {
            List<VirtualMachine> lstVirtualMachines = new List<VirtualMachine>();
            ManagedObjectReference DcMoRef = new ManagedObjectReference();

            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }

            List<EntityViewBase> appVirtualMachines = _vmwarecontext.FindEntityViews(typeof(VirtualMachine), DcMoRef, vmFilter, null);
            if (appVirtualMachines != null)
            {
                foreach (EntityViewBase appVirtualMachine in appVirtualMachines)
                {
                    VirtualMachine thisVirtualMachine = (VirtualMachine)appVirtualMachine;
                    lstVirtualMachines.Add(thisVirtualMachine);
                }
                return lstVirtualMachines;
            }
            else
            {
                return null;
            }

  


        }
    }

}
