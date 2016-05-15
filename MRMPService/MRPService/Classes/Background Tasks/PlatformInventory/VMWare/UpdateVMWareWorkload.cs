﻿using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.API;
using MRMPService.VMWare;
using VMware.Vim;

namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateVMWareWorkload(string _workload_moid, string _platform_id)
        {
            MRP_ApiClient _cloud_movey = new MRP_ApiClient();

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
            List<MRPWorkloadType> _mrp_workloads = _cloud_movey.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();
            List<MRPPlatformdomainType> _mrp_domains = _cloud_movey.platformdomain().listplatformdomains().platformdomains.Where(x => x.platform_id == _platform_id).ToList();
            List<MRPPlatformnetworkType> _mrp_networks = _cloud_movey.platformnetwork().listplatformnetworks().platformnetworks.Where(x => _mrp_domains.Exists(y => y.id == x.platformdomain_id)).ToList();

            //Pupulate workload total storage allocated
            long storage_allocated = 0;
            foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => x.GetType() == typeof(VirtualDisk)))
            {
                VirtualDisk _workloaddisk = (VirtualDisk)_virtualdevice;
                storage_allocated += _workloaddisk.CapacityInKB / 1024 / 1024;
            }

            //if workload is local, updated the local db record
            //User might use these servers later...
            bool _new_workload_flag = true;
            using (MRPDatabase db = new MRPDatabase())
            {
                Workload _db_workload = new Workload();
                if (db.Workloads.ToList().Exists(x => x.moid == _workload_moid && x.platform_id == _platform.id))
                {
                    _new_workload_flag = false;
                    _db_workload = db.Workloads.FirstOrDefault(x => x.moid == _workload_moid && x.platform_id == _platform.id);
                }
                else
                {
                    //if server already exists in portal, retain GUID for the server to keep other table depedencies intact
                    if (_mrp_workloads.Exists(x => x.moid == _workload_moid && x.platform_id == _platform.id))
                    {
                        _db_workload.id = _mrp_workloads.Find(x => x.moid == _workload_moid && x.platform_id == _platform.id).id;
                    }
                    else
                    {
                        _db_workload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    }
                }

                _db_workload.vcpu = _vmware_workload.Config.Hardware.NumCPU;
                _db_workload.vcore = _vmware_workload.Config.Hardware.NumCoresPerSocket;
                _db_workload.vmemory = _vmware_workload.Config.Hardware.MemoryMB / 1024;
                _db_workload.iplist = _vmware_workload.Guest.IpAddress;
                _db_workload.storage_count = storage_allocated;
                _db_workload.hostname = _vmware_workload.Guest.HostName;
                _db_workload.moid = _vmware_workload.MoRef.Value;
                _db_workload.platform_id = _platform.id;
                _db_workload.ostype = _vmware_workload.Config.GuestFullName.Contains("Win") ? "WINDOWS" : "OTHER";
                _db_workload.osedition = OSEditionSimplyfier.Simplyfier(_vmware_workload.Config.GuestFullName);
                if (_new_workload_flag)
                {

                }
                else
                {
                    db.SaveChanges();
                    if (_db_workload.enabled == true)
                    {
                        //copy db object into mrp portal object
                        Objects.Copy(_db_workload, _mrp_workload);

                        //evaluate all virtual network interfaces for workload
                        List<Type> _vmware_nic_types = new List<Type>() { typeof(VirtualE1000), typeof(VirtualE1000e), typeof(VirtualPCNet32), typeof(VirtualSriovEthernetCard), typeof(VirtualVmxnet) };
                        int _index = 1;
                        foreach (VirtualDevice _virtualdevice in _vmware_workload.Config.Hardware.Device.Where(x => _vmware_nic_types.Contains(x.GetType())))
                        {
                            VirtualEthernetCard _workloadnic = (VirtualEthernetCard)_virtualdevice;
                            VirtualEthernetCardNetworkBackingInfo _nic_backing = (VirtualEthernetCardNetworkBackingInfo)_workloadnic.Backing;
                            MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType()
                            {
                                vnic = _index,
                                ipassignment = "manual_ip",
                                macaddress = _workloadnic.MacAddress,
                                _destroy = false,
                                platformnetwork_id = _mrp_networks.FirstOrDefault(x => x.moid == _nic_backing.Network.Value).id
                            };
                            if (_mrp_workloads.Exists((x => x.moid == _workload_moid && x.workloadinterfaces_attributes.Exists(y => y.macaddress == _workloadnic.MacAddress))))
                            {
                                _logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _workload_moid).workloadinterfaces_attributes.FirstOrDefault(y => y.macaddress == _workloadnic.MacAddress).id;
                            }
                            _mrp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                        }

                        _mrp_workload.provisioned = true;

                        //Update if the portal has this workload and create if it's new to the portal....
                        if (_mrp_workloads.Exists(x => x.moid == _workload_moid))
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

        }

    }
}
