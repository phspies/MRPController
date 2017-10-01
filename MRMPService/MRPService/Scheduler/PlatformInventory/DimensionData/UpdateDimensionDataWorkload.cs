using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.MRMPService.Log;
using System.Threading.Tasks;

namespace MRMPService.Scheduler.PlatformInventory.DimensionData
{
    partial class PlatformInventoryWorkloadDo
    {
        public static void UpdateMCPWorkload(String _workload_moid, MRPPlatformType _platform)
        {
            try
            {
                //create dimension data mcp object
                ServerType _caasworkload;
                IEnumerable<TagType> _caas_tags;
                MRPOrganizationType _org_tags = MRMPServiceBase._mrmp_api.organization().get();
                try
                {
                    ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.decrypted_password));
                    CaaS.Login().Wait();
                    _caasworkload = CaaS.ServerManagement.Server.GetServer(Guid.Parse(_workload_moid)).Result;
                    _caas_tags = CaaS.Tagging.GetTags(new DD.CBU.Compute.Api.Contracts.Requests.Tagging.TagListOptions() { AssetId = _caasworkload.id, AssetType = "SERVER" }).Result;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(String.Format("UpdateMCPWorkload: Error connecting to Dimension Data MCP {0}", ex.GetBaseException().Message));
                }
                Logger.log(String.Format("UpdateMCPWorkload: Inventory for {0} in {1} ", _caasworkload.name, _platform.platformdatacenter.moid), Logger.Severity.Info);

                MRMPWorkloadBaseType _update_workload = new MRMPWorkloadBaseType();
                MRMPWorkloadBaseType _current_workload = new MRMPWorkloadBaseType();

                bool _existing_workload = false;
                if (_platform.workloads.Exists(x => x.moid == _caasworkload.id))
                {
                    _existing_workload = true;
                    _current_workload = _platform.workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                }
                else if (_platform.workloads.Exists(x => x.hostname == _caasworkload.name))
                {
                    _existing_workload = true;
                    _current_workload = _platform.workloads.FirstOrDefault(x => x.hostname == _caasworkload.name);
                }

                if (_existing_workload)
                {
                    _update_workload.id = _current_workload.id;
                    _update_workload.workloaddisks = _current_workload.workloaddisks;
                    _update_workload.workloadinterfaces = _current_workload.workloadinterfaces;
                    _update_workload.workloadtags = _current_workload.workloadtags;
                    _update_workload.workloadtags.ForEach(x => x.deleted = true);
                    _update_workload.workloadinterfaces.ForEach(x => x.deleted = true);
                    _update_workload.workloadinterfaces.ForEach(x => x._destroy = true);
                    _update_workload.workloadinterfaces.ForEach(x => x.platformnetwork = null);
                    _update_workload.workloaddisks.ForEach(x => x._destroy = true);
                    _update_workload.workloaddisks.ForEach(x => x.deleted = true);
                }
                else
                {
                    _update_workload.workloadinterfaces = new List<MRPWorkloadInterfaceType>();
                    _update_workload.workloadtags = new List<MRPWorkloadTagType>();
                    _update_workload.workloaddisks = new List<MRPWorkloadDiskType>();
                }
                foreach (var _caas_disk in _caasworkload.disk.Where(x => x.state == "NORMAL"))
                {
                    MRPWorkloadDiskType _mrmp_disk = new MRPWorkloadDiskType();
                    int _real_scsi_id = (_caas_disk.scsiId > 6) ? (_caas_disk.scsiId - 1) : _caas_disk.scsiId;
                    if (_update_workload.workloaddisks.Exists(x => x.moid == _caas_disk.id))
                    {
                        _mrmp_disk = _update_workload.workloaddisks.FirstOrDefault(x => x.moid == _caas_disk.id);
                    }
                    else if (_update_workload.workloaddisks.Exists(x => x.diskindex == _real_scsi_id))
                    {
                        _mrmp_disk = _update_workload.workloaddisks.FirstOrDefault(x => x.diskindex == _real_scsi_id);
                    }
                    else
                    {
                        _update_workload.workloaddisks.Add(_mrmp_disk);
                    }
                    _mrmp_disk.moid = _caas_disk.id;
                    _mrmp_disk.diskindex = _real_scsi_id;
                    _mrmp_disk.platformstoragetier_id = _caas_disk.speed;
                    _mrmp_disk.provisioned = true;
                    _mrmp_disk.deleted = false;
                    _mrmp_disk._destroy = false;
                    _mrmp_disk.disksize = _caas_disk.sizeGb;
                }


                //dont update these attributes is they contain a value. We favour the OS updated information above the platform's information
                if (_current_workload.vcpu == null) _update_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                if (_current_workload.vcore == null) _update_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                if (_current_workload.vmemory == null) _update_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                if (String.IsNullOrWhiteSpace(_current_workload.iplist)) _update_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.privateIpv4, _caasworkload.networkInfo.primaryNic.ipv6);
                if (String.IsNullOrWhiteSpace(_current_workload.hostname)) _update_workload.hostname = _caasworkload.name;
                _update_workload.moid = _caasworkload.id;
                _update_workload.platform_id = _platform.id;
                if (String.IsNullOrWhiteSpace(_current_workload.ostype)) _update_workload.ostype = _caasworkload.guest.operatingSystem.family.ToLower();
                if (String.IsNullOrWhiteSpace(_current_workload.osedition)) _update_workload.osedition = _caasworkload.guest.operatingSystem.displayName;
                _update_workload.hardwaretype = "virtual";
                if (_caasworkload.drsEligible is drsEligible || _caasworkload.consistencyGroup != null)
                {
                    _update_workload.drs_eligible = true;
                }
                else
                {
                    _update_workload.drs_eligible = true;
                }
                if (_caasworkload.consistencyGroup != null)
                {
                    _update_workload.consistency_group_moid = _caasworkload.consistencyGroup.id;
                }
                //update workload source template id with portal template id
                if (_platform.platformtemplates.Exists(x => x.image_moid == _caasworkload.sourceImageId))
                {
                    _update_workload.platformtemplate_id = _platform.platformtemplates.FirstOrDefault(x => x.image_moid == _caasworkload.sourceImageId).id;
                }
                _update_workload.platformdomain_id = _platform.platformdomains.SelectMany(y => y.platformnetworks).FirstOrDefault(x => x.moid == _caasworkload.networkInfo.primaryNic.vlanId).platformdomain_id;

                //populate network interfaces for workload
                MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType();
                if (_update_workload.workloadinterfaces.Exists(y => y.moid == _caasworkload.networkInfo.primaryNic.id))
                {
                    _primary_logical_interface = _update_workload.workloadinterfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id);
                }
                else
                {
                    _update_workload.workloadinterfaces.Add(_primary_logical_interface);
                }
                _primary_logical_interface.vnic = 0;
                _primary_logical_interface.ipassignment = "manual_ip";
                _primary_logical_interface.ipv6address = _caasworkload.networkInfo.primaryNic.ipv6;
                _primary_logical_interface.ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4;
                _primary_logical_interface.moid = _caasworkload.networkInfo.primaryNic.id;
                _primary_logical_interface.macaddress = _caasworkload.networkInfo.primaryNic.macAddress;
                _primary_logical_interface.deleted = false;
                _primary_logical_interface._destroy = false;
                _primary_logical_interface.platformnetwork_id = _platform.platformdomains.SelectMany(x => x.platformnetworks).FirstOrDefault(x => x.moid == _caasworkload.networkInfo.primaryNic.vlanId).id;
                _primary_logical_interface.platformnetwork = null;

                int nic_index = 1;
                if (_caasworkload.networkInfo.additionalNic != null)
                {
                    foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                    {
                        MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType();

                        if (_update_workload.workloadinterfaces.Any((y => y.moid == _caasworkloadinterface.id)))
                        {
                            _logical_interface = _update_workload.workloadinterfaces.FirstOrDefault(y => y.moid == _caasworkloadinterface.id);
                        }
                        else if (_update_workload.workloadinterfaces.Any((y => y.vnic == _caasworkloadinterface.key)))
                        {
                            _logical_interface = _update_workload.workloadinterfaces.FirstOrDefault(y => y.vnic == (_caasworkloadinterface.key - 4000));
                        }
                        else
                        {
                            _update_workload.workloadinterfaces.Add(_logical_interface);
                        }

                        _logical_interface.vnic = nic_index;
                        _logical_interface.ipassignment = "manual_ip";
                        _logical_interface.ipv6address = _caasworkloadinterface.ipv6;
                        _logical_interface.ipaddress = _caasworkloadinterface.privateIpv4;
                        _logical_interface.moid = _caasworkloadinterface.id;
                        _logical_interface.macaddress = _caasworkloadinterface.macAddress;
                        _logical_interface.deleted = false;
                        _logical_interface._destroy = false;
                        _logical_interface.platformnetwork_id = _platform.platformdomains.SelectMany(x => x.platformnetworks).FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id;
                        _logical_interface.platformnetwork = null;
                        nic_index += 1;
                    }
                }
                //Update if the portal has this workload and create if it's new to the portal....

                //remove credential, platform from workload object
                _update_workload.credential = null;
                _update_workload.platform = null;
                _update_workload.provisioned = true;

                //update workload tags
                if (_caas_tags != null)
                {
                    if (_org_tags.organizationtags != null)
                    {
                        foreach (var _caas_tag in _caas_tags)
                        {
                            MRPWorkloadTagType _workload_tag = new MRPWorkloadTagType();
                            if (_org_tags.organizationtags.Exists(x => x.tagkeyid == _caas_tag.tagKeyId))
                            {
                                MRPOrganizationTagType _tag = _org_tags.organizationtags.FirstOrDefault(x => x.tagkeyid == _caas_tag.tagKeyId);
                                if (_update_workload.workloadtags.Exists(x => x.organizationtag_id == _tag.id))
                                {
                                    _workload_tag = _update_workload.workloadtags.FirstOrDefault(x => x.organizationtag_id == _tag.id);
                                }
                                else
                                {
                                    _update_workload.workloadtags.Add(_workload_tag);
                                }
                                _workload_tag.organizationtag_id = _tag.id;
                                if (_caas_tag.valueSpecified)
                                {
                                    _workload_tag.tagvalue = _caas_tag.value;
                                }
                                _workload_tag.deleted = false;
                            }
                        }
                    }
                }

                if (_existing_workload)
                {
                    MRMPServiceBase._mrmp_api.workload().updateworkload(_update_workload);
                }
                else
                {
                    _update_workload.credential_id = _platform.default_credential_id;
                    MRMPServiceBase._mrmp_api.workload().createworkload(_update_workload);
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error collecting inventory information from CaaS workload {0} with error {1}", _workload_moid, ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
