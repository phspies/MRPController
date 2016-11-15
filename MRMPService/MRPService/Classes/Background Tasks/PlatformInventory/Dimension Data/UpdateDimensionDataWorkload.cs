using MRMPService.MRMPAPI.Contracts;
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
        public static void UpdateMCPWorkload(String _workload_moid, MRPPlatformType _platform)
        {
            using (MRMP_ApiClient _mrmp_api = new MRMP_ApiClient())
            {
                //create dimension data mcp object
                ServerType _caasworkload;
                try
                {
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.encrypted_password));
                    CaaS.Login().Wait();
                    _caasworkload = CaaS.ServerManagement.Server.GetServer(Guid.Parse(_workload_moid)).Result;
                }
                catch (Exception ex)
                {
                    throw new System.ArgumentException(String.Format("UpdateMCPWorkload: Error connecting to Dimension Data MCP {0}", ex.Message));
                }
                Logger.log(String.Format("UpdateMCPWorkload: Inventory for {0} in {1} ", _caasworkload.name, _platform.moid), Logger.Severity.Info);

                List<MRPWorkloadType> _mrp_workloads = _platform.workloads_attributes;
                if (_mrp_workloads == null)
                {
                    MRPWorkloadListType _paged_workload = _mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = 1 });
                    _mrp_workloads = new List<MRPWorkloadType>();
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
                //get full platform inventory

                _platform = _mrmp_api.platform().get_by_id(_platform.id);

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
                _mrmp_workload.platformtemplate_id = _platform.platformtemplates_attributes.FirstOrDefault(x => x.image_moid == _caasworkload.sourceImageId).id;

                //populate network interfaces for workload
                _mrmp_workload.workloadinterfaces = new List<MRPWorkloadInterfaceType>();
                MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
                if (_mrp_workloads.Any(x => x.workloadinterfaces.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
                {
                    _primary_logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
                }
                _mrmp_workload.workloadinterfaces.Add(_primary_logical_interface);
                int nic_index = 1;
                if (_caasworkload.networkInfo.additionalNic != null)
                {
                    foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                    {
                        MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType();
                        if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                        {
                            if (_mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces.Any((y => y.moid == _caasworkloadinterface.id)))
                            {
                                _logical_interface.id = _mrp_workloads.FirstOrDefault(x => x.moid == _caasworkload.id).workloadinterfaces.FirstOrDefault(y => y.moid == _caasworkloadinterface.id).id;
                            }
                        }

                        _logical_interface.vnic = nic_index;
                        _logical_interface.ipassignment = "manual_ip";
                        _logical_interface.ipv6address = _caasworkloadinterface.ipv6;
                        _logical_interface.ipaddress = _caasworkloadinterface.privateIpv4;
                        _logical_interface.moid = _caasworkloadinterface.id;
                        _logical_interface._destroy = false;
                        _logical_interface.platformnetwork_id = _platform.platformdomains_attributes.SelectMany(x => x.platformnetworks).FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id;

                        _mrmp_workload.workloadinterfaces.Add(_logical_interface);
                        nic_index += 1;
                    }
                }
                //Update if the portal has this workload and create if it's new to the portal....

                //remove credential, platform from workload object
                _mrmp_workload.credential = null;
                _mrmp_workload.platform = null;
                _mrmp_workload.provisioned = true;
                if (_mrp_workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _mrmp_api.workload().updateworkload(_mrmp_workload);
                }
                else
                {
                    _mrmp_workload.credential_id = _platform.default_credential_id;
                    _mrmp_api.workload().createworkload(_mrmp_workload);
                }
            }
        }

    }
}
