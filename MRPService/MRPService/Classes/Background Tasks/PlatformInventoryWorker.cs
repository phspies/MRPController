using MRPService.MRPService.Log;
using MRPService.LocalDatabase;
using MRPService.API.Types.API;
using MRPService.WCF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Requests.Server20;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using DD.CBU.Compute.Api.Contracts.General;
using MRPService.Utilities;

namespace MRPService.API.Classes
{
    class PlatformInventoryWorker
    {
        ApiClient _cloud_movey = new ApiClient();

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
   
                    Logger.log("Staring platform inventory process", Logger.Severity.Info);

                    //process platform independant items

                    //process platforms
                    List<Platform> _workerplatforms;
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        _workerplatforms = db.Platforms.ToList();
                    }
                    MRPPlatformListType _platformplatforms = _cloud_movey.platform().listplatforms();
                    foreach (var _platform in _workerplatforms)
                    {
                        MRPPlatformCRUDType _crudplatform = new MRPPlatformCRUDType();
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

                    Logger.log(
                        String.Format("Completed data mirroring process.{0} new platforms, {1} updated platforms = total elapsed time: {2}",
                        _new_platforms, _updated_platforms,TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ), Logger.Severity.Info);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in mirror task: {0}", ex.ToString()), Logger.Severity.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }

        public void UpdateMCPPlatform(Platform _platform)
        {
            try
            {


                Logger.log(String.Format("Started data mirroring process for {0}", (_platform.human_vendor + " : " + _platform.datacenter)), Logger.Severity.Info);
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
                MRPWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
                MRPPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
                MRPPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();


                //populate platform credential object

                //create dimension data mcp object
                ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_credential.username, _credential.password));
                CaaS.Login().Wait();


                //mirror platorm templates for this platform
                MRPPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();
                List<OsImageType> _caas_templates = CaaS.ServerManagement.ServerImage.GetOsImages(new ServerOsImageListOptions() {DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result.items.ToList();

                foreach (OsImageType _caas_template in _caas_templates)
                {
                    MRPPlatformtemplateCRUDType _moveytemplate = new MRPPlatformtemplateCRUDType();
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
                PagedResponse<CustomerImageType> _customer_templates = CaaS.ServerManagement.ServerImage.GetCustomerImages(new ServerCustomerImageListOptions() { DatacenterId = _platform.moid }, new PageableRequest() { PageSize = 250 }).Result;
                if (_customer_templates.totalCount > 0)
                {
                    foreach (var _caas_template in _customer_templates.items)
                    {
                        MRPPlatformtemplateCRUDType _moveytemplate = new MRPPlatformtemplateCRUDType();
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
                }

                //refresh templates
                _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();

                //update localdb platform information
                List<NetworkDomainType> networkdomain_list = CaaS.Networking.NetworkDomain.GetNetworkDomains(new DD.CBU.Compute.Api.Contracts.Requests.Network20.NetworkDomainListOptions() { DatacenterId = _platform.moid }).Result.ToList();
                List<ServerType> workload_list = CaaS.ServerManagement.Server.GetServers(new DD.CBU.Compute.Api.Contracts.Requests.Server20.ServerListOptions() { DatacenterId = _platform.moid, State = "NORMAL" }).Result.ToList();
                List<VlanType> vlan_list = CaaS.Networking.Vlan.GetVlans(new DD.CBU.Compute.Api.Contracts.Requests.Network20.VlanListOptions() { DatacenterId = _platform.moid }).Result.ToList();
                DatacenterType _dc = CaaS.Infrastructure.GetDataCenters(new PageableRequest() { PageSize = 250 }, new DataCenterListOptions() { Id = _platform.moid }).Result.ToList().FirstOrDefault();

                int workloads, networkdomains, vlans;
                string workloads_md5, networkdomains_md5, vlans_md5;
                workloads = networkdomains = vlans = 0;

                workloads = workload_list.Count();
                workloads_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(workload_list));
                vlans = vlan_list.Count();
                vlans_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(vlan_list));
                networkdomains = networkdomain_list.Count();
                networkdomains_md5 = ObjectExtensions.GetMD5Hash(JsonConvert.SerializeObject(networkdomain_list));

                using (MRPDatabase db = new MRPDatabase())
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

                foreach (NetworkDomainType _domain in networkdomain_list)
                {
                    MRPPlatformdomainCRUDType _platformdomain = new MRPPlatformdomainCRUDType();
                    _platformdomain.platformnetworks_attributes = new List<MRPPlatformnetworkCRUDType>();
                    _platformdomain.moid = _domain.id;
                    _platformdomain.domain = _domain.name;
                    _platformdomain.platform_id = _platform.id;

                    foreach (VlanType _network in CaaS.Networking.Vlan.GetVlans(new DD.CBU.Compute.Api.Contracts.Requests.Network20.VlanListOptions() { NetworkDomainId = Guid.Parse(_domain.id) }).Result.ToList())
                    {
                        MRPPlatformnetworkCRUDType _platformnetwork = new MRPPlatformnetworkCRUDType();
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
                            _updated_platformnetworks += 1;
                        }
                        else
                        {
                            _new_platformnetworks += 1;
                        }
                        _platformdomain.platformnetworks_attributes.Add(_platformnetwork);
                    }
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
                }
                //refresh platform network list from portal
                _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();

                //process workloads

                //process deleted platform workloads
                using (MRPDatabase db = new MRPDatabase())
                {
                    foreach (var _workload in db.Workloads.Where(x => x.platform_id == _platform.id))
                    {
                        if (!workload_list.Any(x => x.id == _workload.moid && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                        {
                            db.Workloads.Remove(db.Workloads.Find(_workload.id));
                            db.SaveChanges();
                            _removed_workloads += 1;
                        }
                    }
                }

                foreach (ServerType _caasworkload in workload_list.Where(x => x.datacenterId == _platform.datacenter && x.operatingSystem.family.ToUpper() == "WINDOWS"))
                {
                    UpdateMCPWorkload(_caasworkload.id, _platform.moid);
                }
                sw.Stop();

                Logger.log(
                    String.Format("Completed data mirroring process for {5}.{0} new workloads.{1} updated platform networks.{2} updated workloads.{3} removed workloads. = Total Execute Time: {4}",
                    _new_workloads, _updated_platforms, _updated_workloads, _removed_workloads,
                     TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds), (_platform.human_vendor + " : " + _platform.datacenter)
                    ), Logger.Severity.Info);
            }
            catch(Exception ex)
            {
                Logger.log(String.Format("Error in data mirroring process for {0}: {1}", (_platform.human_vendor + " : " + _platform.datacenter), ex.ToString()), Logger.Severity.Error);
            }
        }
        public static void UpdateMCPWorkload(string _workload_moid, string _platform_moid)
        {

            ApiClient _cloud_movey = new ApiClient();


            Platform _platform;
            Credential _platform_credential;

            using (MRPDatabase db = new MRPDatabase())
            {
                _platform = db.Platforms.FirstOrDefault(x => x.moid == _platform_moid);
                _platform_credential = db.Credentials.FirstOrDefault(x => x.id == _platform.credential_id);

            }

            //create dimension data mcp object
            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credential.username, _platform_credential.password));
            CaaS.Login().Wait();

            //get workload object from MCP
            ServerType _caasworkload = CaaS.ServerManagement.Server.GetServer(Guid.Parse(_workload_moid)).Result;

            //Retrieve portal objects
            MRPWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
            MRPPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
            MRPPlatformdomainListType _currentplatformdomains = _cloud_movey.platformdomain().listplatformdomains();
            MRPPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();



            //Pupulate logical volumes for workload
            List<MRPWorkloadDiskType> workloaddisks_parameters = new List<MRPWorkloadDiskType>();
            foreach (ServerTypeDisk _workloaddisk in _caasworkload.disk)
            {
                MRPWorkloadDiskType _virtual_disk = new MRPWorkloadDiskType()
                {
                    moid = _workloaddisk.id,
                    diskindex = _workloaddisk.scsiId,
                    provisioned = true,
                    disksize = _workloaddisk.sizeGb,
                    _destroy = false
                };
                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id && x.disks.Exists(y => y.moid == _workloaddisk.id)))
                {
                    _virtual_disk.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).disks.FirstOrDefault(y => y.moid == _workloaddisk.id).id;
                }
                workloaddisks_parameters.Add(_virtual_disk);
            }

            //populate network interfaces for workload
            List<MRPWorkloadInterfaceType> workloadinterfaces_parameters = new List<MRPWorkloadInterfaceType>();
            MRPWorkloadInterfaceType _primary_logical_interface = new MRPWorkloadInterfaceType() { vnic = 0, ipassignment = "manual_ip", ipv6address = _caasworkload.networkInfo.primaryNic.ipv6, ipaddress = _caasworkload.networkInfo.primaryNic.privateIpv4, moid = _caasworkload.networkInfo.primaryNic.id };
            if (_currentplatformworkloads.workloads.Exists(x => x.interfaces.Exists(y => x.moid == _caasworkload.id && y.moid == _caasworkload.networkInfo.primaryNic.id)))
            {
                _primary_logical_interface.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).interfaces.FirstOrDefault(y => y.moid == _caasworkload.networkInfo.primaryNic.id).id;
            }
            workloadinterfaces_parameters.Add(_primary_logical_interface);
            int nic_index = 1;
            if (_caasworkload.networkInfo.additionalNic != null)
            {
                foreach (NicType _caasworkloadinterface in _caasworkload.networkInfo.additionalNic)
                {
                    MRPWorkloadInterfaceType _logical_interface = new MRPWorkloadInterfaceType()
                    {
                        vnic = nic_index,
                        ipassignment = "manual_ip",
                        ipv6address = _caasworkloadinterface.ipv6,
                        ipaddress = _caasworkloadinterface.privateIpv4,
                        moid = _caasworkloadinterface.id,
                        _destroy = false,
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
            }

            //if workload is local, updated the local db record
            //User might use these servers later...
            bool _new_workload_flag = true;
            using (MRPDatabase db = new MRPDatabase())
            {
                Workload _new_workload = new Workload();
                if (db.Workloads.ToList().Exists(x => x.moid == _caasworkload.id))
                {
                    _new_workload_flag = false;
                    _new_workload = db.Workloads.FirstOrDefault(x => x.moid == _caasworkload.id);
                }
                else
                {
                    //if server already exists in portal, retain GUID for the server to keep other table depedencies intact
                    if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                    {
                        _new_workload.id = _currentplatformworkloads.workloads.Find(x => x.moid == _caasworkload.id).id;
                    }
                    else
                    {
                        _new_workload.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    }
                }

                _new_workload.vcpu = Convert.ToUInt16(_caasworkload.cpu.count);
                _new_workload.vcore = Convert.ToUInt16(_caasworkload.cpu.coresPerSocket);
                _new_workload.vmemory = Convert.ToUInt16(_caasworkload.memoryGb);
                _new_workload.iplist = string.Join(",", _caasworkload.networkInfo.primaryNic.ipv6, _caasworkload.networkInfo.primaryNic.privateIpv4);
                _new_workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                _new_workload.hostname = _caasworkload.name;
                _new_workload.moid = _caasworkload.id;
                _new_workload.platform_id = _platform.id;
                _new_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                _new_workload.osedition = _caasworkload.operatingSystem.displayName;
                if (_new_workload_flag)
                {
                    _new_workload.id = Objects.RamdomGuid();
                    db.Workloads.Add(_new_workload);
                    db.SaveChanges();
                }
                else
                {
                    db.SaveChanges();
                    if (_new_workload.enabled == true)
                    {
                        MRPWorkloadCRUDType _moveyworkload = new MRPWorkloadCRUDType();
                        Objects.MapObjects(_new_workload, _moveyworkload);


                        //update workload source template id with portal template id
                        _moveyworkload.platformtemplate_id = _platformtemplates.platformtemplates.Find(x => x.image_moid == _caasworkload.sourceImageId).id;

                        _moveyworkload.workloaddisks_attributes = workloaddisks_parameters;
                        _moveyworkload.workloadinterfaces_attributes = workloadinterfaces_parameters;

                        _moveyworkload.provisioned = true;


                        //Update if the portal has this workload and create if it's new to the portal....
                        if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                        {
                            _moveyworkload.id = _new_workload.id;
                            _cloud_movey.workload().updateworkload(_moveyworkload);
                        }
                        else
                        {
                            _cloud_movey.workload().createworkload(_moveyworkload);
                        }
                    }
                }
            }

        }
    }
}
