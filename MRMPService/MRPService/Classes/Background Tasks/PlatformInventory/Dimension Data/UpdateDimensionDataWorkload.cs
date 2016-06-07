using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.MRMPAPI;
using MRMPService.MRMPService.Log;

namespace MRMPService.PlatformInventory
{
    partial class PlatformInventoryWorkloadDo
    {
        public void UpdateMCPWorkload(String _workload_moid, MRPPlatformType _platform, List<MRPWorkloadType> _mrp_workloads = null, List<MRPPlatformdomainType> _mrp_domains = null, List<MRPPlatformnetworkType> _mrp_networks = null, List<MRPPlatformtemplateType> _mrp_templates = null)
        {
            using (MRMP_ApiClient _cloud_movey = new MRMP_ApiClient())
            {
                MRPCredentialType _platform_credential = _platform.credential;
                //create dimension data mcp object
                ServerType _caasworkload;
                try
                {
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credential.username, _platform_credential.encrypted_password));
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
                    _mrp_workloads = _cloud_movey.workload().list_by_platform(_platform).workloads.ToList();
                }
                if (_mrp_domains == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Network Domain list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_domains = _cloud_movey.platformdomain().list_by_platform(_platform).platformdomains.ToList();
                }
                if (_mrp_networks == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Network VLAN list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_networks = _cloud_movey.platformnetwork().list_by_platform(_platform).platformnetworks.ToList();
                }
                if (_mrp_templates == null)
                {
                    Logger.log(String.Format("UpdateMCPWorkload: Template list empty, fecthing new list"), Logger.Severity.Info);
                    _mrp_templates = _cloud_movey.platformtemplate().list_by_platform(_platform).platformtemplates;
                }

                MRPWorkloadType _mrmp_workload = new MRPWorkloadType();
                MRPWorkloadType _current_mrmp_workload = new MRPWorkloadType();
                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _mrmp_workload.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).id;
                    _current_mrmp_workload = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                }

                //dont update these attributes is they contain a value. We favour the OS updated information above the platform's information
                if (_current_mrmp_workload.vcpu == null) _mrmp_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                if (_current_mrmp_workload.vcore == null) _mrmp_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                if (_current_mrmp_workload.vmemory == null) _mrmp_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                if (_current_mrmp_workload.iplist == null) _mrmp_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                if (_current_mrmp_workload.hostname == null) _mrmp_workload.hostname = _caasworkload.name;
                if (_current_mrmp_workload.moid == null) _mrmp_workload.moid = _caasworkload.id;
                if (_current_mrmp_workload.platform_id == null) _mrmp_workload.platform_id = _platform.id;
                if (_current_mrmp_workload.ostype == null) _mrmp_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                if (_current_mrmp_workload.osedition == null) _mrmp_workload.osedition = _caasworkload.operatingSystem.displayName;

                //update workload source template id with portal template id
                _mrmp_workload.platformtemplate_id = _mrp_templates.FirstOrDefault(x => x.image_moid == _caasworkload.sourceImageId).id;

                //populate network interfaces for workload
                _mrmp_workload.workloadinterfaces_attributes = new List<MRPWorkloadInterfaceType>();
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
                            if (_mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces_attributes.Any((y => y.moid == _caasworkloadinterface.id)))
                            {
                                _logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces_attributes.FirstOrDefault(y => y.moid == _caasworkloadinterface.id).id;
                            }
                        }

                        _logical_interface.vnic = nic_index;
                        _logical_interface.ipassignment = "manual_ip";
                        _logical_interface.ipv6address = _caasworkloadinterface.ipv6;
                        _logical_interface.ipaddress = _caasworkloadinterface.privateIpv4;
                        _logical_interface.moid = _caasworkloadinterface.id;
                        _logical_interface._destroy = false;
                        _logical_interface.platformnetwork_id = _mrp_networks.FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id;
                        nic_index += 1;

                        _mrmp_workload.workloadinterfaces_attributes.Add(_logical_interface);
                    }
                }
                //Update if the portal has this workload and create if it's new to the portal....

                //remove credential, platform from workload object
                _mrmp_workload.credential = null;
                _mrmp_workload.platform = null;
                _mrmp_workload.provisioned = true;
                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _cloud_movey.workload().updateworkload(_mrmp_workload);
                }
                else
                {
                    _mrmp_workload.credential_id = _platform.default_credential_id;
                    _cloud_movey.workload().createworkload(_mrmp_workload);
                }
            }
        }

    }
}
