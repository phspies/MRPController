using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMware.Vim;

namespace MRPService.VMWare
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
                return null;
            }
        }
        public List<DistributedVirtualPortgroup> GetDVPortGroups(DistributedVirtualSwitch selectedSwitch = null, string switchName = null)
        {
            List<DistributedVirtualPortgroup> lstPortGroups = new List<DistributedVirtualPortgroup>();
            NameValueCollection pgFilter = new NameValueCollection();
            ManagedObjectReference DvcMoRef = new ManagedObjectReference();

            if (switchName != null)
            {
                pgFilter.Add("name", switchName);
            }
            else
            {
                pgFilter = null;
            }

            if (selectedSwitch != null)
            {
               DvcMoRef = selectedSwitch.MoRef;
            }
            else
            {
                DvcMoRef = null;
            }

            List<EntityViewBase> appPortGroups = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualPortgroup), DvcMoRef, pgFilter, null);
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
