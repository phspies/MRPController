﻿using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.RP4VMTypes;
using MRMPService.RP4VMAPI;
using MRMPService.VMWare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VMware.Vim;

namespace MRMPService.TaskExecutioner.Platform

{
    public class DatacenterDiscovery
    {
        public static void DatacenterDiscoveryDo(MRPTaskType payload)
        {
            try
            {
                payload.progress(String.Format("Starting datacenter discovering process"), 5);
                MRPPlatformType _platform = payload.taskdetail.target_platform;
                MRPCredentialType _platform_credentail = _platform.credential;
                switch (_platform.platformtype)
                {
                    case "dimension_data":
                        ComputeApiClient CaaS = null;
                        MRPPlatformdatacenterListType _mrmp_datacenters = null;
                        try
                        {
                            payload.progress(String.Format("Retrieving datacenters from {0} for type MCP", _platform.url), 10);
                            _mrmp_datacenters = MRMPServiceBase._mrmp_api.platformdatacenter().list(_platform);
                            payload.progress(String.Format("Retrieving datacenters from platform for {0}", _platform.platform), 11);
                            CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credentail.username, _platform_credentail.decrypted_password));
                            CaaS.Login().Wait();
                        }
                        catch (Exception ex)
                        {
                            payload.progress(String.Format("Error Connecting to MCP {0}", ex.Message), 15);
                            payload.failcomplete(ex.ToString());
                            return;
                        }

                        List<DatacenterType> _mcp_datacenters = CaaS.Infrastructure.GetDataCenters().Result.ToList();
                        if (_mcp_datacenters != null)
                        {
                            payload.progress(String.Format("Found {0} datacenters", _mcp_datacenters.Count), 15);
                            foreach (DatacenterType _dc in _mcp_datacenters)
                            {
                                MRPPlatformdatacenterType _platform_datacenter = new MRPPlatformdatacenterType();
                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.id))
                                {
                                    _platform_datacenter.id = _mrmp_datacenters.platformdatacenters.FirstOrDefault(x => x.moid == _dc.id).id;
                                }
                                _platform_datacenter.moid = _dc.id;
                                _platform_datacenter.diskspeeds = _dc.hypervisor.diskSpeed;
                                _platform_datacenter.cpuspeeds = _dc.hypervisor.cpuSpeed;
                                _platform_datacenter.vpn_url = _dc.vpnUrl;
                                _platform_datacenter.city = _dc.city;
                                _platform_datacenter.country = _dc.country;
                                _platform_datacenter.displayname = _dc.displayName;
                                if (_dc.drs != null)
                                {
                                    _platform_datacenter.target_drs_moid_list = _dc.drs.targetDatacenters.list;
                                }
                                _platform_datacenter.platform_id = _platform.id;

                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.id))
                                {
                                    MRMPServiceBase._mrmp_api.platformdatacenter().update(_platform_datacenter);
                                }
                                else
                                {
                                    MRMPServiceBase._mrmp_api.platformdatacenter().create(_platform_datacenter);
                                }
                            }
                        }
                        else
                        {
                            payload.progress(String.Format("Something went wrong, null based mcp server list"), 15);
                            payload.failcomplete(String.Format("Something went wrong, null based mcp server list"));
                            return;
                        }
                        payload.progress(String.Format("Successfully created/updated {0} datacenters", _mcp_datacenters.Count), 20);
                        payload.successcomplete(String.Format("Successfully created/updated {0} datacenters", _mcp_datacenters.Count));
                        break;
                    case "hyperv":
                        break;
                    case "vmware":
                        VimApiClient _vim;

                        try
                        {
                            payload.progress(String.Format("Retrieving datacenters from {0} for type VMWare", _platform.vmware_url), 10);
                            _mrmp_datacenters = MRMPServiceBase._mrmp_api.platformdatacenter().list(_platform);
                            payload.progress(String.Format("Retrieving datacenters from platform for {0}", _platform.platform), 15);
                            String username = String.Concat((String.IsNullOrEmpty(_platform_credentail.domain) ? "" : (_platform_credentail.domain + @"\")), _platform_credentail.username);
                            _vim = new VimApiClient(_platform.vmware_url, username, _platform_credentail.decrypted_password);
                            _vim.datacenter().DatacenterList();

                        }
                        catch (Exception ex)
                        {
                            payload.failcomplete(ex.ToString());
                            return;
                        }

                        List<Datacenter> _vmware_datacenters = _vim.datacenter().DatacenterList();
                        if (_vmware_datacenters != null)
                        {
                            payload.progress(String.Format("Found {0} datacenters", _vmware_datacenters.Count), 20);
                            foreach (Datacenter _dc in _vmware_datacenters)
                            {
                                MRPPlatformdatacenterType _platform_datacenter = new MRPPlatformdatacenterType();
                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.MoRef.Value))
                                {
                                    _platform_datacenter.id = _mrmp_datacenters.platformdatacenters.FirstOrDefault(x => x.moid == _dc.MoRef.Value).id;
                                }
                                _platform_datacenter.moid = _dc.MoRef.Value;
                                _platform_datacenter.displayname = _dc.Name;
                                _platform_datacenter.platform_id = _platform.id;

                                if (_mrmp_datacenters.platformdatacenters.Exists(x => x.moid == _dc.MoRef.Value))
                                {
                                    MRMPServiceBase._mrmp_api.platformdatacenter().update(_platform_datacenter);
                                }
                                else
                                {
                                    MRMPServiceBase._mrmp_api.platformdatacenter().create(_platform_datacenter);
                                }
                            }
                            payload.progress(String.Format("Successfully created/updated {0} datacenter(s)", _vmware_datacenters.Count), 30);
                            payload.successcomplete(String.Format("Successfully created/updated {0} datacenter(s)", _vmware_datacenters.Count));
                        }
                        else
                        {
                            payload.progress(String.Format("Something went wrong, null based vmware server list"), 30);
                            payload.failcomplete(String.Format("Something went wrong, null based vmware server list"));
                        }

                        break;
                    case "rp4vm":
                        RP4VM_ApiClient _rp4vm;
                        repositoryVolumeStateSet _repvolumes;

                        SystemSettings _rp4vm_settings = null;
                        try
                        {
                            payload.progress(String.Format("Retrieving clusters from {0} for type EMC RP4VM", _platform.rp4vm_url), 10);
                            _mrmp_datacenters = MRMPServiceBase._mrmp_api.platformdatacenter().list(_platform);
                            payload.progress(String.Format("Retrieving clusters from platform for {0}", _platform.platform), 15);
                            String username = String.Concat((String.IsNullOrEmpty(_platform_credentail.domain) ? "" : (_platform_credentail.domain + @"\")), _platform_credentail.username);
                            _rp4vm = new RP4VM_ApiClient(_platform.rp4vm_url, username, _platform_credentail.decrypted_password);
                            _rp4vm_settings = _rp4vm.system().getSystemSettings_Method();

                            _repvolumes = _rp4vm.reparrays().getRepositoryVolumeStateFromAllClusters_Method();

                        }
                        catch (Exception ex)
                        {
                            payload.progress(String.Format("{0}", ex.GetBaseException().Message), 20);
                        }
                        if (_rp4vm_settings != null)
                        {
                            MRPPlatformType _update_platform = new MRPPlatformType() { id = _platform.id };
                            _update_platform.platformdatacenters = new List<MRPPlatformdatacenterType>();
                            foreach (var _cluster in _rp4vm_settings.clustersSettings)
                            {
                                MRPPlatformdatacenterType _datacenter = new MRPPlatformdatacenterType();
                                if (_platform.platformdatacenters != null)
                                {
                                    if (_platform.platformdatacenters.Exists(x => x.moid == _cluster.clusterUID.id.ToString()))
                                    {
                                        _datacenter.id = _platform.platformdatacenters.FirstOrDefault(x => x.moid == _cluster.clusterUID.id.ToString()).id;
                                    }
                                }
                                _datacenter.moid = _cluster.clusterUID.id.ToString();
                                _datacenter.displayname = _cluster.clusterName;
                                _update_platform.platformdatacenters.Add(_datacenter);
                            }

                            MRMPServiceBase._mrmp_api.platform().update(_update_platform);

                            payload.progress(String.Format("Successfully created/updated {0} RP4VM clusters(s)", _rp4vm_settings.clustersSettings.Count()), 30);
                            payload.successcomplete(String.Format("Successfully created/updated {0} RP4VM clusters(s)", _rp4vm_settings.clustersSettings.Count()));
                        }
                        else
                        {
                            payload.progress(String.Format("Something went wrong, null based cluster list"), 30);
                            payload.failcomplete(String.Format("Something went wrong, null based cluster list"));
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                payload.progress(String.Format("Something went wrong, {0}", ex.Message), 30);
                payload.failcomplete(String.Format("Something went wrong, {0}", ex.Message));
            }
        }
    }
}
