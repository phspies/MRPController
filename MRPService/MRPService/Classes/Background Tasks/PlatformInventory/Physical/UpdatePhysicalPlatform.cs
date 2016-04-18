using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using DD.CBU.Compute.Api.Contracts.General;
using MRPService.Utilities;
using MRPService.API;

namespace MRPService.PlatformInventory
{
    class PlatformPhysicalInventoryDo
    {

        public static void UpdatePhysicalPlatform(String _platform_id, bool full=true)
        {
            MRP_ApiClient _mrp_api_endpoint = new MRP_ApiClient();
            Platform _platform;
            using (PlatformSet _platform_db = new PlatformSet())
            {
                _platform = _platform_db.ModelRepository.GetById(_platform_id);
            }
            try
            {
                Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.human_vendor, _platform.moid), Logger.Severity.Info);
                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

                //define object lists
                Credential _credential;
                using (MRPDatabase db = new MRPDatabase())
                {
                    List<Credential> _workercredentials = db.Credentials.ToList();
                    _credential = _workercredentials.FirstOrDefault(x => x.id == _platform.credential_id);

                }
                List<MRPWorkloadType> _mrp_workloads = _mrp_api_endpoint.workload().listworkloads().workloads.Where(x => x.platform_id == _platform_id).ToList();
                List<MRPPlatformdomainType> _mrp_domains = _mrp_api_endpoint.platformdomain().listplatformdomains().platformdomains.Where(x => x.platform_id == _platform_id).ToList();
                List<MRPPlatformnetworkType> _mrp_networks = _mrp_api_endpoint.platformnetwork().listplatformnetworks().platformnetworks.Where(x => _mrp_domains.Exists(y => y.id == x.platformdomain_id)).ToList();


                //populate platform credential object

                //create dimension data mcp object
                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_credential.username, _credential.password));
                CaaS.Login().Wait();


                //mirror platorm templates for this platform
                MRPPlatformtemplateListType _platformtemplates = _mrp_api_endpoint.platformtemplate().listplatformtemplates();
                List<OsImageType> _caas_templates = CaaS.ServerManagement.ServerImage.GetOsImages(new ServerOsImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result.items.ToList();

                foreach (OsImageType _caas_template in _caas_templates)
                {
                    MRPPlatformtemplateCRUDType _mrp_template = new MRPPlatformtemplateCRUDType();
                    _mrp_template.image_type = _caas_template.softwareLabel == null ? "os" : "software";
                    _mrp_template.image_description = _caas_template.description;
                    _mrp_template.platform_id = _platform.id;
                    _mrp_template.image_moid = _caas_template.id;
                    _mrp_template.image_name = _caas_template.name;
                    _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                    _mrp_template.os_id = _caas_template.operatingSystem.id;
                    _mrp_template.os_type = _caas_template.operatingSystem.family;
                    _mrp_template.platform_moid = _platform.moid;
                    if (_platformtemplates.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                    {
                        _mrp_template.id = _platformtemplates.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id).id;
                        _mrp_api_endpoint.platformtemplate().updateplatformtemplate(_mrp_template);
                    }
                    else
                    {
                        _mrp_api_endpoint.platformtemplate().createplatformtemplate(_mrp_template);
                    }
                }

                //process customer images
                PagedResponse<CustomerImageType> _customer_templates = CaaS.ServerManagement.ServerImage.GetCustomerImages(new ServerCustomerImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result;
                if (_customer_templates.totalCount > 0)
                {
                    foreach (var _caas_template in _customer_templates.items)
                    {
                        MRPPlatformtemplateCRUDType _mrp_template = new MRPPlatformtemplateCRUDType();
                        _mrp_template.image_type = "os";
                        _mrp_template.image_description = _caas_template.description;
                        _mrp_template.image_moid = _caas_template.id;
                        _mrp_template.platform_id = _platform.id;
                        _mrp_template.image_name = _caas_template.name;
                        _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                        _mrp_template.os_id = _caas_template.operatingSystem.id;
                        _mrp_template.os_type = _caas_template.operatingSystem.family;
                        _mrp_template.platform_moid = _platform.moid;
                        if (_platformtemplates.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                        {
                            _mrp_template.id = _platformtemplates.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id).id;
                            _mrp_api_endpoint.platformtemplate().updateplatformtemplate(_mrp_template);
                        }
                        else
                        {
                            _mrp_api_endpoint.platformtemplate().createplatformtemplate(_mrp_template);
                        }
                    }
                }

                //refresh templates
                _platformtemplates = _mrp_api_endpoint.platformtemplate().listplatformtemplates();

                //update localdb platform information
                List<NetworkDomainType> _caas_networkdomain_list = CaaS.Networking.NetworkDomain.GetNetworkDomains(new DD.CBU.Compute.Api.Contracts.Requests.Network20.NetworkDomainListOptions() { DatacenterId = _platform.moid }).Result.ToList();
                List<ServerType> _caas_workload_list = CaaS.ServerManagement.Server.GetServers(new DD.CBU.Compute.Api.Contracts.Requests.Server20.ServerListOptions() { DatacenterId = _platform.moid, State = "NORMAL" }).Result.ToList();
                List<VlanType> _caas_vlan_list = CaaS.Networking.Vlan.GetVlans(new DD.CBU.Compute.Api.Contracts.Requests.Network20.VlanListOptions() { DatacenterId = _platform.moid }).Result.ToList();
                DatacenterType _caas_dc = CaaS.Infrastructure.GetDataCenters(new PageableRequest() { PageSize = 250 }, new DataCenterListOptions() { Id = _platform.moid }).Result.ToList().FirstOrDefault();

                int workloads, networkdomains, vlans;
                string workloads_md5, networkdomains_md5, vlans_md5;
                workloads = networkdomains = vlans = 0;

                workloads = _caas_workload_list.Count();
                workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(_caas_workload_list));
                vlans = _caas_vlan_list.Count();
                vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(_caas_vlan_list));
                networkdomains = _caas_networkdomain_list.Count();
                networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(_caas_networkdomain_list));

                using (PlatformSet _platform_dbset = new PlatformSet())
                {
                     Platform _db_platform = _platform_dbset.ModelRepository.GetById(_platform.id);
                    _db_platform.vlan_count = vlans;
                    _db_platform.workload_count = workloads;
                    _db_platform.networkdomain_count = networkdomains;
                    _db_platform.platform_version = _caas_dc.type;

                    _db_platform.lastupdated = DateTime.Now;
                    _db_platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                    _db_platform.moid = _caas_dc.id;
                    _platform_dbset.Save();
                }

                foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                {
                    MRPPlatformdomainCRUDType _mrp_mdomain = new MRPPlatformdomainCRUDType();
                    _mrp_mdomain.platformnetworks_attributes = new List<MRPPlatformnetworkCRUDType>();
                    _mrp_mdomain.moid = _caas_domain.id;
                    _mrp_mdomain.domain = _caas_domain.name;
                    _mrp_mdomain.platform_id = _platform.id;

                    foreach (VlanType _caas_network in CaaS.Networking.Vlan.GetVlans(new DD.CBU.Compute.Api.Contracts.Requests.Network20.VlanListOptions() { NetworkDomainId = Guid.Parse(_caas_domain.id) }).Result.ToList())
                    {
                        MRPPlatformnetworkCRUDType _mrp_network = new MRPPlatformnetworkCRUDType();
                        _mrp_network.moid = _caas_network.id;
                        _mrp_network.network = _caas_network.name;
                        _mrp_network.description = _caas_network.description;
                        _mrp_network.platformdomain_id = _mrp_mdomain.id;
                        _mrp_network.ipv4subnet = _caas_network.privateIpv4Range.address;
                        _mrp_network.ipv4netmask = _caas_network.privateIpv4Range.prefixSize;
                        _mrp_network.ipv6subnet = _caas_network.ipv6Range.address;
                        _mrp_network.ipv6netmask = _caas_network.ipv6Range.prefixSize;
                        _mrp_network.networkdomain_moid = _caas_network.networkDomain.id;
                        _mrp_network.provisioned = true;
                        if (_mrp_networks.Exists(x => x.moid == _caas_network.id))
                        {
                            _mrp_network.id = _mrp_networks.FirstOrDefault(x => x.moid == _caas_network.id).id;
                            _updated_platformnetworks += 1;
                        }
                        else
                        {
                            _new_platformnetworks += 1;
                        }
                        _mrp_mdomain.platformnetworks_attributes.Add(_mrp_network);
                    }
                    if (_mrp_domains.Exists(x => x.moid == _caas_domain.id))
                    {
                        _mrp_mdomain.id = _mrp_domains.FirstOrDefault(x => x.moid == _caas_domain.id).id;
                        _mrp_api_endpoint.platformdomain().updateplatformdomain(_mrp_mdomain);
                        _updated_platformnetworks += 1;
                    }
                    else
                    {
                        _mrp_api_endpoint.platformdomain().createplatformdomain(_mrp_mdomain);
                        _new_platformnetworks += 1;
                    }
                }
                //refresh platform network list from portal
                _mrp_networks = _mrp_api_endpoint.platformnetwork().listplatformnetworks().platformnetworks.Where(x => _mrp_domains.Any(y => y.id == x.platformdomain_id)).ToList();

                //process workloads

                //process deleted platform workloads
                using (MRPDatabase db = new MRPDatabase())
                {
                    foreach (var _workload in db.Workloads.Where(x => x.platform_id == _platform.id))
                    {
                        if (!_caas_workload_list.Any(x => x.id == _workload.moid && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                        {
                            db.Workloads.Remove(db.Workloads.Find(_workload.id));
                            db.SaveChanges();
                            _removed_workloads += 1;
                        }
                    }
                }
                if (full)
                {
                    foreach (ServerType _caasworkload in _caas_workload_list.Where(x => x.datacenterId == _platform.moid && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                    {
                        PlatformInventoryWorkloadDo.UpdateMCPWorkload(_caasworkload.id, _platform.moid);
                    }
                }
                sw.Stop();

                Logger.log(
                    String.Format("Completed inventory process for {5} - {0} new workloads - {1} updated platform networks - {2} updated workloads - {3} removed workloads = Total Execute Time: {4}",
                    _new_workloads, _updated_platforms, _updated_workloads, _removed_workloads,
                     TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), (_platform.human_vendor + " : " + _platform.moid)
                    ), Logger.Severity.Info);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error in inventory process for {0} {1}", (_platform.human_vendor + " : " + _platform.moid), ex.ToString()), Logger.Severity.Error);
            }
        }

    }
}
