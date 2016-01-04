using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.LocalDatabase;
using CloudMoveyWorkerService.Portal.Types.API;
using CloudMoveyWorkerService.WCF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Utils;

namespace CloudMoveyWorkerService.Portal.Classes
{
    class PlatformInventoryWorker
    {
        CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();

        //Order or sync process
        // 1. update worker information from portal - perf_collection
        // 2. push credentials to portal
        // 3. delete workloads from local DB that is no longer in source platforms
        // 4. push workloads and networks to portal

        public void Start()
        {
            while (true)
            { 

                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

                try
                {
                    

                    Global.event_log.WriteEntry("Staring platform inventory process");

                    //process platform independant items

                    //process platforms
                    List<Platform> _workerplatforms;
                    using (LocalDB db = new LocalDB())
                    {
                        _workerplatforms = db.Platforms.ToList();
                    }
                    MoveyPlatformListType _platformplatforms = _cloud_movey.platform().listplatforms();
                    foreach (var _platform in _workerplatforms)
                    {
                        MoveyPlatformCRUDType _crudplatform = new MoveyPlatformCRUDType();
                        _crudplatform.id = _platform.id;
                        _crudplatform.worker_id = Global.agent_id;
                        _crudplatform.credential_id = _platform.credential_id;
                        _crudplatform.platform_version = _platform.platform_version;
                        _crudplatform.platformtype = (new Vendors()).VendorList.FirstOrDefault(x => x.ID == _platform.vendor).Vendor.Replace(" ", "_").ToLower();
                        _crudplatform.moid = _platform.moid;

                        _crudplatform.platform = _platform.description;
                        if (_platformplatforms.platforms.Exists(x => x.id == _platform.id))
                        {
                            _cloud_movey.platform().updateplatform(_crudplatform);
                            _updated_platforms += 1;
                        }
                        else
                        {
                            _cloud_movey.platform().createplatform(_crudplatform);
                            _new_platforms += 1;
                        }
                    }

                    //process dimension data networks
                    foreach (var _platform in _workerplatforms.Where(x => x.vendor == 0 && x.platform_version == "MCP 2.0"))
                    {
                        UpdateMCPPlatform(_platform);
                    }
                    sw.Stop();

                    Global.event_log.WriteEntry(
                        String.Format("Completed data mirroring process.{2}{0} new platforms.{2}{1} updated platforms.{2}{2}{3} total elapsed time",
                        _new_platforms, _updated_platforms, 
                        Environment.NewLine, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ));
                }
                catch (Exception ex)
                {
                    Global.event_log.WriteEntry(String.Format("Error in mirror task: {0}", ex.ToString()), EventLogEntryType.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }

        public void UpdateMCPPlatform(Platform _platform)
        {
            try
            {
                

                Global.event_log.WriteEntry(String.Format("Started data mirroring process for {0}", (_platform.human_vendor + " : " + _platform.datacenter)));
                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads, _removed_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = _removed_workloads = 0;

                //define object lists
                Credential _credential;
                using (LocalDB db = new LocalDB())
                {
                    List<Credential> _workercredentials = db.Credentials.ToList();
                    _credential = _workercredentials.FirstOrDefault(x => x.id == _platform.credential_id);

                }
                MoveyWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
                MoveyPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
                MoveyPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();


                //populate platform credential object

                //create dimension data mcp object
                DimensionData _caas = new DimensionData(_platform.url, _credential.username, _credential.password);

                //mirror platorm templates for this platform
                MoveyPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();
                OsImagesType _caas_templates = _caas.templates().platformtemplates();

                foreach (OsImageType _caas_template in _caas_templates.osImage.Where(x => x.datacenterId == _platform.moid))
                {
                    MoveyPlatformtemplateCRUDType _moveytemplate = new MoveyPlatformtemplateCRUDType();
                    _moveytemplate.image_type = _caas_template.softwareLabel == null ? "os" : "software";
                    _moveytemplate.image_description = _caas_template.description;
                    _moveytemplate.platform_id = _platform.id;
                    _moveytemplate.image_moid = _caas_template.id;
                    _moveytemplate.image_name = _caas_template.name;
                    _moveytemplate.os_displayname = _caas_template.operatingSystem.displayName;
                    _moveytemplate.os_id = _caas_template.operatingSystem.id;
                    _moveytemplate.os_type = _caas_template.operatingSystem.family;
                    _moveytemplate.platform_moid = _platform.moid;
                    if (_platformtemplates.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                    {
                        _moveytemplate.id = _platformtemplates.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id).id;
                        _cloud_movey.platformtemplate().updateplatformtemplate(_moveytemplate);
                    }
                    else
                    {
                        _cloud_movey.platformtemplate().createplatformtemplate(_moveytemplate);
                    }
                }

                //process customer images
                CustomerImagesType _customer_templates = _caas.templates().customertemplates();
                foreach (var _caas_template in _customer_templates.customerImage.Where(x => x.datacenterId == _platform.moid))
                {
                    MoveyPlatformtemplateCRUDType _moveytemplate = new MoveyPlatformtemplateCRUDType();
                    _moveytemplate.image_type = "os";
                    _moveytemplate.image_description = _caas_template.description;
                    _moveytemplate.image_moid = _caas_template.id;
                    _moveytemplate.platform_id = _platform.id;
                    _moveytemplate.image_name = _caas_template.name;
                    _moveytemplate.os_displayname = _caas_template.operatingSystem.displayName;
                    _moveytemplate.os_id = _caas_template.operatingSystem.id;
                    _moveytemplate.os_type = _caas_template.operatingSystem.family;
                    _moveytemplate.platform_moid = _platform.moid;
                    if (_platformtemplates.platformtemplates.Exists(x => x.image_moid == _caas_template.id))
                    {
                        _moveytemplate.id = _platformtemplates.platformtemplates.FirstOrDefault(x => x.image_moid == _caas_template.id).id;
                        _cloud_movey.platformtemplate().updateplatformtemplate(_moveytemplate);
                    }
                    else
                    {
                        _cloud_movey.platformtemplate().createplatformtemplate(_moveytemplate);
                    }
                }

                //refresh templates
                _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();

                //update localdb platform information
                List<Option> _dcoptions = new List<Option>();
                List<Option> _resourceoptions = new List<Option>();
                _resourceoptions.Add(new Option() { option = "datacenterId", value = _platform.datacenter });

                _dcoptions.Add(new Option() { option = "id", value = _platform.datacenter });
                DatacenterType _dc = ((DatacenterListType)_caas.datacenter().datacenters(_dcoptions)).datacenter[0];
                int workloads, networkdomains, vlans;
                string workloads_md5, networkdomains_md5, vlans_md5;
                workloads = networkdomains = vlans = 0;

                List<ServerType> workload_list = _caas.workloads().list(_resourceoptions).server;
                workloads = workload_list.Count();
                workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(workload_list));
                List<VlanType> vlan_list = _caas.vlans().list(_resourceoptions).vlan;
                vlans = vlan_list.Count();
                vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(vlan_list));
                List<NetworkDomainType> networkdomain_list = _caas.networkdomain().list(_resourceoptions).networkDomain;
                networkdomains = networkdomain_list.Count();
                networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(networkdomain_list));

                using (LocalDB db = new LocalDB())
                {
                    Platform __platform = db.Platforms.Find(_platform.id);
                    __platform.vlan_count = vlans;
                    __platform.workload_count = workloads;
                    __platform.networkdomain_count = networkdomains;
                    __platform.platform_version = _dc.type;

                    __platform.lastupdated = DateTime.Now;
                    __platform.human_vendor = (new Vendors()).VendorList.First(x => x.ID == _platform.vendor).Vendor;
                    __platform.moid = _dc.id;
                    db.SaveChanges();
                }


                List<Option> _domainoptions = new List<Option>();
                _domainoptions.Add(new Option() { option = "datacenterId", value = _platform.datacenter });
                _domainoptions.Add(new Option() { option = "state", value = "NORMAL" });
                foreach (NetworkDomainType _domain in _caas.networkdomain().list(_domainoptions).networkDomain)
                {
                    MoveyPlatformdomainCRUDType _platformdomain = new MoveyPlatformdomainCRUDType();
                    _platformdomain.moid = _domain.id;
                    _platformdomain.domain = _domain.name;
                    _platformdomain.platform_id = _platform.id;
                    if (_currentplatformdomains.platformdomains.Exists(x => x.moid == _domain.id))
                    {
                        _platformdomain.id = _currentplatformdomains.platformdomains.FirstOrDefault(x => x.moid == _domain.id).id;
                        _cloud_movey.platformdomain().updateplatformdomain(_platformdomain);
                        _updated_platformnetworks += 1;
                    }
                    else
                    {
                        _cloud_movey.platformdomain().createplatformdomain(_platformdomain);
                        _new_platformnetworks += 1;
                    }
                    List<Option> _networkoptions = new List<Option>();
                    _networkoptions.Add(new Option() { option = "networkDomainId", value = _domain.id });
                    _networkoptions.Add(new Option() { option = "state", value = "NORMAL" });
                    foreach (VlanType _network in _caas.vlans().list(_networkoptions).vlan)
                    {
                        MoveyPlatformnetworkCRUDType _platformnetwork = new MoveyPlatformnetworkCRUDType();
                        _platformnetwork.moid = _network.id;
                        _platformnetwork.network = _network.name;
                        _platformnetwork.description = _network.description;
                        _platformnetwork.platformdomain_id = _platformdomain.id;
                        _platformnetwork.ipv4subnet = _network.privateIpv4Range.address;
                        _platformnetwork.ipv4netmask = _network.privateIpv4Range.prefixSize;
                        _platformnetwork.ipv6subnet = _network.ipv6Range.address;
                        _platformnetwork.ipv6netmask = _network.ipv6Range.prefixSize;
                        _platformnetwork.networkdomain_moid = _network.networkDomain.id;
                        _platformnetwork.provisioned = true;
                        if (_currentplatformnetworks.platformnetworks.Exists(x => x.moid == _network.id))
                        {
                            _platformnetwork.id = _currentplatformnetworks.platformnetworks.FirstOrDefault(x => x.moid == _network.id).id;
                            _cloud_movey.platformnetwork().updateplatformnetwork(_platformnetwork);
                            _updated_platformnetworks += 1;
                        }
                        else
                        {
                            _cloud_movey.platformnetwork().createplatformnetwork(_platformnetwork);
                            _new_platformnetworks += 1;
                        }
                    }
                }
                //refresh platform network list from portal
                _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();

                //process workloads
                List<Option> _workload_mcp2_options = new List<Option>();
                _workload_mcp2_options.Add(new Option() { option = "datacenterId", value = _platform.datacenter });
                _workload_mcp2_options.Add(new Option() { option = "state", value = "NORMAL" });
                List<ServerType> _caasworkloads = _caas.workloads().list(_workload_mcp2_options).server.ToList();


                //process deleted platform workloads
                using (LocalDB db = new LocalDB())
                {
                    foreach (var _workload in db.Workloads.Where(x => x.platform_id == _platform.id))
                    {
                        if (!_caasworkloads.Any(x => x.id == _workload.moid && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                        {
                            db.Workloads.Remove(db.Workloads.Find(_workload.id));
                            db.SaveChanges();
                            _removed_workloads += 1;
                        }
                    }
                }

                foreach (ServerType _caasworkload in _caasworkloads.Where(x => x.datacenterId == _platform.datacenter && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                {
                    //Pupulate logical volumes for workload
                    List<MoveyWorkloadVolumeType> workloaddisks_parameters = new List<MoveyWorkloadVolumeType>();
                    foreach (ServerTypeDisk _workloaddisk in _caasworkload.disk)
                    {
                        MoveyWorkloadVolumeType _logical_volume = new MoveyWorkloadVolumeType() { moid = _workloaddisk.id, diskindex = _workloaddisk.scsiId, provisioned = true, disksize = _workloaddisk.sizeGb };
                        if (_currentplatformworkloads.workloads.Exists(x => x.volumes.Exists(y => y.moid == _workloaddisk.id)))
                        {
                            _logical_volume.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).volumes.FirstOrDefault(y => y.moid == _workloaddisk.id).id;
                        }
                        workloaddisks_parameters.Add(_logical_volume);
                    }

                    //populate network interfaces for workload
                    List<MoveyWorkloadInterfaceType> workloadinterfaces_parameters = new List<MoveyWorkloadInterfaceType>();
                    MoveyWorkloadInterfaceType _primary_logical_interface = new MoveyWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
                    if (_currentplatformworkloads.workloads.Exists(x => x.interfaces.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
                    {
                        _primary_logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
                    }
                    workloadinterfaces_parameters.Add(_primary_logical_interface);
                    int nic_index = 1;
                    foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                    {
                        MoveyWorkloadInterfaceType _logical_interface = new MoveyWorkloadInterfaceType()
                        {
                            vnic = nic_index,
                            ipassignment = "manual_ip",
                            ipv6address = _caasworkloadinterface.ipv6,
                            ipaddress = _caasworkloadinterface.privateIpv4,
                            moid = _caasworkloadinterface.id,
                            platformnetwork_id = _currentplatformnetworks.platformnetworks.FirstOrDefault(x => x.moid == _caasworkloadinterface.vlanId).id
                        };
                        if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                        {
                            if (_currentplatformworkloads.workloads.Exists((x => x.interfaces.Exists(y => y.moid == _caasworkloadinterface.id))))
                            {
                                _logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkloadinterface.id).id;
                            }
                        }
                        workloadinterfaces_parameters.Add(_logical_interface);
                        nic_index += 1;
                    }

                    //if workload is local, updated the local db record
                    //User might use these servers later...
                    bool _new_workload = true;
                    using (LocalDB db = new LocalDB())
                    {
                        Workload _workload = new Workload();
                        if (db.Workloads.ToList().Exists(x => x.moid == _caasworkload.id))
                        {
                            _new_workload = false;
                            _workload = db.Workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                        }
                        else
                        {
                            //if server already exists in portal, retain GUID for the server to keep other table depedencies intact
                            if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                            {
                                _workload.id = _currentplatformworkloads.workloads.Find(x => x.moid == _caasworkload.id).id;
                            }
                            else
                            {
                                _workload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                            }
                        }

                        _workload.vcpu = _caasworkload.cpu.count;
                        _workload.vcore = _caasworkload.cpu.coresPerSocket;
                        _workload.vmemory = _caasworkload.memoryGb;
                        _workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                        _workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                        _workload.hostname = _caasworkload.name;
                        _workload.moid = _caasworkload.id;
                        _workload.platform_id = _platform.id;
                        _workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                        _workload.osedition = _caasworkload.operatingSystem.displayName;
                        if (_new_workload)
                        {
                            _workload.id = Objects.RamdomGuid();
                            db.Workloads.Add(_workload);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.SaveChanges();
                            if (_workload.enabled == true)
                            {
                                MoveyWorkloadCRUDType _moveyworkload = new MoveyWorkloadCRUDType();
                                Objects.MapObjects(_workload, _moveyworkload);


                                //update workload source template id with portal template id
                                _moveyworkload.platformtemplate_id = _platformtemplates.platformtemplates.Find(x => x.image_moid == _caasworkload.sourceImageId).id;

                                _moveyworkload.workloaddisks_attributes = workloaddisks_parameters;
                                _moveyworkload.workloadinterfaces_attributes = workloadinterfaces_parameters;

                                //Update if the portal has this workload and create if it's new to the portal....
                                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                                {
                                    _moveyworkload.id = _workload.id;
                                    _cloud_movey.workload().updateworkload(_moveyworkload);
                                    _updated_workloads += 1;
                                }
                                else
                                {
                                    _cloud_movey.workload().createworkload(_moveyworkload);
                                    _new_workloads += 1;
                                }
                            }
                        }
                    }
                }
                sw.Stop();

                Global.event_log.WriteEntry(
                    String.Format("Completed data mirroring process for {6}.{5}{0} new workloads.{5}{1} updated platform networks.{5}{2} updated workloads.{5}{3} removed workloads.{5}{5}Total Execute Time: {4}",
                    _new_workloads, _updated_platforms, _updated_workloads, _removed_workloads,
                     TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), Environment.NewLine, (_platform.human_vendor + " : " + _platform.datacenter)
                    ));
            }
            catch(Exception ex)
            {
                Global.event_log.WriteEntry(String.Format("Error in data mirroring process for {0}: {1}", (_platform.human_vendor + " : " + _platform.datacenter), ex.ToString()), EventLogEntryType.Error);
            }
        }
    }
}
