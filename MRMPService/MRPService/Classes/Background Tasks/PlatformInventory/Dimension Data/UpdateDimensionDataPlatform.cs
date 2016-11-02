using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.General;
using MRMPService.MRMPAPI;
using DD.CBU.Compute.Api.Contracts.Requests.Network20;

namespace MRMPService.PlatformInventory
{
    class PlatformDimensionDataMCP2InventoryDo : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlatformDimensionDataMCP2InventoryDo()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        public static void UpdateMCPPlatform(MRPPlatformType _platform, bool full = true)
        {
            using (MRMP_ApiClient _mrp_api_endpoint = new MRMP_ApiClient())
            {
                Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.moid), Logger.Severity.Info);
                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

                //define object lists
                MRPCredentialType _credential = _platform.credential;
                MRPPlatformType _update_platform = new MRPPlatformType() { id = _platform.id };

                List<MRPWorkloadType> _mrp_workloads = new List<MRPWorkloadType>();
                using (MRMP_ApiClient _mrmp_api = new MRMP_ApiClient())
                {
                    MRPWorkloadListType _paged_workload = _mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id });
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

                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_credential.username, _credential.encrypted_password));
                try
                {
                    CaaS.Login().Wait();
                }
                catch (Exception ex)
                {
                    throw new System.ArgumentException(String.Format("Error connecting to MCP Endpoint {0}", ex.ToString()));
                }

                //mirror platorm templates for this platform
                List<OsImageType> _caas_templates = CaaS.ServerManagement.ServerImage.GetOsImages(new ServerOsImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result.items.ToList();


                //preload all templates and mark them as deleted
                if (_platform.platformtemplates_attributes == null)
                {
                    _update_platform.platformtemplates_attributes = new List<MRPPlatformtemplateType>();
                }
                else
                {
                    _update_platform.platformtemplates_attributes = _platform.platformtemplates_attributes;
                }
                _update_platform.platformtemplates_attributes.ForEach(x => x.deleted = true);

                foreach (OsImageType _caas_template in _caas_templates)
                {
                    MRPPlatformtemplateType _mrp_template = new MRPPlatformtemplateType();
                    if (_update_platform.platformtemplates_attributes.Exists(x => x.image_moid == _caas_template.id))
                    {
                        _mrp_template = _update_platform.platformtemplates_attributes.FirstOrDefault(x => x.image_moid == _caas_template.id);
                    }
                    else
                    {
                        _update_platform.platformtemplates_attributes.Add(_mrp_template);
                    }

                    _mrp_template.image_type = _caas_template.softwareLabel == null ? "os" : "software";
                    _mrp_template.image_description = _caas_template.description;
                    _mrp_template.platform_id = _platform.id;
                    _mrp_template.image_moid = _caas_template.id;
                    _mrp_template.image_name = _caas_template.name;
                    _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                    _mrp_template.os_id = _caas_template.operatingSystem.id;
                    _mrp_template.os_type = _caas_template.operatingSystem.family;
                    _mrp_template.platform_moid = _platform.moid;
                    _mrp_template.deleted = false;
                }

                //process customer images
                PagedResponse<CustomerImageType> _customer_templates = CaaS.ServerManagement.ServerImage.GetCustomerImages(new ServerCustomerImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result;
                if (_customer_templates.totalCount > 0)
                {
                    foreach (var _caas_template in _customer_templates.items)
                    {
                        MRPPlatformtemplateType _mrp_template = new MRPPlatformtemplateType();
                        if (_update_platform.platformtemplates_attributes.Exists(x => x.image_moid == _caas_template.id))
                        {
                            _mrp_template = _update_platform.platformtemplates_attributes.FirstOrDefault(x => x.image_moid == _caas_template.id);
                        }
                        else
                        {
                            _update_platform.platformtemplates_attributes.Add(_mrp_template);
                        }

                        _mrp_template.image_type = "os";
                        _mrp_template.image_description = _caas_template.description;
                        _mrp_template.image_moid = _caas_template.id;
                        _mrp_template.platform_id = _platform.id;
                        _mrp_template.image_name = _caas_template.name;
                        _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                        _mrp_template.os_id = _caas_template.operatingSystem.id;
                        _mrp_template.os_type = _caas_template.operatingSystem.family;
                        _mrp_template.platform_moid = _platform.moid;
                        _mrp_template.deleted = false;


                    }
                }

                //populate domain and networks into update object
                if (_platform.platformdomains_attributes == null)
                {
                    _update_platform.platformdomains_attributes = new List<MRPPlatformdomainType>();
                }
                else
                {
                    _update_platform.platformdomains_attributes = _platform.platformdomains_attributes;
                }
                _update_platform.platformdomains_attributes.ForEach(x => x.deleted = true);
                _update_platform.platformdomains_attributes.ForEach(x => x.platformnetworks_attributes.ForEach(y => y.deleted = true));


                IEnumerable<NetworkDomainType> _caas_networkdomain_list = CaaS.Networking.NetworkDomain.GetNetworkDomains(new NetworkDomainListOptions() { DatacenterId = _platform.moid }).Result;

                IEnumerable<VlanType> _caas_vlan_list = CaaS.Networking.Vlan.GetVlans(new VlanListOptions() { DatacenterId = _platform.moid }).Result;

                if (_caas_networkdomain_list != null)
                {
                    foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                    {
                        MRPPlatformdomainType _mrp_domain = new MRPPlatformdomainType();
                        if (_update_platform.platformdomains_attributes.Exists(x => x.moid == _caas_domain.id))
                        {
                            _mrp_domain = _update_platform.platformdomains_attributes.FirstOrDefault(x => x.moid == _caas_domain.id);
                        }
                        else
                        {
                            _update_platform.platformdomains_attributes.Add(_mrp_domain);
                        }

                        if (_mrp_domain.platformnetworks_attributes == null)
                        {
                            _mrp_domain.platformnetworks_attributes = new List<MRPPlatformnetworkType>();
                        }

                        _mrp_domain.moid = _caas_domain.id;
                        _mrp_domain.domain = _caas_domain.name;
                        _mrp_domain.platform_id = _platform.id;
                        _mrp_domain.deleted = false;



                        VlanListOptions _vlan_options = new VlanListOptions() { DatacenterId = _platform.moid };
                        IEnumerable<VlanType> _vlans = CaaS.Networking.Vlan.GetVlans(_vlan_options).Result;
                        foreach (VlanType _caas_network in _vlans.Where(x => x.networkDomain.id == _caas_domain.id))
                        {
                            MRPPlatformnetworkType _mrp_network = new MRPPlatformnetworkType();
                            if (_mrp_domain.platformnetworks_attributes.Any(y => y.moid == _caas_network.id))
                            {
                                _mrp_network = _mrp_domain.platformnetworks_attributes.FirstOrDefault(y => y.moid == _caas_network.id);
                            }
                            else
                            {
                                _mrp_domain.platformnetworks_attributes.Add(_mrp_network);
                            }

                            _mrp_network.moid = _caas_network.id;
                            _mrp_network.network = _caas_network.name;
                            _mrp_network.description = _caas_network.description;
                            _mrp_network.platformdomain_id = _mrp_domain.id;
                            _mrp_network.ipv4subnet = _caas_network.privateIpv4Range.address;
                            _mrp_network.ipv4netmask = _caas_network.privateIpv4Range.prefixSize;
                            _mrp_network.ipv6subnet = _caas_network.ipv6Range.address;
                            _mrp_network.ipv6netmask = _caas_network.ipv6Range.prefixSize;
                            _mrp_network.networkdomain_moid = _caas_network.networkDomain.id;
                            _mrp_network.provisioned = true;
                            _mrp_network.deleted = false;


                        }
                        //Workaround
                        //if no networks were found, set platformnetworks_attributes to null
                        if (_mrp_domain.platformnetworks_attributes.Count == 0)
                        {
                            _mrp_domain.platformnetworks_attributes = null;
                        }

                    }

                }
                //nullify all networks in domains where domain was deleted
                _update_platform.platformdomains_attributes.Where(x => x.deleted == true).ForEach(x => x.platformnetworks_attributes = null);



                if (_update_platform.platformdomains_attributes.Count == 0)
                {
                    _update_platform.platformdomains_attributes = null;
                }
                //update platform with nested object lists
                _mrp_api_endpoint.platform().update(_update_platform);

                var _tmp_server_list = CaaS.ServerManagement.Server.GetServers(new ServerListOptions() { DatacenterId = _platform.moid, State = "NORMAL" }).Result;
                List<ServerType> _caas_workload_list = new List<ServerType>();
                if (_tmp_server_list != null)
                {
                    _caas_workload_list = _tmp_server_list.ToList();
                }
                //process deleted platform workloads
                foreach (var _workload in _mrp_workloads.Where(x => x.platform_id == _platform.id && x.workloadtype != "manager"))
                {
                    if (!_caas_workload_list.Any(x => x.id == _workload.moid))
                    {
                        Logger.log(String.Format("Portal workload {0} {1} is not found in the MCP {2}", _workload.hostname, _workload.moid, _platform.moid), Logger.Severity.Info);
                        MRPWorkloadType _removed_workload = new MRPWorkloadType();
                        _removed_workload.id = _workload.id;
                        _removed_workload.deleted = true;
                        _removed_workload.enabled = false;
                        using (MRMP_ApiClient _api = new MRMP_ApiClient())
                        {
                            _api.workload().updateworkload(_removed_workload);
                        }
                        _removed_workloads += 1;
                    }
                }
                //refresh platform from portal
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _platform = _api.platform().get_by_id(_platform.id);
                }

                if (_caas_workload_list.Count() > 0)
                {
                    if (full)
                    {
                        //update lists before we start the workload inventory process
                        foreach (ServerType _caasworkload in _caas_workload_list)
                        {
                            PlatformInventoryWorkloadDo.UpdateMCPWorkload(_caasworkload.id, _platform, _mrp_workloads);
                        }
                    }
                }
                sw.Stop();

                Logger.log(
                    String.Format("Completed inventory process for {5} - {0} new workloads - {1} updated platform networks - {2} updated workloads - {3} removed workloads = Total Execute Time: {4}",
                    _new_workloads, _updated_platforms, _updated_workloads, _removed_workloads,
                     TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), (_platform.platformtype + " : " + _platform.moid)
                    ), Logger.Severity.Info);
            }
        }

    }
}
