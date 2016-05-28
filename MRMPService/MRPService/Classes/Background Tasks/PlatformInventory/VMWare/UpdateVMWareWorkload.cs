using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.MRMPAPI;
using MRMPService.VMWare;
using VMware.Vim;
using MRMPService.MRMPService.Log;

namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateVMWareWorkload(string _workload_moid, MRPPlatformType _platform, List<MRPWorkloadType> _mrp_workloads = null, List<MRPPlatformdomainType> _mrp_domains = null, List<MRPPlatformnetworkType> _mrp_networks = null)
        {
            MRMP_ApiClient _cloud_movey = new MRMP_ApiClient();

            MRPCredentialType _platform_credential = _platform.credential;

            //create dimension data mcp object
            VimApiClient _vim;
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);
                _vim = new VimApiClient(_platform.url, username, _platform_credential.password);
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Error connecting to VMMare VCenter Server {1}", ex.Message));
            }

            VirtualMachine _vmware_workload = _vim.workload().GetWorkload(_workload_moid);

            if (_mrp_workloads == null)
            {
                Logger.log(String.Format("UpdateVMwareWorkload: Workload list empty, fecthing new list"), Logger.Severity.Info);
                _mrp_workloads = _cloud_movey.workload().list_by_platform_all(_platform).workloads.ToList();
            }
            if (_mrp_domains == null)
            {
                Logger.log(String.Format("UpdateVMwareWorkload: Network Domain list empty, fecthing new list"), Logger.Severity.Info);
                _mrp_domains = _cloud_movey.platformdomain().list_by_platform(_platform).platformdomains.ToList();
            }
            if (_mrp_networks == null)
            {
                Logger.log(String.Format("UpdateVMwareWorkload: Network VLAN list empty, fecthing new list"), Logger.Severity.Info);
                _mrp_networks = _cloud_movey.platformnetwork().list_by_platform(_platform).platformnetworks.ToList();
            }

            //if workload is local, updated the local db record
            //User might use these servers later...
            MRPWorkloadType _mrp_workload = new MRPWorkloadType();
            if (_mrp_workloads.Exists(x => x.moid == _vmware_workload.MoRef.Value))
            {
                _mrp_workload = _mrp_workloads.FirstOrDefault(x => x.moid == _vmware_workload.MoRef.Value);
            }

            _mrp_workload.vcpu = _vmware_workload.Config.Hardware.NumCPU;
            _mrp_workload.vcore = (int)_vmware_workload.Config.Hardware.NumCoresPerSocket;
            _mrp_workload.vmemory = _vmware_workload.Config.Hardware.MemoryMB / 1024;
            _mrp_workload.iplist = _vmware_workload.Guest.IpAddress;
            _mrp_workload.hostname = _vmware_workload.Guest.HostName;
            _mrp_workload.moid = _vmware_workload.MoRef.Value;
            _mrp_workload.platform_id = _platform.id;
            _mrp_workload.ostype = _vmware_workload.Config.GuestFullName.Contains("Win") ? "WINDOWS" : "UNIX";
            _mrp_workload.osedition = OSEditionSimplyfier.Simplyfier(_vmware_workload.Config.GuestFullName);

            //evaluate all virtual network interfaces for workload
            List<Type> _vmware_nic_types = new List<Type>() { typeof(VirtualE1000), typeof(VirtualE1000e), typeof(VirtualPCNet32), typeof(VirtualSriovEthernetCard), typeof(VirtualVmxnet) };
            int _index = 1;
            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => _vmware_nic_types.Contains(x.GetType())))
            {
                VirtualEthernetCard _workloadnic = (VirtualEthernetCard)_virtualdevice;
                VirtualEthernetCardNetworkBackingInfo _nic_backing = (VirtualEthernetCardNetworkBackingInfo)_workloadnic.Backing;

                MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType();
                if (_mrp_workloads.Exists((x => x.moid == _workload_moid && x.workloadinterfaces_attributes.Exists(y => y.macaddress == _workloadnic.MacAddress))))
                {
                    _logical_interface = _mrp_workloads.FirstOrDefault(x => x.moid == _workload_moid).workloadinterfaces_attributes.FirstOrDefault(y => y.macaddress == _workloadnic.MacAddress);
                }
                else
                {
                    if (_mrp_workload.workloadinterfaces_attributes == null)
                    {
                        _mrp_workload.workloadinterfaces_attributes = new List<MRPWorkloadInterfaceType>();
                    }
                    _mrp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                }
                //populate interface object
                _logical_interface.vnic = _index;
                _logical_interface.ipassignment = "manual_ip";
                _logical_interface.macaddress = _workloadnic.MacAddress;
                _logical_interface._destroy = false;
                _logical_interface.platformnetwork_id = _mrp_networks.FirstOrDefault(x => x.moid == _nic_backing.Network.Value).id;
    
            }

            _mrp_workload.provisioned = true;

            //Update if the portal has this workload and create if it's new to the portal....
            if (_mrp_workloads.Exists(x => x.moid == _mrp_workload.moid))
            {
                _cloud_movey.workload().updateworkload(_mrp_workload);
            }
            else
            {
                _cloud_movey.workload().createworkload(_mrp_workload);
            }
        }
    }
}


