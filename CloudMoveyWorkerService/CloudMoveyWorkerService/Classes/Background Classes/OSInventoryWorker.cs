﻿using CloudMoveyWorkerService.CaaS;
using CloudMoveyWorkerService.Database;
using CloudMoveyWorkerService.Portal.Types.API;
using CloudMoveyWorkerService.WCF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CloudMoveyWorkerService.Portal.Classes
{
    class OSInventoryWorker
    {
        CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
        public void Start()
        {
            CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();

            while (true)
            {
                Stopwatch sw = Stopwatch.StartNew();
                int _new_credentials, _new_platforms, _new_platformnetworks, _new_workloads, _updated_credentials, _updated_platforms, _updated_platformnetworks, _updated_workloads;
                _new_credentials = _new_platforms = _new_platformnetworks = _new_workloads = _updated_credentials = _updated_platformnetworks = _updated_platforms = _updated_workloads = 0;

                try
                {
                    Global.event_log.WriteEntry("Staring operating system inventory process");
                    //process credentials
                    MoveyCredentialListType _platformcredentials = _cloud_movey.credential().listcredentials();
                    foreach (var _credential in LocalData.get_as_list<Credential>())
                    {
                        MoveyCredentialCRUDType _crudcredential = new MoveyCredentialCRUDType();
                        _crudcredential.id = _credential.id;
                        _crudcredential.description = _credential.description;
                        _crudcredential.credential_type = _credential.credential_type;
                        if (_platformcredentials.credentials.Exists(x => x.id == _credential.id))
                        {
                            _cloud_movey.credential().updatecredential(_crudcredential);
                            _updated_credentials += 1;
                        }
                        else
                        {
                            _cloud_movey.credential().createcredential(_crudcredential);
                            _new_credentials += 1;
                        }
                    }

                    //process platforns
                    MoveyPlatformListType _platformplatforms = _cloud_movey.platform().listplatforms();
                    foreach (var _platform in LocalData.get_as_list<Platform>())
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
                    foreach (var _platform in LocalData.get_as_list<Platform>().Where(x => x.vendor == 0 && x.platform_version == "MCP 2.0"))
                    {
                        if (_platform.platform_version == "MCP 2.0")
                        {
                            MoveyPlatformnetworkListType _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();
                            var _credential = LocalData.get_as_list<Credential>().FirstOrDefault(x => x.id == _platform.credential_id);
                            DimensionData _caas = new DimensionData(_platform.url, _credential.username, _credential.password);

                            //mirror platorm templates for this platform
                            MirrorPlatformTemplates(_credential, _caas, _platform);

                            List<Option> _options = new List<Option>();
                            _options.Add(new Option() { option = "datacenterId", value = _platform.datacenter });
                            _options.Add(new Option() { option = "state", value = "NORMAL" });
                            foreach (VlanType _network in _caas.vlans().list(_options).vlan)
                            {
                                MoveyPlatformnetworkCRUDType _platformnetwork = new MoveyPlatformnetworkCRUDType();
                                _platformnetwork.moid = _network.id;
                                _platformnetwork.network = _network.name;
                                _platformnetwork.description = _network.description;
                                _platformnetwork.platform_id = _platform.id;
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
                            //refresh platform network list from portal
                            _currentplatformnetworks = _cloud_movey.platformnetwork().listplatformnetworks();

                            //process workloads
                            MoveyWorkloadListType _currentplatformworkloads = _cloud_movey.workload().listworkloads();
                            List<Option> _workload_mcp2_options = new List<Option>();
                            _workload_mcp2_options.Add(new Option() { option = "datacenterId", value = _platform.datacenter });
                            _workload_mcp2_options.Add(new Option() { option = "state", value = "NORMAL" });
                            List<ServerType> _caasworkloads = _caas.workloads().list(_workload_mcp2_options).server.ToList();
                            foreach (ServerType _caasworkload in _caasworkloads.Where(x => x.datacenterId == _platform.datacenter))
                            {
                                MoveyWorkloadCRUDType _moveyworkload = new MoveyWorkloadCRUDType();
                                _moveyworkload.hostname = _caasworkload.name;
                                _moveyworkload.moid = _caasworkload.id;
                                _moveyworkload.vcpu = (_caasworkload.cpu.coresPerSocket * _caasworkload.cpu.count) as int?;
                                _moveyworkload.vmemory = _caasworkload.memoryGb as int?;
                                _moveyworkload.platform_id = _platform.id;
                                _moveyworkload.enabled = true;
                                _moveyworkload.ostype = _caasworkload.operatingSystem.family.ToLower();
                                _moveyworkload.osedition = _caasworkload.operatingSystem.displayName;

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

                                _moveyworkload.workloaddisks_attributes = workloaddisks_parameters;
                                _moveyworkload.workloadinterfaces_attributes = workloadinterfaces_parameters;

                                if (_currentplatformworkloads.workloads.Exists(x => x.moid == _caasworkload.id))
                                {
                                    _moveyworkload.id = _currentplatformworkloads.workloads.FirstOrDefault(x => x.moid == _caasworkload.id).id;
                                    _cloud_movey.workload().updateworkload(_moveyworkload);
                                    _updated_workloads += 1;
                                }
                                else
                                {
                                    _cloud_movey.workload().createworkload(_moveyworkload);
                                    _new_workloads += 1;
                                }

                                //Update database with workload information
                                Workload _workload = new Workload();
                                _workload.cpu_count = (_caasworkload.cpu.coresPerSocket * _caasworkload.cpu.count) as int?;
                                _workload.memory_count = _caasworkload.memoryGb as int?;
                                _workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                                _workload.hostname = _caasworkload.name;
                                _workload.moid = _caasworkload.id;
                                _workload.platform_id = _platform.id;
                                _workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                                _workload.osedition = _caasworkload.operatingSystem.displayName;
                                if (LocalData.get_as_list<Workload>().Count(x => x.moid == _caasworkload.id) == 0)
                                {
                                    new CloudMoveyService().AddWorkload(_workload);
                                }
                                else
                                {
                                    Workload _database_workload = LocalData.get_record<Workload>(_caasworkload.id);
                                    _database_workload.cpu_count = (_caasworkload.cpu.coresPerSocket * _caasworkload.cpu.count) as int?;
                                    _database_workload.memory_count = _caasworkload.memoryGb as int?;
                                    _database_workload.storage_count = _caasworkload.disk.Sum(x => x.sizeGb);
                                    _database_workload.hostname = _caasworkload.name;
                                    _database_workload.moid = _caasworkload.id;
                                    _database_workload.platform_id = _platform.id;
                                    _database_workload.ostype = _caasworkload.operatingSystem.family.ToLower();
                                    _database_workload.osedition = _caasworkload.operatingSystem.displayName;
                                    LocalData.update_record<Workload>(_database_workload);
                                }
                            }
                        }
                    }
                    sw.Stop();

                    Global.event_log.WriteEntry(
                        String.Format("Completed data mirroring process.{6}{0} new credentials.{6}{1} new platforms.{6}{7} new platform networks.{6}{2} new workloads.{6}{3} updated credentials.{6}{4} updated platforms.{6}{8} updated platform networks.{6}{5} updated workloads.{6}{6}Total Execute Time: {9}",
                        _new_credentials, _new_platforms, _new_workloads, _updated_credentials, _updated_platforms, _updated_workloads,
                        Environment.NewLine, _new_platformnetworks, _updated_platformnetworks, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ));
                }
                catch (Exception ex)
                {
                    Global.event_log.WriteEntry(String.Format("Error in mirror task: {0}", ex.ToString()), EventLogEntryType.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
        private void MirrorPlatformTemplates(Credential _credential, DimensionData _caas, Platform _platform)
        {
            MoveyPlatformtemplateListType _platformtemplates = _cloud_movey.platformtemplate().listplatformtemplates();

            //process platform images
            OsImagesType _caas_templates = _caas.templates().platformtemplates();
            foreach (var _caas_template in _caas_templates.osImage.Where(x => x.datacenterId == _platform.moid))
            {
                MoveyPlatformtemplateCRUDType _moveytemplate = new MoveyPlatformtemplateCRUDType();
                _moveytemplate.image_type = _caas_template.softwareLabel.Count() == 0 ? "os" : "software";
                _moveytemplate.image_description = _caas_template.description;
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
    }
}
