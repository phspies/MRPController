using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRPService.Utilities;
using MRPService.API;
using MRPService.VMWare;
using VMware.Vim;

namespace MRPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateVMWareWorkload(string _workload_moid, string _platform_id)
        {
            ApiClient _cloud_movey = new ApiClient();

            Platform _platform;
            Credential _platform_credential;

            using (MRPDatabase db = new MRPDatabase())
            {
                _platform = db.Platforms.FirstOrDefault(x => x.id == _platform_id);
                _platform_credential = db.Credentials.FirstOrDefault(x => x.id == _platform.credential_id);

            }

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

            //Retrieve portal objects
            MRPWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
            MRPPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
            MRPPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();
            MRPPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();



            //Pupulate logical volumes for workload
            List<MRPWorkloadDiskType> workloaddisks_parameters = new List<MRPWorkloadDiskType>();

            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => x.GetType() == typeof(VirtualDisk)))
            {
                VirtualDisk _workloaddisk = (VirtualDisk)_virtualdevice;

                MRPWorkloadDiskType _virtual_disk = new MRPWorkloadDiskType()
                {
                    diskindex = (int)_workloaddisk.UnitNumber,
                    provisioned = true,
                    disksize = _workloaddisk.CapacityInKB / 1024 / 1024,
                    _destroy = false
                };
                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _vmware_workload.MoRef.Value && x.disks.Exists(y => y.diskindex == _workloaddisk.UnitNumber)))
                {
                    _virtual_disk.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _vmware_workload.MoRef.Value).disks.FirstOrDefault(y => y.diskindex == _workloaddisk.UnitNumber).id;
                }
                workloaddisks_parameters.Add(_virtual_disk);
            }

            //populate network interfaces for workload
            List<MRPWorkloadInterfaceType> workloadinterfaces_parameters = new List<MRPWorkloadInterfaceType>();
            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => x.GetType() == typeof(VirtualDisk)))
            {
                VirtualEthernetCard _workloadnic = (VirtualEthernetCard)_virtualdevice;
                VirtualEthernetCardNetworkBackingInfo _nic_backing = (VirtualEthernetCardNetworkBackingInfo)_workloadnic.Backing;
                MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType()
                {
                    vnic = (int)_workloadnic.UnitNumber,
                    ipassignment = "manual_ip",
                    _destroy = false,
                    platformnetwork_id = _currentplatformnetworks.platformnetworks.FirstOrDefault(x => x.moid == _workloadnic.UnitNumber.ToString()).id
                };
                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _nic_backing.Network.Value))
                {
                    if (_currentplatformworkloads.workloads.Exists((x => x.interfaces.Exists(y => y.vnic == _workloadnic.UnitNumber))))
                    {
                        _logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.platform_id == _platform_id).interfaces.FirstOrDefault(y => y.vnic == _workloadnic.UnitNumber).id;
                    }
                }
                workloadinterfaces_parameters.Add(_logical_interface);
            }

            //if workload is local, updated the local db record
            //User might use these servers later...
            bool _new_workload_flag = true;
            using (MRPDatabase db = new MRPDatabase())
            {
                Workload _new_workload = new Workload();
                if (db.Workloads.ToList().Exists(x => x.moid == _workload_moid && x.platform_id == _platform.id))
                {
                    _new_workload_flag = false;
                    _new_workload = db.Workloads.FirstOrDefault(x => x.moid == _workload_moid && x.platform_id == _platform.id);
                }
                else
                {
                    //if server already exists in portal, retain GUID for the server to keep other table depedencies intact
                    if (_currentplatformworkloads.workloads.Exists(x => x.moid == _workload_moid && x.platform_id == _platform.id))
                    {
                        _new_workload.id = _currentplatformworkloads.workloads.Find(x => x.moid == _workload_moid && x.platform_id == _platform.id).id;
                    }
                    else
                    {
                        _new_workload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    }
                }

                _new_workload.vcpu = _vmware_workload.Config.Hardware.NumCPU;
                _new_workload.vcore = _vmware_workload.Config.Hardware.NumCoresPerSocket;
                _new_workload.vmemory = _vmware_workload.Config.Hardware.MemoryMB/1024;
                _new_workload.iplist = _vmware_workload.Guest.IpAddress;
                _new_workload.storage_count = _vmware_workload.Storage.PerDatastoreUsage.Sum(x => x.Committed)/1024/1024/1024;
                _new_workload.hostname = _vmware_workload.Guest.HostName;
                _new_workload.moid = _vmware_workload.MoRef.Value;
                _new_workload.platform_id = _platform.id;
                _new_workload.ostype = _vmware_workload.Config.GuestFullName.Contains("Win") ? "WINDOWS" : "OTHER";
                _new_workload.osedition = _vmware_workload.Config.AlternateGuestName;
                if (_new_workload_flag)
                {
                    _new_workload.id = Objects.RamdomGuid();
                    db.Workloads.Add(_new_workload);
                    db.SaveChanges();
                }
                else
                {
                    db.SaveChanges();
                    if (_new_workload.enabled == true)
                    {
                        MRPWorkloadCRUDType _moveyworkload = new MRPWorkloadCRUDType();
                        Objects.Copy(_new_workload, _moveyworkload);

                        _moveyworkload.workloaddisks_attributes = workloaddisks_parameters;
                        _moveyworkload.workloadinterfaces_attributes = workloadinterfaces_parameters;

                        _moveyworkload.provisioned = true;


                        //Update if the portal has this workload and create if it's new to the portal....
                        if (_currentplatformworkloads.workloads.Exists(x => x.moid == _workload_moid && x.platform_id == _platform_id))
                        {
                            _moveyworkload.id = _new_workload.id;
                            _cloud_movey.workload().updateworkload(_moveyworkload);
                        }
                        else
                        {
                            _cloud_movey.workload().createworkload(_moveyworkload);
                        }
                    }
                }
            }

        }

    }
}
