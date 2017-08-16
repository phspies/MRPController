using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.VMWare;
using VMware.Vim;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;
using System.Net;

namespace MRMPService.Scheduler.PlatformInventory.VMWare
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateVMWareWorkload(string _workload_moid, MRPPlatformType _platform, List<MRMPWorkloadBaseType> _mrp_workloads = null)
        {
            MRPCredentialType _platform_credential = _platform.credential;

            //create dimension data mcp object
            VimApiClient _vim;
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);
                _vim = new VimApiClient(_platform.vmware_url, username, _platform_credential.decrypted_password);
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
                _mrp_workloads = new List<MRMPWorkloadBaseType>();

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
            MRMPWorkloadBaseType _mrp_workload = new MRMPWorkloadBaseType();
            _mrp_workload.workloadinterfaces = new List<MRPWorkloadInterfaceType>();
            _mrp_workload.workloadvolumes = new List<MRPWorkloadVolumeType>();
            _mrp_workload.workloaddisks = new List<MRPWorkloadDiskType>();
            bool _new_workload = true;
            if (_mrp_workloads.Exists(x => x.moid == _vmware_workload.MoRef.Value))
            {
                _mrp_workload = _mrp_workloads.FirstOrDefault(x => x.moid == _vmware_workload.MoRef.Value);
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
            _mrp_workload.model = "VMware, Inc. VMware Virtual Platform";
            _mrp_workload.hardwaretype = "virtual";
            //_mrp_workload.vcpu_speed = _vmware_workload.Config.Hardware.Device.ToString();


            foreach (GuestDiskInfo _volume in _vmware_workload?.Guest?.Disk)
            {
                MRPWorkloadVolumeType _workload_volume = new MRPWorkloadVolumeType();
                if (_mrp_workload.workloadvolumes.Exists(x => x.driveletter == _volume.DiskPath))
                {
                    _workload_volume = _mrp_workload.workloadvolumes.FirstOrDefault(x => x.driveletter == _volume.DiskPath);
                }
                else
                {
                    _mrp_workload.workloadvolumes.Add(_workload_volume);

                }
                _workload_volume.driveletter = _volume.DiskPath;
                _workload_volume.volumesize = (int)(_volume.Capacity / 1024 / 1024 / 1024);
                _workload_volume.volumefreespace = (int)(_volume.FreeSpace / 1024 / 1024 / 1024);
                _workload_volume.deleted = false;
            }
            //evaluate all virtual disks for workload
            List<Type> _vmware_disk_types = new List<Type>() { typeof(VirtualDisk) };
            int _index = 0;
            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => _vmware_disk_types.Contains(x.GetType())))
            {
                MRPWorkloadDiskType _logical_disk = new MRPWorkloadDiskType();

                VirtualDisk _workloaddisk = (VirtualDisk)_virtualdevice;

                if (_workloaddisk.Backing is VirtualDiskFlatVer1BackingInfo)
                {
                    VirtualDiskFlatVer1BackingInfo _disk_backing = (VirtualDiskFlatVer1BackingInfo)_workloaddisk.Backing;
                    if (_platform.platformdatastores.Any(y => y.moid == _disk_backing.Datastore.Value))
                    {
                        _logical_disk.platformstoragetier_id = _platform.platformdatastores.FirstOrDefault(y => y.moid == _disk_backing.Datastore.Value).datastore;
                    }
                }
                else if (_workloaddisk.Backing is VirtualDiskFlatVer2BackingInfo)
                {
                    VirtualDiskFlatVer2BackingInfo _disk_backing = (VirtualDiskFlatVer2BackingInfo)_workloaddisk.Backing;
                    if (_platform.platformdatastores.Any(y => y.moid == _disk_backing.Datastore.Value))
                    {
                        _logical_disk.platformstoragetier_id = _platform.platformdatastores.FirstOrDefault(y => y.moid == _disk_backing.Datastore.Value).datastore;
                    }
                }
                else if (_workloaddisk.Backing is VirtualDiskPartitionedRawDiskVer2BackingInfo)
                {
                    VirtualDiskPartitionedRawDiskVer2BackingInfo _disk_backing = (VirtualDiskPartitionedRawDiskVer2BackingInfo)_workloaddisk.Backing;
                    if (_platform.platformdatastores.Any(y => y.moid == _disk_backing.DeviceName))
                    {
                        _logical_disk.platformstoragetier_id = _platform.platformdatastores.FirstOrDefault(y => y.moid == _disk_backing.DeviceName).datastore;
                    }
                }
                else if (_workloaddisk.Backing is VirtualDiskRawDiskMappingVer1BackingInfo)
                {
                    VirtualDiskRawDiskMappingVer1BackingInfo _disk_backing = (VirtualDiskRawDiskMappingVer1BackingInfo)_workloaddisk.Backing;
                    if (_platform.platformdatastores.Any(y => y.moid == _disk_backing.Datastore.Value))
                    {
                        _logical_disk.platformstoragetier_id = _platform.platformdatastores.FirstOrDefault(y => y.moid == _disk_backing.Datastore.Value).datastore;
                    }
                }
                else
                {
                    Logger.log(String.Format("UpdateVMwareWorkload: Could not determine workload network backing type"), Logger.Severity.Error);
                }

                if (_mrp_workloads.Exists((x => x.moid == _workload_moid && x.workloaddisks.Exists(y => y.moid == _workloaddisk.Key.ToString()))))
                {
                    _logical_disk.id = _mrp_workloads.FirstOrDefault(x => x.moid == _workload_moid).workloaddisks.FirstOrDefault(y => y.moid == _workloaddisk.Key.ToString()).id;
                }
                else
                {
                    if (_mrp_workload.workloaddisks == null)
                    {
                        _mrp_workload.workloaddisks = new List<MRPWorkloadDiskType>();
                    }
                }

                _logical_disk.provisioned = true;
                _logical_disk.moid = _workloaddisk.Key.ToString();
                _logical_disk.deleted = false;
                _logical_disk.diskindex = _index;
                _logical_disk.disksize = (int)(_workloaddisk.CapacityInKB / 1024 / 1024);

                _mrp_workload.workloaddisks.Add(_logical_disk);

                _index += 1;
            }

            //evaluate all virtual network interfaces for workload
            List<Type> _vmware_nic_types = new List<Type>() { typeof(VirtualE1000), typeof(VirtualE1000e), typeof(VirtualPCNet32), typeof(VirtualSriovEthernetCard), typeof(VirtualVmxnet), typeof(VirtualVmxnet3) };
            _index = 0;
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
                if (_vmware_workload?.Guest?.Net?[_index] != null)
                {
                    IPAddress address;
                    foreach (var _ip in _vmware_workload?.Guest?.Net?[_index].IpAddress)
                    {
                        if (IPAddress.TryParse(_ip, out address))
                        {
                            switch (address.AddressFamily)
                            {
                                case System.Net.Sockets.AddressFamily.InterNetwork:
                                    if (string.IsNullOrWhiteSpace(_logical_interface.ipaddress)) _logical_interface.ipaddress = _ip;
                                    break;
                                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                                    if (string.IsNullOrWhiteSpace(_logical_interface.ipv6address)) _logical_interface.ipv6address = _ip;
                                    break;
                            }
                        }
                    }
                }


                _index += 1;
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


