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

namespace MRPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateMCPWorkload(string _workload_moid, string _platform_moid)
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
            ServerType _caasworkload;
            try {
                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credential.username, _platform_credential.password));
                CaaS.Login().Wait();
                _caasworkload = CaaS.ServerManagement.Server.GetServer(Guid.Parse(_workload_moid)).Result;

            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Error connecting to Dimension Data MCP {1}", ex.Message));
            }

            //Retrieve portal objects
            MRPWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
            MRPPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
            MRPPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();
            MRPPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();



            //Pupulate logical volumes for workload
            List<MRPWorkloadDiskType> workloaddisks_parameters = new List<MRPWorkloadDiskType>();
            foreach (ServerTypeDisk _workloaddisk in _caasworkload.disk)
            {
                MRPWorkloadDiskType _virtual_disk = new MRPWorkloadDiskType()
                {
                    moid = _workloaddisk.id,
                    diskindex = _workloaddisk.scsiId,
                    provisioned = true,
                    disksize = _workloaddisk.sizeGb,
                    _destroy = false
                };
                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id && x.disks.Exists(y => y.moid == _workloaddisk.id)))
                {
                    _virtual_disk.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).disks.FirstOrDefault(y => y.moid == _workloaddisk.id).id;
                }
                workloaddisks_parameters.Add(_virtual_disk);
            }

            //populate network interfaces for workload
            List<MRPWorkloadInterfaceType> workloadinterfaces_parameters = new List<MRPWorkloadInterfaceType>();
            MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
            if (_currentplatformworkloads.workloads.Exists(x => x.interfaces.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
            {
                _primary_logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
            }
            workloadinterfaces_parameters.Add(_primary_logical_interface);
            int nic_index = 1;
            if (_caasworkload.networkInfo.additionalNic != null)
            {
                foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                {
                    MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType()
                    {
                        vnic = nic_index,
                        ipassignment = "manual_ip",
                        ipv6address = _caasworkloadinterface.ipv6,
                        ipaddress = _caasworkloadinterface.privateIpv4,
                        moid = _caasworkloadinterface.id,
                        _destroy = false,
                        platformnetwork_id = _currentplatformnetworks.platformnetworks.FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id
                    };
                    if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                    {
                        if (_currentplatformworkloads.workloads.Exists((x => x.interfaces.Exists(y => y.moid == _caasworkloadinterface.id))))
                        {
                            _logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkloadinterface.id).id;
                        }
                    }
                    workloadinterfaces_parameters.Add(_logical_interface);
                    nic_index += 1;
                }
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
