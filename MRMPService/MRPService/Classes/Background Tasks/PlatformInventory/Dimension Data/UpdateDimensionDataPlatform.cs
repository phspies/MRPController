using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Contracts;
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
    class PlatformDimensionDataMCP2InventoryDo
    {
        public static async void UpdateMCPPlatform(MRPPlatformType _platform, bool full = true)
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
                //update datacenters for this platform
                List<DatacenterType> _mcp_datacenters = CaaS.Infrastructure.GetDataCenters().Result.ToList();
                _update_platform.platformdatacenters_attributes = _platform.platformdatacenters_attributes;
                _update_platform.platformdatacenters_attributes.ForEach(x => { x.deleted = true; x.platformclusters_attributes.ForEach(y => y.deleted = true); });
                if (_mcp_datacenters != null)
                {
                    foreach (DatacenterType _dc in _mcp_datacenters)
                    {
                        MRPPlatformdatacenterType _platform_datacenter = new MRPPlatformdatacenterType();
                        if (_platform.platformdatacenters_attributes.Exists(x => x.moid == _dc.id))
                        {
                            _platform_datacenter = _platform.platformdatacenters_attributes.FirstOrDefault(x => x.moid == _dc.id);
                        }
                        else
                        {
                            _platform.platformdatacenters_attributes.Add(_platform_datacenter);
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
                    }
                }

                //mirror platorm templates for this platform
                List<OsImageType> _caas_templates = CaaS.ServerManagement.ServerImage.GetOsImages(new ServerOsImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result.items.ToList();

                //preload all templates and mark them as deleted
                _update_platform.platformtemplates_attributes = _platform.platformtemplates_attributes;
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

                _update_platform.platformdomains_attributes = _platform.platformdomains_attributes;
                _update_platform.platformdomains_attributes.ForEach(x => { x.domainiplists_attributes = null; x.domainportlists_attributes = null; });
                _update_platform.platformdomains_attributes.ForEach(x => x.deleted = true);
                _update_platform.platformdomains_attributes.ForEach(x => x.platformnetworks_attributes.ForEach(y => y.deleted = true));

                IEnumerable<NetworkDomainType> _caas_networkdomain_list = CaaS.Networking.NetworkDomain.GetNetworkDomains(new NetworkDomainListOptions() { DatacenterId = _platform.moid }).Result;

                IEnumerable<VlanType> _caas_vlan_list = CaaS.Networking.Vlan.GetVlans(new VlanListOptions() { DatacenterId = _platform.moid }).Result;

                if (_caas_networkdomain_list != null)
                {
                    foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                    {
                        if (_platform.platformdomains_attributes.Exists(x => x.moid == _caas_domain.id))
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
                        }
                    }
                }
                //nullify all networks in domains where domain was deleted
                _update_platform.platformdomains_attributes.Where(x => x.deleted == true).ForEach(x => x.platformnetworks_attributes = null);

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
                            PlatformInventoryWorkloadDo.UpdateMCPWorkload(_caasworkload.id, _platform);
                        }
                    }
                }
                //refresh platform from portal
                _platform = _mrp_api_endpoint.platform().get_by_id(_platform.id);
                _update_platform = new MRPPlatformType() { id = _platform.id };
                //update IP and Port Lists
                if (_caas_networkdomain_list != null)
                {
                    foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                    {

                        MRPPlatformdomainType _mrmp_update_domain = _platform.platformdomains_attributes.FirstOrDefault(x => x.moid == _caas_domain.id);
                        _update_platform.platformdomains_attributes = new List<MRPPlatformdomainType>();
                        _update_platform.platformdomains_attributes.Add(_mrmp_update_domain);

                        _mrmp_update_domain.domainfwrules_attributes.ForEach(x => x.deleted = true);
                        _mrmp_update_domain.domainiplists_attributes.ForEach(x => { x.deleted = true; x.domainiplistaddresses_attributes.ForEach(y => y.deleted = true); });
                        _mrmp_update_domain.domainnatrules_attributes.ForEach(x => x.deleted = true);
                        _mrmp_update_domain.domainportlists_attributes.ForEach(x => { x.deleted = true; x.domainportlistports_attributes.ForEach(y => y.deleted = true); });

                        var _iplist = await CaaS.Networking.FirewallRule.GetIpAddressLists(Guid.Parse(_caas_domain.id));
                        if (_iplist != null)
                        {
                            foreach (var _ip_entry in _iplist)
                            {
                                MRPDomainIPType _mrmp_domain_ip = new MRPDomainIPType();

                                if (_mrmp_update_domain.domainiplists_attributes.Exists(x => x.moid == _ip_entry.id))
                                {
                                    _mrmp_domain_ip = _mrmp_update_domain.domainiplists_attributes.FirstOrDefault(x => x.moid == _ip_entry.id);
                                }
                                else
                                {
                                    _mrmp_update_domain.domainiplists_attributes.Add(_mrmp_domain_ip);
                                }
                                _mrmp_domain_ip.moid = _ip_entry.id;
                                _mrmp_domain_ip.iptype = "platform";
                                _mrmp_domain_ip.name = _ip_entry.name;
                                _mrmp_domain_ip.description = _ip_entry.description;
                                _mrmp_domain_ip.platformdomain_id = _mrmp_update_domain.id;
                                _mrmp_domain_ip.created_time = _ip_entry.createTime;
                                _mrmp_domain_ip.deleted = false;

                                foreach (var _ip in _ip_entry.ipAddress)
                                {
                                    MRPIPListAddressType _mrmp_ipaddress = new MRPIPListAddressType();
                                    if (_mrmp_update_domain.domainiplists_attributes.SelectMany(x => x.domainiplistaddresses_attributes).Any(x => x.begin_address == _ip.begin && x.end_address == _ip.end && x.prefix_size == _ip.prefixSize))
                                    {
                                        _mrmp_ipaddress.id = _mrmp_update_domain.domainiplists_attributes.SelectMany(x => x.domainiplistaddresses_attributes).FirstOrDefault(x => x.begin_address == _ip.begin && x.end_address == _ip.end && x.prefix_size == _ip.prefixSize).id;
                                    }
                                    else
                                    {
                                        _mrmp_domain_ip.domainiplistaddresses_attributes.Add(_mrmp_ipaddress);
                                    }
                                    _mrmp_ipaddress.begin_address = _ip.begin;
                                    _mrmp_ipaddress.end_address = _ip.end;
                                    _mrmp_ipaddress.prefix_size = _ip.prefixSize;
                                    _mrmp_ipaddress.deleted = false;
                                }
                            }
                        }
                        var _caas_portlist = await CaaS.Networking.FirewallRule.GetPortLists(Guid.Parse(_caas_domain.id));
                        if (_caas_portlist != null)
                        {
                            foreach (var _port_entry in _caas_portlist)
                            {
                                MRPDomainPortType _mrmp_domain_port = new MRPDomainPortType();
                                if (_mrmp_update_domain.domainportlists_attributes.Exists(x => x.moid == _port_entry.id))
                                {
                                    _mrmp_domain_port = _mrmp_update_domain.domainportlists_attributes.FirstOrDefault(x => x.moid == _port_entry.id);
                                }
                                else
                                {
                                    _mrmp_update_domain.domainportlists_attributes.Add(_mrmp_domain_port);
                                }

                                _mrmp_domain_port.moid = _port_entry.id;
                                _mrmp_domain_port.name = _port_entry.name;
                                _mrmp_domain_port.description = _port_entry.description;
                                _mrmp_domain_port.platformdomain_id = _mrmp_update_domain.id;
                                _mrmp_domain_port.porttype = "platform";
                                _mrmp_domain_port.created_time = _port_entry.createTime;
                                _mrmp_domain_port.deleted = false;

                                foreach (var _port in _port_entry.port)
                                {
                                    MRPPortListPortType _mrmp_port = new MRPPortListPortType();

                                    if (_mrmp_update_domain.domainportlists_attributes.SelectMany(x => x.domainportlistports_attributes).Any(x => x.begin_port == _port.begin && x.end_port == _port.end))
                                    {
                                        _mrmp_port = _mrmp_update_domain.domainportlists_attributes.SelectMany(x => x.domainportlistports_attributes).FirstOrDefault(x => x.begin_port == _port.begin && x.end_port == _port.end);
                                    }
                                    else
                                    {
                                        _mrmp_update_domain.domainportlists_attributes.Add(_mrmp_domain_port);
                                    }
                                    _mrmp_port.begin_port = _port.begin;
                                    _mrmp_port.end_port = _port.end;
                                    _mrmp_port.deleted = false;
                                }
                            }
                        }

                        //update platform object
                        _mrp_api_endpoint.platform().update(_update_platform);

                        //populate update objects
                        _platform = _mrp_api_endpoint.platform().get_by_id(_platform.id);
                        _update_platform = new MRPPlatformType() { id = _platform.id };
                        _mrmp_update_domain = _platform.platformdomains_attributes.FirstOrDefault(x => x.moid == _caas_domain.id);
                        _update_platform.platformdomains_attributes = new List<MRPPlatformdomainType>();
                        _update_platform.platformdomains_attributes.Add(_mrmp_update_domain);
                        _update_platform.platformdomains_attributes.ForEach(x => x.domainfwrules_attributes.ForEach(y => y.deleted = true));

                        var _firewall_filter = new FirewallRuleListOptions();
                        _firewall_filter.NetworkDomainId = Guid.Parse(_caas_domain.id);
                        var _firewall_rules = await CaaS.Networking.FirewallRule.GetFirewallRules(_firewall_filter);


                        _update_platform.platformdomains_attributes.ForEach(x => x.domainnatrules_attributes.ForEach(y => y.deleted = true));
                        var _nat_rules = await CaaS.Networking.Nat.GetNatRules(Guid.Parse(_caas_domain.id));

                        if (_nat_rules != null)
                        {
                            foreach (var _natrule in _nat_rules)
                            {
                                MRPDomainNATRuleType _mrmpnatrule = new MRPDomainNATRuleType();
                                if (_mrmp_update_domain.domainnatrules_attributes.Exists(x => x.moid == _natrule.id))
                                {
                                    _mrmpnatrule = _mrmp_update_domain.domainnatrules_attributes.FirstOrDefault(x => x.moid == _natrule.id);
                                }
                                else
                                {
                                    _mrmp_update_domain.domainnatrules_attributes.Add(_mrmpnatrule);
                                }
                                _mrmpnatrule.deleted = false;
                                _mrmpnatrule.moid = _natrule.id;
                                _mrmpnatrule.platformdomain_id = _mrmp_update_domain.id;
                                _mrmpnatrule.rulename = String.Format("{0} NAT Rule", _natrule.internalIp);
                                _mrmpnatrule.internal_ip = _natrule.internalIp;
                                _mrmpnatrule.external_ip = _natrule.externalIp;
                                if (_platform.workloads_attributes.Exists(x => x.iplist.Contains(_natrule.internalIp)))
                                {
                                    _mrmpnatrule.workload_id = _platform.workloads_attributes.FirstOrDefault(x => x.iplist.Contains(_natrule.internalIp)).id;
                                }
                            }

                            if (_firewall_rules != null)
                            {
                                //loop firewall rules
                                foreach (var _firewallrule in _firewall_rules.Where(x => x.ruleType != "DEFAULT_RULE"))
                                {
                                    MRPDomainFWRuleType _mrmpfirewallrule = new MRPDomainFWRuleType();

                                    if (_mrmp_update_domain.domainfwrules_attributes.Any(y => y.moid == _firewallrule.id))
                                    {
                                        _mrmpfirewallrule.id = _mrmp_update_domain.domainfwrules_attributes.FirstOrDefault(y => y.moid == _firewallrule.id).id;
                                    }
                                    else
                                    {
                                        _mrmp_update_domain.domainfwrules_attributes.Add(_mrmpfirewallrule);
                                    }

                                    _mrmpfirewallrule.deleted = false;
                                    _mrmpfirewallrule.action = _firewallrule.action;
                                    _mrmpfirewallrule.ipversion = _firewallrule.ipVersion;
                                    // _mrmpfirewallrule.placement = _firewallrule.
                                    _mrmpfirewallrule.enabled = _firewallrule.enabled;
                                    _mrmpfirewallrule.platformdomain_id = _platform.platformdomains_attributes.FirstOrDefault(x => x.moid == _firewallrule.networkDomainId).id;
                                    _mrmpfirewallrule.protocol = _firewallrule.protocol;
                                    _mrmpfirewallrule.rulename = _firewallrule.name;
                                    _mrmpfirewallrule.moid = _firewallrule.id;
                                    if (_firewallrule.source.IpAddress != null)
                                    {
                                        _mrmpfirewallrule.source_ips = _firewallrule.source.IpAddress.address;
                                    }
                                    if (_firewallrule.destination.IpAddress != null)
                                    {
                                        _mrmpfirewallrule.target_ips = _firewallrule.destination.IpAddress.address;
                                    }
                                    //source rules
                                    if (_firewallrule.source.IpAddress != null)
                                    {
                                        _mrmpfirewallrule.source_ips = _firewallrule.source.IpAddress.address;
                                        _mrmpfirewallrule.source_ips_prefix = _firewallrule.source.IpAddress.prefixSize;
                                    }
                                    else if (_firewallrule.source.IpAddressList != null)
                                    {
                                        if (_mrmp_update_domain.domainiplists_attributes != null)
                                        {
                                            _mrmpfirewallrule.source_domainiplist_id = _mrmp_update_domain.domainiplists_attributes.FirstOrDefault(x => x.moid == _firewallrule.source.IpAddressList.id).id;
                                        }
                                    }
                                    if (_firewallrule.source.PortRange != null)
                                    {
                                        _mrmpfirewallrule.source_begin_port = _firewallrule.source.PortRange.begin;
                                        _mrmpfirewallrule.source_end_port = _firewallrule.source.PortRange.end;
                                    }
                                    else if (_firewallrule.source.PortList != null)
                                    {
                                        if (_mrmp_update_domain.domainportlists_attributes != null)
                                        {
                                            _mrmpfirewallrule.source_portlist_id = _mrmp_update_domain.domainiplists_attributes.FirstOrDefault(x => x.moid == _firewallrule.source.IpAddressList.id).id;
                                        }
                                    }

                                    //target rules
                                    if (_firewallrule.destination.IpAddress != null)
                                    {
                                        _mrmpfirewallrule.target_ips = _firewallrule.destination.IpAddress.address;
                                        _mrmpfirewallrule.target_ips_prefix = _firewallrule.destination.IpAddress.prefixSize;
                                    }
                                    else if (_firewallrule.destination.IpAddressList != null)
                                    {
                                        if (_mrmp_update_domain.domainiplists_attributes != null)
                                        {
                                            _mrmpfirewallrule.target_domainiplist_id = _mrmp_update_domain.domainiplists_attributes.FirstOrDefault(x => x.moid == _firewallrule.destination.IpAddressList.id).id;
                                        }
                                    }
                                    if (_firewallrule.destination.PortRange != null)
                                    {
                                        _mrmpfirewallrule.target_begin_port = _firewallrule.destination.PortRange.begin;
                                        _mrmpfirewallrule.target_end_port = _firewallrule.destination.PortRange.end;
                                    }
                                    else if (_firewallrule.destination.PortList != null)
                                    {
                                        if (_mrmp_update_domain.domainportlists_attributes != null)
                                        {
                                            _mrmpfirewallrule.target_portlist_id = _mrmp_update_domain.domainiplists_attributes.FirstOrDefault(x => x.moid == _firewallrule.destination.IpAddressList.id).id;
                                        }

                                    }
                                }
                                _mrp_api_endpoint.platform().update(_update_platform);
                            }

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
