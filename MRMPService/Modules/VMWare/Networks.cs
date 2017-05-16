﻿using MRMPService.MRMPService.Log;
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
            }
            return lstPortGroups;

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
            try
            {
                List<EntityViewBase> _entitylist_switches = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualSwitch), DcMoRef, dvFilter, null);
                if (_entitylist_switches != null)
                {
                    foreach (EntityViewBase _dvswitch in _entitylist_switches)
                    {
                        DistributedVirtualSwitch _current_dvswitch = (DistributedVirtualSwitch)_dvswitch;
                        lstDVSwitchs.Add(_current_dvswitch);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error retrieving network information from {0} : {1}", selectedDC.MoRef, ex.GetBaseException().Message), Logger.Severity.Error);
            }

            return lstDVSwitchs;

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

            try
            {
                List<EntityViewBase> _entitylist_portgroupts = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualPortgroup), DcMoRef, pgFilter, null);
                if (_entitylist_portgroupts != null)
                {
                    foreach (EntityViewBase appPortGroup in _entitylist_portgroupts)
                    {
                        DistributedVirtualPortgroup thisPortGroup = (DistributedVirtualPortgroup)appPortGroup;
                        lstPortGroups.Add(thisPortGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error retrieving network information from {0} : {1}", selectedDC.MoRef, ex.GetBaseException().Message), Logger.Severity.Error);
            }
            return lstPortGroups;


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

            try
            {
                List<EntityViewBase> _entitylist_portgroups = _vmwarecontext.FindEntityViews(typeof(DistributedVirtualPortgroup), DcMoRef, pgFilter, null);
                if (_entitylist_portgroups != null)
                {
                    foreach (EntityViewBase appPortGroup in _entitylist_portgroups)
                    {
                        DistributedVirtualPortgroup thisPortGroup = (DistributedVirtualPortgroup)appPortGroup;
                        lstPortGroups.Add(thisPortGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error retrieving network information from {0} : {1}", selectedDC.MoRef, ex.GetBaseException().Message), Logger.Severity.Error);
            }
            return lstPortGroups;

        }
    }
}
