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
    partial class PlatformInventoryWorkloadDo : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlatformInventoryWorkloadDo()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public static void UpdateVMWareWorkload(string _workload_moid, MRPPlatformType _platform, List<MRPWorkloadType> _mrp_workloads = null)
        {

            MRPCredentialType _platform_credential = _platform.credential;

            //create dimension data mcp object
            VimApiClient _vim;
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);
                _vim = new VimApiClient(_platform.vmware_url, username, _platform_credential.encrypted_password);
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Error connecting to VMMare VCenter Server {1}", ex.Message));
            }

            VirtualMachine _vmware_workload = _vim.workload().GetWorkload(_workload_moid);
            if (String.IsNullOrWhiteSpace(_vmware_workload.Guest.HostName) && String.IsNullOrWhiteSpace(_vmware_workload.Guest.IpAddress))
            {
                Logger.log(String.Format("Virtual Machine {0} ({1}) does not have a hostname or valid ip address", _vmware_workload.Config.Name, _vmware_workload.MoRef.Value), Logger.Severity.Error);
                return;
            }

            if (_mrp_workloads == null)
            {
                _mrp_workloads = new List<MRPWorkloadType>();
                using (MRMP_ApiClient _mrmp_api = new MRMP_ApiClient())
                {
                    MRPWorkloadListType _paged_workload = _mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = 1 });
                    _mrp_workloads.AddRange(_paged_workload.workloads);
                    while (_paged_workload.pagination.page_size > 0)
                    {
                        _mrp_workloads.AddRange(_paged_workload.workloads);
                        if (_paged_workload.pagination.next_page > 0)
                        {
                            _paged_workload = _mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = _paged_workload.pagination.next_page });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            //if workload is local, updated the local db record
            //User might use these servers later...
            MRPWorkloadType _mrp_workload = new MRPWorkloadType();
            if (_mrp_workloads.Exists(x => x.moid == _vmware_workload.MoRef.Value))
            {
                _mrp_workload.id = _mrp_workloads.FirstOrDefault(x => x.moid == _vmware_workload.MoRef.Value).id;
            }
            else
            {
                _mrp_workload.vcpu = _vmware_workload.Config.Hardware.NumCPU; 
                _mrp_workload.vcore = (int)_vmware_workload.Config.Hardware.NumCoresPerSocket;
                _mrp_workload.vmemory = _vmware_workload.Config.Hardware.MemoryMB / 1024; 
                _mrp_workload.iplist = _vmware_workload.Guest.IpAddress;
                _mrp_workload.hostname = _vmware_workload.Guest.HostName;
                _mrp_workload.ostype = _vmware_workload.Config.GuestFullName.Contains("Win") ? "WINDOWS" : "UNIX"; 
                _mrp_workload.osedition = OSEditionSimplyfier.Simplyfier(_vmware_workload.Config.GuestFullName);
            }

            _mrp_workload.moid = _vmware_workload.MoRef.Value;
            _mrp_workload.platform_id = _platform.id;
            _mrp_workload.vcenter_uuid = _vmware_workload.Config.InstanceUuid;

            //evaluate all virtual network interfaces for workload
            List<Type> _vmware_nic_types = new List<Type>() { typeof(VirtualE1000), typeof(VirtualE1000e), typeof(VirtualPCNet32), typeof(VirtualSriovEthernetCard), typeof(VirtualVmxnet), typeof(VirtualVmxnet3) };
            int _index = 1;
            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => _vmware_nic_types.Contains(x.GetType())))
            {
                MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType();

                VirtualEthernetCard _workloadnic = (VirtualEthernetCard)_virtualdevice;
                //first check if the VM is connected to a vswitch

                if (_workloadnic.Backing is VirtualEthernetCardNetworkBackingInfo)
                {
                    VirtualEthernetCardNetworkBackingInfo _nic_backing = (VirtualEthernetCardNetworkBackingInfo)_workloadnic.Backing;
                    if (_platform.platformdomains_attributes.SelectMany(x => x.platformnetworks_attributes).Any(y => y.moid == _nic_backing.Network.Value))
                    {
                        _logical_interface.platformnetwork_id = _platform.platformdomains_attributes.SelectMany(x => x.platformnetworks_attributes).FirstOrDefault(y => y.moid == _nic_backing.Network.Value).id;
                    }
                }
                else if (_workloadnic.Backing is VirtualEthernetCardDistributedVirtualPortBackingInfo)
                {
                    VirtualEthernetCardDistributedVirtualPortBackingInfo _nic_backing = (VirtualEthernetCardDistributedVirtualPortBackingInfo)_workloadnic.Backing;
                    if (_platform.platformdomains_attributes.SelectMany(x => x.platformnetworks_attributes).Any(y => y.moid == _nic_backing.Port.PortgroupKey))
                    {
                        _logical_interface.platformnetwork_id = _platform.platformdomains_attributes.SelectMany(x => x.platformnetworks_attributes).FirstOrDefault(y => y.moid == _nic_backing.Port.PortgroupKey).id;
                    }
                }
                else
                {
                    Logger.log(String.Format("UpdateVMwareWorkload: Could not determine workload network backing type"), Logger.Severity.Error);
                }

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
            }

            _mrp_workload.provisioned = true;

            //Update if the portal has this workload and create if it's new to the portal....
            using (MRMP_ApiClient _cloud_movey = new MRMP_ApiClient())
            {
                if (_mrp_workloads.Exists(x => x.moid == _mrp_workload.moid))
                {
                    _cloud_movey.workload().updateworkload(_mrp_workload);
                }
                else
                {
                    _mrp_workload.credential_id = _platform.default_credential_id;
                    _cloud_movey.workload().createworkload(_mrp_workload);
                }
            }
        }
    }
}


