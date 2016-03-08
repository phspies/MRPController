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
        public static void UpdateVMWareWorkload(string _workload_moid, string _platform_moid)
        {
            ApiClient _cloud_movey = new ApiClient();

            Platform _platform;
            Credential _platform_credential;

            using (MRPDatabase db = new MRPDatabase())
            {
                _platform = db.Platforms.FirstOrDefault(x => x.moid == _platform_moid);
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
                    platformnetwork_id = _currentplatformnetworks.platformnetworks.FirstOrDefault(x => x.moid == _workloadnic.).id
                };
                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _nic_backing.Network.Value))
                {
                    if (_currentplatformworkloads.workloads.Exists((x => x.interfaces.Exists(y => y.vnic == _workloadnic.UnitNumber))))
                    {
                        _logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.vnic == _workloadnic.UnitNumber).id;
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
                if (db.Workloads.ToList().Exists(x => x.moid == _caasworkload.id))
                {
                    _new_workload_flag = false;
                    _new_workload = db.Workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                }
                else
                {
                    //if server already exists in portal, retain GUID for the server to keep other table depedencies intact
                    if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                    {
                        _new_workload.id = _currentplatformworkloads.workloads.Find(x => x.moid == _caasworkload.id).id;
                    }
                    else
                    {
                        _new_workload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    }
                }

                _new_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                _new_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                _new_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                _new_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                _new_workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                _new_workload.hostname = _caasworkload.name;
                _new_workload.moid = _caasworkload.id;
                _new_workload.platform_id = _platform.id;
                _new_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                _new_workload.osedition = _caasworkload.operatingSystem.displayName;
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


                        //update workload source template id with portal template id
                        _moveyworkload.platformtemplate_id = _platformtemplates.platformtemplates.Find(x => x.image_moid == _caasworkload.sourceImageId).id;

                        _moveyworkload.workloaddisks_attributes = workloaddisks_parameters;
                        _moveyworkload.workloadinterfaces_attributes = workloadinterfaces_parameters;

                        _moveyworkload.provisioned = true;


                        //Update if the portal has this workload and create if it's new to the portal....
                        if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
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
