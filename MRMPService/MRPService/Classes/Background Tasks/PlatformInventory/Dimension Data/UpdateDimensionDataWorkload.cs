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
        public void UpdateMCPWorkload(String _workload_moid, MRPPlatformType _platform, List<MRPWorkloadType> _mrp_workloads = null, List<MRPPlatformdomainType> _mrp_domains = null, List<MRPPlatformnetworkType> _mrp_networks = null, MRPPlatformtemplateListType _mrp_templates = null)
        {
            using (MRP_ApiClient _cloud_movey = new MRP_ApiClient())
            {
                //create dimension data mcp object
                ServerType _caasworkload;
                try
                {
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.username, _platform.password));
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
                    _mrp_domains = _cloud_movey.platformdomain().listplatformdomains().platformdomains.Where(x => x.platform_id == _platform.id).ToList();
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

                MRPWorkloadType _mrmp_workload = new MRPWorkloadType();
                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _mrmp_workload = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                }

                _mrmp_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                _mrmp_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                _mrmp_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                _mrmp_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                if (String.IsNullOrEmpty(_mrmp_workload.hostname))
                {
                    _mrmp_workload.hostname = _caasworkload.name;
                }
                _mrmp_workload.moid = _caasworkload.id;
                _mrmp_workload.platform_id = _platform.id;
                _mrmp_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                _mrmp_workload.osedition = _caasworkload.operatingSystem.displayName;

                //update workload source template id with portal template id
                _mrmp_workload.platformtemplate_id = _mrp_templates.platformtemplates.FirstOrDefault(x => x.image_moid == _caasworkload.sourceImageId).id;

                //populate network interfaces for workload
                MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
                if (_mrp_workloads.Any(x => x.workloadinterfaces_attributes.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
                {
                    _primary_logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces_attributes.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
                }
                _mrmp_workload.workloadinterfaces_attributes.Add(_primary_logical_interface);
                int nic_index = 1;
                if (_caasworkload.networkInfo.additionalNic != null)
                {
                    foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                    {
                        MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType();
                        if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                        {
                            if (_mrp_workloads.Any((x => x.workloadinterfaces_attributes.Any(y => y.moid == _caasworkloadinterface.id))))
                            {
                                _logical_interface = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces_attributes.FirstOrDefault(y => y.moid == _caasworkloadinterface.id);
                            }
                            else
                            {
                                _mrmp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                            }
                        }
                        else
                        {
                            _mrmp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                        }
                        _logical_interface.vnic = nic_index;
                        _logical_interface.ipassignment = "manual_ip";
                        _logical_interface.ipv6address = _caasworkloadinterface.ipv6;
                        _logical_interface.ipaddress = _caasworkloadinterface.privateIpv4;
                        _logical_interface.moid = _caasworkloadinterface.id;
                        _logical_interface._destroy = false;
                        _logical_interface.platformnetwork_id = _mrp_networks.FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id;
                        nic_index += 1;
                    }
                }
                //Update if the portal has this workload and create if it's new to the portal....
                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _cloud_movey.workload().updateworkload(_mrmp_workload);
                }
                else
                {
                    _cloud_movey.workload().createworkload(_mrmp_workload);
                }
            }
        }

    }
}
