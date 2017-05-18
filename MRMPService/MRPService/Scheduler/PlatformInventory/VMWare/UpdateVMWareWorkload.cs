using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.VMWare;
using VMware.Vim;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;

namespace MRMPService.Scheduler.PlatformInventory.VMWare
{
    partial class PlatformInventoryWorkloadDo
    {
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
            if (_vmware_workload.Guest.ToolsStatus == VirtualMachineToolsStatus.toolsNotRunning || _vmware_workload.Guest.ToolsStatus == VirtualMachineToolsStatus.toolsNotInstalled)
            {
                Logger.log(String.Format("Virtual Machine {0} ({1}) does not have a running tools", _vmware_workload.Config.Name, _vmware_workload.MoRef.Value), Logger.Severity.Error);
                return;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(_vmware_workload.Guest.HostName) && String.IsNullOrWhiteSpace(_vmware_workload.Guest.IpAddress))
                {
                    Logger.log(String.Format("Virtual Machine {0} ({1}) does not have a hostname and/or valid ip address", _vmware_workload.Config.Name, _vmware_workload.MoRef.Value), Logger.Severity.Error);
                    return;
                }
            }

            if (_mrp_workloads == null)
            {
                _mrp_workloads = new List<MRPWorkloadType>();

                MRPWorkloadListType _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = 1 });
                while (_paged_workload.pagination.page_size > 0)
                {
                    _mrp_workloads.AddRange(_paged_workload.workloads);
                    if (_paged_workload.pagination.next_page > 0)
                    {
                        _paged_workload = MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = _paged_workload.pagination.next_page });
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //if workload is local, updated the local db record
            //User might use these servers later...
            MRPWorkloadType _mrp_workload = new MRPWorkloadType();
            bool _new_workload = true;
            if (_mrp_workloads.Exists(x => x.moid == _vmware_workload.MoRef.Value))
            {
                _mrp_workload= _mrp_workloads.FirstOrDefault(x => x.moid == _vmware_workload.MoRef.Value);
                _new_workload = false;
            }
            else if (_mrp_workloads.Exists(x => x.iplist == _vmware_workload.Guest.IpAddress))
            {
                _mrp_workload = _mrp_workloads.FirstOrDefault(x => x.iplist == _vmware_workload.Guest.IpAddress);
                _new_workload = false;
            }

            if (_mrp_workload.vcpu == null || _mrp_workload.vcpu == 0) _mrp_workload.vcpu = _vmware_workload.Config.Hardware.NumCPU;
            if (_mrp_workload.vcore == null || _mrp_workload.vcore == 0) _mrp_workload.vcore = (int)_vmware_workload.Config.Hardware.NumCoresPerSocket;
            if (_mrp_workload.vmemory == null || _mrp_workload.vmemory == 0) _mrp_workload.vmemory = (_vmware_workload.Config.Hardware.MemoryMB / 1024);
            if (string.IsNullOrWhiteSpace(_mrp_workload.iplist)) _mrp_workload.iplist = _vmware_workload.Guest.IpAddress;
            if (String.IsNullOrWhiteSpace(_mrp_workload.hostname)) _mrp_workload.hostname = _vmware_workload.Guest.HostName;
            if (String.IsNullOrWhiteSpace(_mrp_workload.osedition)) _mrp_workload.osedition = OSEditionSimplyfier.Simplyfier(_vmware_workload.Config.GuestFullName);

            _mrp_workload.ostype = _vmware_workload.Config.GuestFullName.Contains("Win") ? "WINDOWS" : "UNIX";
            _mrp_workload.moid = _vmware_workload.MoRef.Value;
            _mrp_workload.platform_id = _platform.id;
            _mrp_workload.vcenter_uuid = _vmware_workload.Config.InstanceUuid;
            _mrp_workload.provisioned = true;
            _mrp_workload.deleted = false;


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
                    if (_platform.platformdomains.SelectMany(x => x.platformnetworks).Any(y => y.moid == _nic_backing.Network.Value))
                    {
                        _logical_interface.platformnetwork_id = _platform.platformdomains.SelectMany(x => x.platformnetworks).FirstOrDefault(y => y.moid == _nic_backing.Network.Value).id;
                    }
                }
                else if (_workloadnic.Backing is VirtualEthernetCardDistributedVirtualPortBackingInfo)
                {
                    VirtualEthernetCardDistributedVirtualPortBackingInfo _nic_backing = (VirtualEthernetCardDistributedVirtualPortBackingInfo)_workloadnic.Backing;
                    if (_platform.platformdomains.SelectMany(x => x.platformnetworks).Any(y => y.moid == _nic_backing.Port.PortgroupKey))
                    {
                        _logical_interface.platformnetwork_id = _platform.platformdomains.SelectMany(x => x.platformnetworks).FirstOrDefault(y => y.moid == _nic_backing.Port.PortgroupKey).id;
                    }
                }
                else
                {
                    Logger.log(String.Format("UpdateVMwareWorkload: Could not determine workload network backing type"), Logger.Severity.Error);
                }

                if (_mrp_workloads.Exists((x => x.moid == _workload_moid && x.workloadinterfaces.Exists(y => y.macaddress == _workloadnic.MacAddress))))
                {
                    _logical_interface = _mrp_workloads.FirstOrDefault(x => x.moid == _workload_moid).workloadinterfaces.FirstOrDefault(y => y.macaddress == _workloadnic.MacAddress);
                }
                else
                {
                    if (_mrp_workload.workloadinterfaces == null)
                    {
                        _mrp_workload.workloadinterfaces = new List<MRPWorkloadInterfaceType>();
                    }
                    _mrp_workload.workloadinterfaces.Add(_logical_interface);
                }
                //populate interface object
                _logical_interface.vnic = _index;
                _logical_interface.ipassignment = "manual_ip";
                _logical_interface.macaddress = _workloadnic.MacAddress;
                _logical_interface.deleted = false;
            }


            if (_new_workload)
            {
                _mrp_workload.credential_id = _platform.default_credential_id;
                MRMPServiceBase._mrmp_api.workload().createworkload(_mrp_workload);
            }
            else
            {
                MRMPServiceBase._mrmp_api.workload().updateworkload(_mrp_workload);
            }
        }
    }
}


