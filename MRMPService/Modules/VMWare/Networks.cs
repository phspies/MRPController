using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace MRMPService.VMWare
{
    public class Networks : Core
    {
        public Networks(VimApiClient _virtualcenter) : base(_virtualcenter) { }

        public VmwareDistributedVirtualSwitch GetDvSwitch(ManagedObjectReference dvportGroupSwitch)
        {
            ViewBase appSwitch = _vmwarecontext.GetView(dvportGroupSwitch, null);
            if (appSwitch != null)
            {
                VmwareDistributedVirtualSwitch thisDvSwitch = (VmwareDistributedVirtualSwitch)appSwitch;
                return thisDvSwitch;
            }
            else
            {
                return null;
            }
        }
        public List<Network> GetStandardPgs(Datacenter selectedDC = null)
        {
            List<Network> lstPortGroups = new List<Network>();
            ManagedObjectReference DcMoRef = new ManagedObjectReference();

            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }
            List<EntityViewBase> appPortGroups = _vmwarecontext.FindEntityViews(typeof(Network), DcMoRef, null, null);
            if (appPortGroups != null)
            {
                foreach (EntityViewBase appPortGroup in appPortGroups.Where(x => x.GetType() == typeof(Network)))
                {
                    lstPortGroups.Add((Network)appPortGroup);
                }
                return lstPortGroups;
            }
            else
            {
                return null;
            }
        }

        public List<DistributedVirtualSwitch> GetDVSwitches(Datacenter selectedDC = null, string dvName = null)
        {
            List<DistributedVirtualSwitch> lstDVSwitchs = new List<DistributedVirtualSwitch>();
            NameValueCollection dvFilter = new NameValueCollection();
            ManagedObjectReference DcMoRef = new ManagedObjectReference();

            if (dvName != null)
            {
                dvFilter.Add("name", dvName);
            }
            else
            {
                dvFilter = null;
            }

            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }

            List<EntityViewBase> DVs = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualSwitch), DcMoRef, dvFilter, null);
            if (DVs != null)
            {
                foreach (EntityViewBase DVSwitch in DVs)
                {
                    DistributedVirtualSwitch thisDVSwitch = (DistributedVirtualSwitch)DVSwitch;
                    lstDVSwitchs.Add(thisDVSwitch);
                }
                return lstDVSwitchs;
            }
            else
            {
                return new List<DistributedVirtualSwitch>();
            }
        }

        public List<DistributedVirtualPortgroup> GetDVPortGroups(DistributedVirtualSwitch selectedSwitch = null)
        {
            List<DistributedVirtualPortgroup> lstPortGroups = new List<DistributedVirtualPortgroup>();
            foreach (ManagedObjectReference _pg in selectedSwitch.Portgroup)
            {
                lstPortGroups.Add((DistributedVirtualPortgroup)_vmwarecontext.GetView(_pg, null));

            }
            return lstPortGroups;
        }

        public List<DistributedVirtualPortgroup> GetDVPortGroups(Datacenter selectedDC = null, string pgName = null)
        {
            List<DistributedVirtualPortgroup> lstPortGroups = new List<DistributedVirtualPortgroup>();
            NameValueCollection pgFilter = new NameValueCollection();
            ManagedObjectReference DcMoRef = new ManagedObjectReference();

            if (pgName != null)
            {
                pgFilter.Add("name", pgName);
            }
            else
            {
                pgFilter = null;
            }

            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }

            List<EntityViewBase> appPortGroups = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualPortgroup), DcMoRef, pgFilter, null);
            if (appPortGroups != null)
            {
                foreach (EntityViewBase appPortGroup in appPortGroups)
                {
                    DistributedVirtualPortgroup thisPortGroup = (DistributedVirtualPortgroup)appPortGroup;
                    lstPortGroups.Add(thisPortGroup);
                }
                return lstPortGroups;
            }
            else
            {
                return null;
            }
        }
        public List<Network> GetPortGroups(Datacenter selectedDC = null, string pgName = null)
        {
            List<Network> lstPortGroups = new List<Network>();
            NameValueCollection pgFilter = new NameValueCollection();
            ManagedObjectReference DcMoRef = new ManagedObjectReference();

            if (pgName != null)
            {
                pgFilter.Add("name", pgName);
            }
            else
            {
                pgFilter = null;
            }

            if (selectedDC != null)
            {
                DcMoRef = selectedDC.MoRef;
            }
            else
            {
                DcMoRef = null;
            }

            List<EntityViewBase> appPortGroups = _vmwarecontext.FindEntityViews(typeof(Network), DcMoRef, pgFilter, null);
            if (appPortGroups != null)
            {
                foreach (EntityViewBase appPortGroup in appPortGroups)
                {
                    Network thisPortGroup = (Network)appPortGroup;
                    lstPortGroups.Add(thisPortGroup);
                }
                return lstPortGroups;
            }
            else
            {
                return null;
            }
        }
    }
}
