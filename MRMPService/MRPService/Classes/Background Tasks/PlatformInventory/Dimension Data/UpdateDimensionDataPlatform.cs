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
using System.Threading.Tasks;
using DD.CBU.Compute.Api.Contracts.Directory;

namespace MRMPService.PlatformInventory
{
    class PlatformDimensionDataMCP2InventoryDo
    {
        public static async Task UpdateMCPPlatform(MRPPlatformType _platform, bool full = true)
        {

            Logger.log(String.Format("Started inventory process for {0} : {1}", _platform.platformtype, _platform.platformdatacenter.moid), Logger.Severity.Info);
            Stopwatch sw = Stopwatch.StartNew();
            int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
            _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

            //define object lists
            MRPCredentialType _credential = _platform.credential;
            _platform = await MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
            MRPPlatformType _update_platform = new MRPPlatformType() { id = _platform.id };

            List<MRPWorkloadType> _mrp_workloads = new List<MRPWorkloadType>();

            MRPWorkloadListType _paged_workload = await MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id });
            while (_paged_workload.pagination.page_size > 0)
            {
                _mrp_workloads.AddRange(_paged_workload.workloads);
                if (_paged_workload.pagination.next_page > 0)
                {
                    _paged_workload = await MRMPServiceBase._mrmp_api.workload().list_paged_filtered_brief(new MRPWorkloadFilterPagedType() { platform_id = _platform.id, page = _paged_workload.pagination.next_page });
                }
                else
                {
                    break;
                }
            }

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_credential.username, _credential.encrypted_password));
            IAccount _caas_account;
            try
            {
                _caas_account = await CaaS.Login();
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(String.Format("Error connecting to MCP Endpoint {0}", ex.ToString()));
            }

            //first update organization tags
            var _dcs = await CaaS.Account.GetDataCentersWithMaintenanceStatuses();
            var _caas_tags = await CaaS.Tagging.GetTags();

            var _mrmp_org = await MRMPServiceBase._mrmp_api.organization().get();
            MRPOrganizationCRUDType _update_org = new MRPOrganizationCRUDType() { organization = new MRPOrganizationType() { id = MRMPServiceBase.organization_id, organizationtags = _mrmp_org.organizationtags } };

            if (_caas_tags != null)
            {
                foreach (var _caas_tag in _caas_tags)
                {
                    MRPOrganizationTagType _mrp_tag = new MRPOrganizationTagType();
                    if (_update_org.organization.organizationtags.Exists(x => x.tagkeyid == _caas_tag.tagKeyId))
                    {
                        _mrp_tag = _update_org.organization.organizationtags.FirstOrDefault(x => x.tagkeyid == _caas_tag.tagKeyId);
                    }
                    else
                    {
                        _mrp_tag.tagtype = "platform";
                        _update_org.organization.organizationtags.Add(_mrp_tag);
                    }
                    _mrp_tag.tagdisplayreport = _caas_tag.displayOnReport;
                    _mrp_tag.tagkeyid = _caas_tag.tagKeyId;
                    _mrp_tag.tagkeyname = _caas_tag.tagKeyName;
                    _mrp_tag.tagvaluerequired = _caas_tag.valueRequired;
                    _mrp_tag.deleted = false;
                }
            }
            //update organization
            await MRMPServiceBase._mrmp_api.organization().update(_update_org);

            //update datacenters for this platform
            List<DatacenterType> _mcp_datacenters = CaaS.Infrastructure.GetDataCenters().Result.ToList();
            _update_platform.platformdatacenters = _platform.platformdatacenters;
            _update_platform.platformdatacenters.ForEach(x => { x.deleted = true; x.platformclusters.ForEach(y => y.deleted = true); });
            if (_mcp_datacenters != null)
            {
                foreach (DatacenterType _dc in _mcp_datacenters)
                {
                    MRPPlatformdatacenterType _platform_datacenter = new MRPPlatformdatacenterType();
                    if (_platform.platformdatacenters.Exists(x => x.moid == _dc.id))
                    {
                        _platform_datacenter = _platform.platformdatacenters.FirstOrDefault(x => x.moid == _dc.id);
                    }
                    else
                    {
                        _platform.platformdatacenters.Add(_platform_datacenter);
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
                    _platform_datacenter.deleted = false;
                }
            }

            //mirror platorm templates for this platform
            List<OsImageType> _caas_templates = CaaS.ServerManagement.ServerImage.GetOsImages(new ServerOsImageListOptions() { DatacenterId = _platform.platformdatacenter.moid }, new PageableRequest() { PageSize = 250 }).Result.items.ToList();

            //preload all templates and mark them as deleted
            _update_platform.platformtemplates = _platform.platformtemplates;
            _update_platform.platformtemplates.ForEach(x => x.deleted = true);

            foreach (OsImageType _caas_template in _caas_templates)
            {
                MRPPlatformtemplateType _mrp_template = new MRPPlatformtemplateType();
                if (_update_platform.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                {
                    _mrp_template = _update_platform.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id);
                }
                else
                {
                    _update_platform.platformtemplates.Add(_mrp_template);
                }

                _mrp_template.image_type = _caas_template.softwareLabel == null ? "os" : "software";
                _mrp_template.image_description = _caas_template.description;
                _mrp_template.platform_id = _platform.id;
                _mrp_template.image_moid = _caas_template.id;
                _mrp_template.image_name = _caas_template.name;
                _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                _mrp_template.os_id = _caas_template.operatingSystem.id;
                _mrp_template.os_type = _caas_template.operatingSystem.family;
                _mrp_template.platform_moid = _platform.platformdatacenter.moid;
                _mrp_template.deleted = false;
            }

            //process customer images
            PagedResponse<CustomerImageType> _customer_templates = await CaaS.ServerManagement.ServerImage.GetCustomerImages(new ServerCustomerImageListOptions() { DatacenterId = _platform.platformdatacenter.moid }, new PageableRequest() { PageSize = 250 });
            if (_customer_templates.totalCount > 0)
            {
                foreach (var _caas_template in _customer_templates.items)
                {
                    MRPPlatformtemplateType _mrp_template = new MRPPlatformtemplateType();
                    if (_update_platform.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                    {
                        _mrp_template = _update_platform.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id);
                    }
                    else
                    {
                        _update_platform.platformtemplates.Add(_mrp_template);
                    }

                    _mrp_template.image_type = "os";
                    _mrp_template.image_description = _caas_template.description;
                    _mrp_template.image_moid = _caas_template.id;
                    _mrp_template.platform_id = _platform.id;
                    _mrp_template.image_name = _caas_template.name;
                    _mrp_template.os_displayname = _caas_template.operatingSystem.displayName;
                    _mrp_template.os_id = _caas_template.operatingSystem.id;
                    _mrp_template.os_type = _caas_template.operatingSystem.family;
                    _mrp_template.platform_moid = _platform.platformdatacenter.moid;
                    _mrp_template.deleted = false;
                }
            }

            _update_platform.platformdomains = _platform.platformdomains;
            _update_platform.platformdomains.ForEach(x => { x.domainiplists = null; x.domainportlists = null; });
            _update_platform.platformdomains.ForEach(x => x.deleted = true);
            _update_platform.platformdomains.ForEach(x => x.platformnetworks.ForEach(y => y.deleted = true));

            IEnumerable<NetworkDomainType> _caas_networkdomain_list = CaaS.Networking.NetworkDomain.GetNetworkDomains(new NetworkDomainListOptions() { DatacenterId = _platform.platformdatacenter.moid }).Result;
            IEnumerable<VlanType> _caas_vlan_list = CaaS.Networking.Vlan.GetVlans(new VlanListOptions() { DatacenterId = _platform.platformdatacenter.moid }).Result;

            if (_caas_networkdomain_list != null)
            {
                foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                {

                    MRPPlatformdomainType _mrp_domain = new MRPPlatformdomainType() { platformnetworks = new List<MRPPlatformnetworkType>() };
                    if (_update_platform.platformdomains.Exists(x => x.moid == _caas_domain.id))
                    {
                        _mrp_domain = _update_platform.platformdomains.FirstOrDefault(x => x.moid == _caas_domain.id);
                    }
                    else
                    {
                        _update_platform.platformdomains.Add(_mrp_domain);
                    }

                    _mrp_domain.snatip4address = _caas_domain.snatIpv4Address;
                    _mrp_domain.domaintype = _caas_domain.type;
                    _mrp_domain.moid = _caas_domain.id;
                    _mrp_domain.domain = _caas_domain.name;
                    _mrp_domain.platform_id = _platform.id;
                    _mrp_domain.deleted = false;

                    VlanListOptions _vlan_options = new VlanListOptions() { DatacenterId = _platform.platformdatacenter.moid };
                    IEnumerable<VlanType> _vlans = CaaS.Networking.Vlan.GetVlans(_vlan_options).Result;
                    foreach (VlanType _caas_network in _vlans.Where(x => x.networkDomain.id == _caas_domain.id))
                    {
                        MRPPlatformnetworkType _mrp_network = new MRPPlatformnetworkType();
                        if (_mrp_domain.platformnetworks.Any(y => y.moid == _caas_network.id))
                        {
                            _mrp_network = _mrp_domain.platformnetworks.FirstOrDefault(y => y.moid == _caas_network.id);
                        }
                        else
                        {
                            _mrp_domain.platformnetworks.Add(_mrp_network);
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
                        _mrp_network.ipv4gateway = _caas_network.ipv4GatewayAddress;
                        _mrp_network.ipv6gateway = _caas_network.ipv6GatewayAddress;
                        _mrp_network.provisioned = true;
                        _mrp_network.deleted = false;
                    }

                }
            }
            await MRMPServiceBase._mrmp_api.platform().update(_update_platform);

            //refresh platform from portal
            _platform = await MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
            _update_platform = new MRPPlatformType() { id = _platform.id };
            //update IP and Port Lists
            if (_caas_networkdomain_list != null)
            {
                foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                {

                    MRPPlatformdomainType _mrmp_update_domain = new MRPPlatformdomainType();
                    if (_platform.platformdomains.Exists(x => x.moid == _caas_domain.id))
                    {
                        _mrmp_update_domain = _platform.platformdomains.FirstOrDefault(x => x.moid == _caas_domain.id);
                        _mrmp_update_domain.domainiplists.Where(x => x.iptype == "platform").ForEach(x => { x.deleted = true; x.domainiplistaddresses.ForEach(y => y.deleted = true); });
                        _mrmp_update_domain.domainfwrules.Where(x => x.fwtype == "platform").ForEach(x => x.deleted = true);
                        _mrmp_update_domain.domainnatrules.Where(x => x.nattype == "platform").ForEach(x => x.deleted = true);
                        _mrmp_update_domain.domainaffinityrules.Where(x => x.affinitytype == "platform").ForEach(x => x.deleted = true);
                        _mrmp_update_domain.domainportlists.Where(x => x.porttype == "platform").ForEach(x => { x.deleted = true; x.domainportlistports.ForEach(y => y.deleted = true); });
                    }
                    else
                    {
                        _mrmp_update_domain.domainfwrules = new List<MRPDomainFWRuleType>();
                        _mrmp_update_domain.domainiplists = new List<MRPDomainIPType>();
                        _mrmp_update_domain.domainnatrules = new List<MRPDomainNATRuleType>();
                        _mrmp_update_domain.domainportlists = new List<MRPDomainPortType>();
                        _mrmp_update_domain.domainaffinityrules = new List<MRPDomainAffinityRuleType>();
                        _update_platform.platformdomains.Add(_mrmp_update_domain);
                    }

                    var _iplist = await CaaS.Networking.FirewallRule.GetIpAddressLists(Guid.Parse(_caas_domain.id));
                    if (_iplist != null)
                    {
                        foreach (var _ip_entry in _iplist)
                        {
                            MRPDomainIPType _mrmp_domain_ip = new MRPDomainIPType();

                            if (_mrmp_update_domain.domainiplists.Exists(x => x.moid == _ip_entry.id))
                            {
                                _mrmp_domain_ip = _mrmp_update_domain.domainiplists.FirstOrDefault(x => x.moid == _ip_entry.id);
                            }
                            else
                            {
                                _mrmp_domain_ip.iptype = "platform";
                                _mrmp_update_domain.domainiplists.Add(_mrmp_domain_ip);
                            }
                            _mrmp_domain_ip.ipversion = _ip_entry.ipVersion.ToLower();
                            _mrmp_domain_ip.moid = _ip_entry.id;
                            _mrmp_domain_ip.name = _ip_entry.name;
                            _mrmp_domain_ip.description = _ip_entry.description;
                            _mrmp_domain_ip.platformdomain_id = _mrmp_update_domain.id;
                            _mrmp_domain_ip.created_time = _ip_entry.createTime;
                            _mrmp_domain_ip.deleted = false;
                            if (_mrmp_domain_ip.domainiplistaddresses == null)
                            {
                                _mrmp_domain_ip.domainiplistaddresses = new List<MRPIPListAddressType>();
                            }
                            if (_ip_entry.childIpAddressList != null)
                            {
                                foreach (var _childiplist in _ip_entry.childIpAddressList)
                                {
                                    MRPIPListAddressType _mrmp_ip = new MRPIPListAddressType();
                                    if (_mrmp_domain_ip.domainiplistaddresses.Any(x => x.moid == _childiplist.id && x.iptype == "address_list"))
                                    {
                                        _mrmp_ip = _mrmp_domain_ip.domainiplistaddresses.FirstOrDefault(x => x.moid == _childiplist.id && x.iptype == "address_list");
                                    }
                                    else if (_mrmp_domain_ip.domainiplistaddresses.Any(x => x.moid == _childiplist.id))
                                    {
                                        _mrmp_ip.iptype = "address_list";
                                        _mrmp_ip.domainiplist_id = _mrmp_domain_ip.domainiplistaddresses.FirstOrDefault(x => x.moid == _childiplist.id && x.iptype == "address_list").id;
                                        if (_mrmp_domain_ip.domainiplistaddresses == null)
                                        {
                                            _mrmp_domain_ip.domainiplistaddresses = new List<MRPIPListAddressType>();
                                        }
                                        _mrmp_domain_ip.domainiplistaddresses.Add(_mrmp_ip);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var _ip in _ip_entry.ipAddress)
                                {
                                    MRPIPListAddressType _mrmp_ipaddress = new MRPIPListAddressType();

                                    if (_ip.prefixSizeSpecified)
                                    {
                                        if (_mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).Any(x => x.begin_address == _ip.begin && x.prefix_size == _ip.prefixSize))
                                        {
                                            _mrmp_ipaddress = _mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).FirstOrDefault(x => x.begin_address == _ip.begin && x.prefix_size == _ip.prefixSize);
                                        }
                                        else
                                        {
                                            if (_mrmp_domain_ip.domainiplistaddresses == null)
                                            {
                                                _mrmp_domain_ip.domainiplistaddresses = new List<MRPIPListAddressType>();
                                            }
                                            _mrmp_domain_ip.domainiplistaddresses.Add(_mrmp_ipaddress);
                                        }
                                        _mrmp_ipaddress.begin_address = _ip.begin;
                                        _mrmp_ipaddress.ipversion = _ip_entry.ipVersion.ToLower();
                                        _mrmp_ipaddress.prefix_size = _ip.prefixSize;
                                        _mrmp_ipaddress.iptype = "subnet";
                                    }
                                    else if (_ip.end != null && !_ip.prefixSizeSpecified)
                                    {
                                        if (_mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).Any(x => x.begin_address == _ip.begin && x.end_address == _ip.end))
                                        {
                                            _mrmp_ipaddress = _mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).FirstOrDefault(x => x.begin_address == _ip.begin && x.end_address == _ip.end);
                                        }
                                        else
                                        {
                                            if (_mrmp_domain_ip.domainiplistaddresses == null)
                                            {
                                                _mrmp_domain_ip.domainiplistaddresses = new List<MRPIPListAddressType>();
                                            }
                                            _mrmp_domain_ip.domainiplistaddresses.Add(_mrmp_ipaddress);
                                        }
                                        _mrmp_ipaddress.begin_address = _ip.begin;
                                        _mrmp_ipaddress.ipversion = _ip_entry.ipVersion.ToLower();
                                        _mrmp_ipaddress.end_address = _ip.end;
                                        _mrmp_ipaddress.iptype = "range";
                                    }
                                    else
                                    {
                                        if (_mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).Any(x => x.begin_address == _ip.begin))
                                        {
                                            _mrmp_ipaddress = _mrmp_update_domain.domainiplists.Where(x => x.domainiplistaddresses != null).SelectMany(x => x.domainiplistaddresses).FirstOrDefault(x => x.begin_address == _ip.begin);
                                        }
                                        else
                                        {
                                            if (_mrmp_domain_ip.domainiplistaddresses == null)
                                            {
                                                _mrmp_domain_ip.domainiplistaddresses = new List<MRPIPListAddressType>();
                                            }
                                            _mrmp_domain_ip.domainiplistaddresses.Add(_mrmp_ipaddress);
                                        }
                                        _mrmp_ipaddress.begin_address = _ip.begin;
                                        _mrmp_ipaddress.ipversion = _ip_entry.ipVersion.ToLower();
                                        _mrmp_ipaddress.iptype = "address";
                                    }
                                    _mrmp_ipaddress.deleted = false;

                                }
                            }
                        }
                    }
                    var _caas_portlist = await CaaS.Networking.FirewallRule.GetPortLists(Guid.Parse(_caas_domain.id));
                    if (_caas_portlist != null)
                    {
                        foreach (var _port_entry in _caas_portlist)
                        {
                            MRPDomainPortType _mrmp_domain_port = new MRPDomainPortType();
                            if (_mrmp_update_domain.domainportlists.Exists(x => x.moid == _port_entry.id))
                            {
                                _mrmp_domain_port = _mrmp_update_domain.domainportlists.FirstOrDefault(x => x.moid == _port_entry.id && x.porttype == "platform");
                            }
                            else
                            {
                                _mrmp_domain_port.porttype = "platform";
                                if (_mrmp_update_domain.domainportlists == null)
                                {
                                    _mrmp_update_domain.domainportlists = new List<MRPDomainPortType>();
                                }
                                _mrmp_update_domain.domainportlists.Add(_mrmp_domain_port);
                            }

                            _mrmp_domain_port.moid = _port_entry.id;
                            _mrmp_domain_port.name = _port_entry.name;
                            _mrmp_domain_port.description = _port_entry.description;
                            _mrmp_domain_port.platformdomain_id = _mrmp_update_domain.id;
                            _mrmp_domain_port.created_time = _port_entry.createTime;
                            _mrmp_domain_port.deleted = false;
                            if (_mrmp_domain_port.domainportlistports == null)
                            {
                                _mrmp_domain_port.domainportlistports = new List<MRPPortListPortType>();
                            }
                            if (_port_entry.childPortList != null)
                            {
                                foreach (var _childportlist in _port_entry.childPortList)
                                {
                                    MRPPortListPortType _mrmp_port = new MRPPortListPortType();
                                    if (_mrmp_domain_port.domainportlistports.Exists(x => x.moid == _childportlist.id && x.porttype == "port_list"))
                                    {
                                        _mrmp_port = _mrmp_domain_port.domainportlistports.FirstOrDefault(x => x.moid == _childportlist.id && x.porttype == "port_list");
                                    }
                                    else if (_mrmp_domain_port.domainportlistports.Exists(x => x.moid == _childportlist.id))
                                    {
                                        _mrmp_port.porttype = "port_list";
                                        _mrmp_port.domainportlist_id = _mrmp_domain_port.domainportlistports.FirstOrDefault(x => x.moid == _childportlist.id && x.porttype == "port_list").id;
                                        if (_mrmp_domain_port.domainportlistports == null)
                                        {
                                            _mrmp_domain_port.domainportlistports = new List<MRPPortListPortType>();
                                        }
                                        _mrmp_domain_port.domainportlistports.Add(_mrmp_port);


                                    }
                                }
                            }
                            else if (_port_entry.port != null)
                            {
                                foreach (var _port in _port_entry.port)
                                {
                                    MRPPortListPortType _mrmp_port = new MRPPortListPortType();
                                    if (_port.endSpecified)
                                    {
                                        if (_mrmp_domain_port.domainportlistports.Exists(x => x.begin_port == _port.begin && x.end_port == _port.end && x.porttype == "port_range"))
                                        {
                                            _mrmp_port = _mrmp_domain_port.domainportlistports.FirstOrDefault(x => x.begin_port == _port.begin && x.end_port == _port.end && x.porttype == "port_range");
                                        }
                                        else
                                        {
                                            _mrmp_port.porttype = "port_range";
                                            if (_mrmp_domain_port.domainportlistports == null)
                                            {
                                                _mrmp_domain_port.domainportlistports = new List<MRMPAPI.Contracts.MRPPortListPortType>();
                                            }
                                            _mrmp_domain_port.domainportlistports.Add(_mrmp_port);
                                        }
                                        _mrmp_port.begin_port = _port.begin;
                                        _mrmp_port.end_port = _port.end;
                                    }
                                    else
                                    {
                                        if (_mrmp_domain_port.domainportlistports.Exists(x => x.begin_port == _port.begin && x.porttype == "single_port"))
                                        {
                                            _mrmp_port = _mrmp_domain_port.domainportlistports.FirstOrDefault(x => x.begin_port == _port.begin && x.porttype == "single_port");
                                        }
                                        else
                                        {
                                            _mrmp_port.porttype = "single_port";
                                            if (_mrmp_domain_port.domainportlistports == null)
                                            {
                                                _mrmp_domain_port.domainportlistports = new List<MRPPortListPortType>();
                                            }
                                            _mrmp_domain_port.domainportlistports.Add(_mrmp_port);
                                        }
                                        _mrmp_port.begin_port = _port.begin;
                                    }
                                    _mrmp_port.deleted = false;
                                }
                            }
                        }
                    }
                    await MRMPServiceBase._mrmp_api.platform().update(_update_platform);
                }
                _platform = await MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
                _update_platform = new MRPPlatformType() { id = _platform.id, workloads = new List<MRPWorkloadType>() };

                IEnumerable<ServerType> _caas_workload_list = await CaaS.ServerManagement.Server.GetServers(new ServerListOptions() { DatacenterId = _platform.platformdatacenter.moid, State = "NORMAL" });
                //process deleted platform workloads
                foreach (var _workload in _platform.workloads.Where(x => x.workloadtype != "manager"))
                {
                    MRPWorkloadType _mrp_workload = new MRPWorkloadType() { id = _workload.id };
                    if (!_caas_workload_list.Any(x => x.id == _workload.moid))
                    {
                        _mrp_workload.deleted = true;
                        _mrp_workload.enabled = false;
                    }
                    else
                    {
                        _mrp_workload.deleted = false;
                    }
                    _update_platform.workloads.Add(_mrp_workload);
                }

                await MRMPServiceBase._mrmp_api.platform().update(_update_platform);

                _platform = await MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);

                if (_caas_workload_list.Count() > 0)
                {
                    if (full)
                    {
                        Parallel.ForEach(_caas_workload_list, new ParallelOptions { MaxDegreeOfParallelism = MRMPServiceBase.platform_workload_inventory_concurrency }, async (_caasworkload) =>
                        {
                            try
                            {
                                //update lists before we start the workload inventory process
                                await PlatformInventoryWorkloadDo.UpdateMCPWorkload(_caasworkload.id, _platform);
                            }
                            catch (Exception ex)
                            {
                                Logger.log(String.Format("Error collecting inventory information from CaaS workload {0} with error {1}", _caasworkload.name, ex.GetBaseException().Message), Logger.Severity.Error);
                            }
                        });

                    }
                }

                _platform = await MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
                if (_caas_networkdomain_list != null)
                {
                    foreach (NetworkDomainType _caas_domain in _caas_networkdomain_list)
                    {
                        _update_platform = new MRPPlatformType() { id = _platform.id, platformdomains = _platform.platformdomains.Where(x => x.moid == _caas_domain.id).ToList() };
                        MRPPlatformdomainType _mrmp_update_domain = new MRPPlatformdomainType();
                        if (_update_platform.platformdomains.Exists(x => x.moid == _caas_domain.id))
                        {
                            _mrmp_update_domain = _update_platform.platformdomains.FirstOrDefault(x => x.moid == _caas_domain.id);
                            _mrmp_update_domain.domainiplists.Where(x => x.iptype == "platform").ForEach(x => { x.deleted = true; x.domainiplistaddresses.ForEach(y => y.deleted = true); });
                            _mrmp_update_domain.domainfwrules.Where(x => x.fwtype == "platform").ForEach(x => x.deleted = true);
                            _mrmp_update_domain.domainnatrules.Where(x => x.nattype == "platform").ForEach(x => x.deleted = true);
                            _mrmp_update_domain.domainaffinityrules.Where(x => x.affinitytype == "platform").ForEach(x => x.deleted = true);
                            _mrmp_update_domain.domainportlists.Where(x => x.porttype == "platform").ForEach(x => { x.deleted = true; x.domainportlistports.ForEach(y => y.deleted = true); });
                        }
                        else
                        {
                            _mrmp_update_domain.domainfwrules = new List<MRPDomainFWRuleType>();
                            _mrmp_update_domain.domainiplists = new List<MRPDomainIPType>();
                            _mrmp_update_domain.domainnatrules = new List<MRPDomainNATRuleType>();
                            _mrmp_update_domain.domainportlists = new List<MRPDomainPortType>();
                            _mrmp_update_domain.domainaffinityrules = new List<MRPDomainAffinityRuleType>();
                            _update_platform.platformdomains.Add(_mrmp_update_domain);
                        }

                        var _firewall_rules = await CaaS.Networking.FirewallRule.GetFirewallRules(new FirewallRuleListOptions() { NetworkDomainId = Guid.Parse(_caas_domain.id) });
                        var _nat_rules = await CaaS.Networking.Nat.GetNatRules(Guid.Parse(_caas_domain.id));
                        var _affinity_rules = await CaaS.ServerManagement.AntiAffinityRule.GetAntiAffinityRulesForNetworkDomain(Guid.Parse(_caas_domain.id));

                        if (_affinity_rules != null)
                        {
                            foreach (var _affinity_rule in _affinity_rules)
                            {
                                MRPDomainAffinityRuleType _mrp_affinityrule = new MRPDomainAffinityRuleType();
                                if (_mrmp_update_domain.domainaffinityrules.Exists(x => x.moid == _affinity_rule.id))
                                {
                                    _mrp_affinityrule = _mrmp_update_domain.domainaffinityrules.FirstOrDefault(x => x.moid == _affinity_rule.id);
                                }
                                else
                                {

                                    _mrmp_update_domain.domainaffinityrules.Add(_mrp_affinityrule);
                                }
                                _mrp_affinityrule.deleted = false;
                                _mrp_affinityrule.moid = _affinity_rule.id;
                                _mrp_affinityrule.workload1_id = _platform.workloads.FirstOrDefault(x => x.moid == _affinity_rule.serverSummary[0].id).id;
                                _mrp_affinityrule.workload2_id = _platform.workloads.FirstOrDefault(x => x.moid == _affinity_rule.serverSummary[1].id).id;
                            }
                        }

                        if (_nat_rules != null)
                        {
                            foreach (var _natrule in _nat_rules)
                            {
                                MRPDomainNATRuleType _mrmpnatrule = new MRPDomainNATRuleType();
                                if (_mrmp_update_domain.domainnatrules.Exists(x => x.moid == _natrule.id))
                                {
                                    _mrmpnatrule = _mrmp_update_domain.domainnatrules.FirstOrDefault(x => x.moid == _natrule.id);
                                }
                                else
                                {
                                    _mrmpnatrule.nattype = "platform";
                                    _mrmp_update_domain.domainnatrules.Add(_mrmpnatrule);
                                }
                                _mrmpnatrule.deleted = false;
                                _mrmpnatrule.moid = _natrule.id;
                                _mrmpnatrule.platformdomain_id = _mrmp_update_domain.id;
                                _mrmpnatrule.rulename = String.Format("{0} NAT Rule", _natrule.internalIp);
                                _mrmpnatrule.internal_ip = _natrule.internalIp;
                                _mrmpnatrule.external_ip = _natrule.externalIp;
                                if (_platform.workloads.Exists(x => x.iplist.Contains(_natrule.internalIp)))
                                {
                                    _mrmpnatrule.workload_id = _platform.workloads.FirstOrDefault(x => x.iplist.Contains(_natrule.internalIp)).id;
                                }
                            }
                        }

                        if (_firewall_rules != null)
                        {
                            //loop firewall rules
                            foreach (var _firewallrule in _firewall_rules.Where(x => x.ruleType != "DEFAULT_RULE"))
                            {
                                MRPDomainFWRuleType _mrmpfirewallrule = new MRPDomainFWRuleType();

                                if (_mrmp_update_domain.domainfwrules.Any(y => y.moid == _firewallrule.id))
                                {
                                    _mrmpfirewallrule = _mrmp_update_domain.domainfwrules.FirstOrDefault(y => y.moid == _firewallrule.id);
                                }
                                else
                                {
                                    _mrmpfirewallrule.fwtype = "platform";
                                    _mrmp_update_domain.domainfwrules.Add(_mrmpfirewallrule);
                                }

                                _mrmpfirewallrule.deleted = false;
                                _mrmpfirewallrule.action = _firewallrule.action.ToUpper();
                                _mrmpfirewallrule.ipversion = _firewallrule.ipVersion.ToUpper();
                                // _mrmpfirewallrule.placement = _firewallrule.
                                _mrmpfirewallrule.enabled = _firewallrule.enabled;
                                _mrmpfirewallrule.platformdomain_id = _platform.platformdomains.FirstOrDefault(x => x.moid == _firewallrule.networkDomainId).id;
                                _mrmpfirewallrule.protocol = _firewallrule.protocol;
                                _mrmpfirewallrule.rulename = _firewallrule.name;
                                _mrmpfirewallrule.moid = _firewallrule.id;

                                //source ip
                                if (_firewallrule.source.IpAddress != null)
                                {
                                    if (_firewallrule.source.IpAddress.prefixSizeSpecified) //This is a subnet
                                    {
                                        _mrmpfirewallrule.source_ip_type = "subnet";
                                        _mrmpfirewallrule.source_subnet = _firewallrule.source.IpAddress.address;
                                        _mrmpfirewallrule.source_subnet_prefix = _firewallrule.source.IpAddress.prefixSize;
                                    }
                                    else
                                    {
                                        _mrmpfirewallrule.source_ip_type = "address";
                                        _mrmpfirewallrule.source_ip = _firewallrule.source.IpAddress.address;
                                    }
                                }
                                else if (_firewallrule.source.IpAddressList != null)
                                {
                                    _mrmpfirewallrule.source_ip_type = "address_list";
                                    if (_mrmp_update_domain.domainiplists != null)
                                    {
                                        _mrmpfirewallrule.source_domainiplist_id = _mrmp_update_domain.domainiplists.FirstOrDefault(x => x.moid == _firewallrule.source.IpAddressList.id).id;
                                    }
                                }

                                //source port
                                if (_firewallrule.source.PortRange != null)
                                {
                                    _mrmpfirewallrule.source_port_type = "single_port";
                                    _mrmpfirewallrule.source_begin_port = _firewallrule.source.PortRange.begin;
                                    if (_firewallrule.source.PortRange.endSpecified)
                                    {
                                        _mrmpfirewallrule.source_port_type = "port_range";
                                        _mrmpfirewallrule.source_end_port = _firewallrule.source.PortRange.end;
                                    }
                                }
                                else if (_firewallrule.source.PortList != null)
                                {
                                    _mrmpfirewallrule.source_port_type = "port_list";
                                    if (_mrmp_update_domain.domainportlists != null)
                                    {
                                        _mrmpfirewallrule.source_domainportlist_id = _mrmp_update_domain.domainiplists.FirstOrDefault(x => x.moid == _firewallrule.source.IpAddressList.id).id;
                                    }
                                }



                                //target ip
                                if (_firewallrule.destination.IpAddress != null)
                                {
                                    if (_firewallrule.destination.IpAddress.prefixSizeSpecified) //This is a subnet
                                    {
                                        _mrmpfirewallrule.target_ip_type = "subnet";
                                        _mrmpfirewallrule.target_subnet = _firewallrule.destination.IpAddress.address;
                                        _mrmpfirewallrule.target_subnet_prefix = _firewallrule.destination.IpAddress.prefixSize;
                                    }
                                    else
                                    {
                                        _mrmpfirewallrule.target_ip_type = "address";
                                        _mrmpfirewallrule.target_ip = _firewallrule.destination.IpAddress.address;
                                    }
                                }
                                else if (_firewallrule.destination.IpAddressList != null)
                                {
                                    _mrmpfirewallrule.target_ip_type = "address_list";
                                    if (_mrmp_update_domain.domainiplists != null)
                                    {
                                        _mrmpfirewallrule.target_domainiplist_id = _mrmp_update_domain.domainiplists.FirstOrDefault(x => x.moid == _firewallrule.destination.IpAddressList.id).id;
                                    }
                                }

                                //target port
                                if (_firewallrule.destination.PortRange != null)
                                {
                                    _mrmpfirewallrule.target_port_type = "single_port";
                                    _mrmpfirewallrule.target_begin_port = _firewallrule.destination.PortRange.begin;
                                    if (_firewallrule.destination.PortRange.endSpecified)
                                    {
                                        _mrmpfirewallrule.target_port_type = "port_range";
                                        _mrmpfirewallrule.target_end_port = _firewallrule.destination.PortRange.end;
                                    }
                                }
                                else if (_firewallrule.destination.PortList != null)
                                {
                                    _mrmpfirewallrule.target_port_type = "port_list";
                                    if (_mrmp_update_domain.domainportlists != null)
                                    {
                                        _mrmpfirewallrule.target_domainportlist_id = _mrmp_update_domain.domainiplists.FirstOrDefault(x => x.moid == _firewallrule.destination.IpAddressList.id).id;
                                    }
                                }

                            }
                        }
                        await MRMPServiceBase._mrmp_api.platform().update(_update_platform);

                    }
                }


                sw.Stop();

                Logger.log(
                    String.Format("Completed inventory process for {5} - {0} new workloads - {1} updated platform networks - {2} updated workloads - {3} removed workloads = Total Execute Time: {4}",
                    _new_workloads, _updated_platforms, _updated_workloads, _removed_workloads,
                     TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), (_platform.platformtype + " : " + _platform.platformdatacenter.moid)
                    ), Logger.Severity.Info);
            }

        }
    }
}
