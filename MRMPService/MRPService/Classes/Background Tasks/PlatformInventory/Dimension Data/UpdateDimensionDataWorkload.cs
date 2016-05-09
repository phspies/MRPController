using MRMPService.LocalDatabase;
using MRMPService.API.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.Utilities;
using MRMPService.API;
using MRMPService.MRMPService.Log;

namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public void UpdateMCPWorkload(string _workload_moid, string _platform_id, List<MRPWorkloadType> _mrp_workloads = null, List<MRPPlatformdomainType> _mrp_domains = null, List<MRPPlatformnetworkType> _mrp_networks = null, MRPPlatformtemplateListType _mrp_templates = null)
        {
            Platform _platform;
            Credential _platform_credential;
            using (MRP_ApiClient _cloud_movey = new MRP_ApiClient())
            {
                using (MRPDatabase db = new MRPDatabase())
                {
                    _platform = db.Platforms.FirstOrDefault(x => x.id == _platform_id);
                    _platform_credential = db.Credentials.FirstOrDefault(x => x.id == _platform.credential_id);
                }

                //create dimension data mcp object
                ServerType _caasworkload;
                try
                {
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credential.username, _platform_credential.password));
                    CaaS.Login().Wait();
                    _caasworkload = CaaS.ServerManagement.Server.GetServer(Guid.Parse(_workload_moid)).Result;
                }
                catch (Exception ex)
                {
                    throw new System.ArgumentException(String.Format("UpdateMCPWorkload: Error connecting to Dimension Data MCP {0}", ex.Message));
                }
                Logger.log(String.Format("UpdateMCPWorkload: Inventory for {0} in {1} ", _caasworkload.name, _platform.moid), Logger.Severity.Info);

                //Retrieve portal objects
                if (_mrp_workloads == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Workload list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_workloads = _cloud_movey.workload().listworkloads().workloads.ToList();
                }
                if (_mrp_domains == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Network Domain list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_domains = _cloud_movey.platformdomain().listplatformdomains().platformdomains.Where(x => x.platform_id == _platform_id).ToList();
                }
                if (_mrp_networks == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Network VLAN list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_networks = _cloud_movey.platformnetwork().listplatformnetworks().platformnetworks.Where(x => _mrp_domains.Exists(y => y.id == x.platformdomain_id)).ToList();
                }
                if (_mrp_templates == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Template list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_templates = _cloud_movey.platformtemplate().listplatformtemplates();
                }

                //if workload is local, updated the local db record
                //User might use these servers later...
                bool _new_workload_flag = true;
                using (WorkloadSet _workload_db_set = new WorkloadSet())
                {
                    Workload _new_workload = new Workload();
                    if (_workload_db_set.ModelRepository.Get().Exists(x => x.moid == _caasworkload.id))
                    {
                        _new_workload_flag = false;
                        _new_workload = _workload_db_set.ModelRepository.Get().FirstOrDefault(x => x.moid == _caasworkload.id);
                    }
                    else
                    {
                        _new_workload_flag = true;
                        _new_workload.id = Objects.RamdomGuid();
                    }
                    //if we have this server in the platform, set the id for this server from the server of the platform
                    if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                    {
                        MRPWorkloadType _mrp_workload = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                        //it seems the manager has a MOID that matches but the GUID is different
                        //We need to remove the workload from the manager and add it back with the correct guid
                        if (!_new_workload_flag && (_new_workload.id != _mrp_workload.id))
                        {
                            _workload_db_set.ModelRepository.Delete(_new_workload.id);
                            _new_workload = new Workload();
                            _new_workload_flag = true;
                            Logger.log(String.Format("UpdateMCPWorkload: GUID missmatch {0} in {1} ", _caasworkload.name, _caasworkload.id), Logger.Severity.Info);
                        }
                        _new_workload.id = _mrp_workload.id;
                    }

                    _new_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                    _new_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                    _new_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                    _new_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                    _new_workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                    if (String.IsNullOrEmpty(_new_workload.hostname))
                    {
                        _new_workload.hostname = _caasworkload.name;
                    }
                    _new_workload.moid = _caasworkload.id;
                    _new_workload.platform_id = _platform_id;
                    _new_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                    _new_workload.osedition = _caasworkload.operatingSystem.displayName;
                    if (_new_workload_flag)
                    {
                        _workload_db_set.ModelRepository.Insert(_new_workload);
                    }

                    if (_new_workload.enabled == true)
                    {
                        MRPWorkloadCRUDType _crud_mrp_workload = new MRPWorkloadCRUDType();
                        _crud_mrp_workload.workloadinterfaces_attributes = new List<MRPWorkloadInterfaceType>();

                        Objects.Copy(_new_workload, _crud_mrp_workload);

                        //update workload source template id with portal template id
                        _crud_mrp_workload.platformtemplate_id = _mrp_templates.platformtemplates.Find(x => x.image_moid == _caasworkload.sourceImageId).id;

                        //populate network interfaces for workload
                        MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
                        if (_mrp_workloads.Any(x => x.interfaces.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
                        {
                            _primary_logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
                        }
                        _crud_mrp_workload.workloadinterfaces_attributes.Add(_primary_logical_interface);
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
                                    platformnetwork_id = _mrp_networks.FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id
                                };
                                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                                {
                                    if (_mrp_workloads.Exists((x => x.interfaces.Exists(y => y.moid == _caasworkloadinterface.id))))
                                    {
                                        _logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkloadinterface.id).id;
                                    }
                                }
                                _crud_mrp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                                nic_index += 1;
                            }
                        }

                        _crud_mrp_workload.provisioned = true;

                        //Update if the portal has this workload and create if it's new to the portal....
                        if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                        {
                            _crud_mrp_workload.id = _new_workload.id;
                            _cloud_movey.workload().updateworkload(_crud_mrp_workload);
                        }
                        else
                        {
                            _cloud_movey.workload().createworkload(_crud_mrp_workload);
                        }
                    }
                }
            }

        }

    }
}
