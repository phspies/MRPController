using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPAPI.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using MRMPService.Utilities;
using MRMPService.PlatformInventory;
using DD.CBU.Compute.Api.Contracts.General;
using MRMPService.MRPService.Classes.Utilities;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        public static void ProvisionVM(String _task_id, MRPPlatformType _platform, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, float _start_progress, float _end_progress, bool _os_customization = false)
        {
            //Get workload object from portal to perform updates once provisioned
            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(_task_id, String.Format("Starting provisioning process"), ReportProgress.Progress(_start_progress, _end_progress, 1));
                
                //update target workload with updated iplist and moid information
                MRPWorkloadType _temp_workload = _mrp_api.workload().get_by_id(_target_workload.id);

                _target_workload.moid = _temp_workload.moid;
                _target_workload.iplist = _temp_workload.iplist;
            }

            if (_target_workload.credential == null)
            {
                throw new System.ArgumentException("Cannot find standalone credential for workload deployment");
            }

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.encrypted_password));
            CaaS.Login().Wait();

            try
            {
                //test to see if the workload exists in the platform
                if (!String.IsNullOrEmpty(_target_workload.moid))
                {
                    ServerType _caas_server = null;
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Workload contains a platform management id: {0}", _target_workload.moid), ReportProgress.Progress(_start_progress, _end_progress, 10));
                    }
                    try
                    {
                        _caas_server = CaaS.ServerManagement.Server.GetServer(new Guid(_target_workload.moid)).Result;
                    }
                    catch (Exception ex)
                    {

                    }
                    if (_caas_server != null)
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Reusing available workload which was deployed {0}", _caas_server.createTime), ReportProgress.Progress(_start_progress, _end_progress, 11));
                            _mrp_api.task().progress(_task_id, String.Format("Not rerunning Operating System Customization. Please make sure the server the network and storage components are configured correctly."), ReportProgress.Progress(_start_progress, _end_progress, 12));
                            return;
                        }
                    }
                    else
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().progress(_task_id, String.Format("Workload {0} does not exist in platform. New workload will be provisioned", _target_workload.moid), ReportProgress.Progress(_start_progress, _end_progress, 11));
                        }
                    }
                }

                //deploy workload in platform
                DataCenterListOptions _dc_options = new DataCenterListOptions();
                _dc_options.Id = _platform.moid;
                DatacenterType _dc = CaaS.Infrastructure.GetDataCenters(null, _dc_options).Result.FirstOrDefault();

                DeployServerType _vm = new DeployServerType();

                List<DeployServerTypeDisk> _disks = new List<DeployServerTypeDisk>();

                //Set Tier for first disk being deployed
                MRPWorkloadVolumeType _first_disk = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.diskindex == 0);
                _disks.Add(new DeployServerTypeDisk() { scsiId = 0, speed = _first_disk.platformstoragetier.shortname });

                _vm.name = _target_workload.hostname;
                _vm.description = String.Format("{0} MRMP : {1}", DateTime.UtcNow, _protectiongroup.service);
                DeployServerTypeNetwork _network = new DeployServerTypeNetwork();
                _network.Item = _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid;
                _network.ItemElementName = NetworkIdOrPrivateIpv4ChoiceType.networkId;
                _network.networkId = _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid;
                DeployServerTypeNetworkInfo _networkInfo = new DeployServerTypeNetworkInfo();
                _networkInfo.networkDomainId = _target_workload.workloadinterfaces_attributes[0].platformnetwork.networkdomain_moid;
                _networkInfo.primaryNic = new NewNicType()
                {
                    ItemElementName = _target_workload.workloadinterfaces_attributes[0].ipassignment == "auto_ip" ? PrivateIpv4OrVlanIdChoiceType.vlanId : PrivateIpv4OrVlanIdChoiceType.privateIpv4,
                    vlanId = _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid != null ? _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid : null,
                    privateIpv4 = _target_workload.workloadinterfaces_attributes[0].ipassignment != "auto_ip" ? _target_workload.workloadinterfaces_attributes[0].ipaddress : null
                };

                _vm.network = _network;
                _vm.networkInfo = _networkInfo;
                _vm.imageId = _target_workload.platformtemplate.image_moid;
                _vm.cpu = new DeployServerTypeCpu() { count = Convert.ToUInt16(_target_workload.vcpu), countSpecified = true, coresPerSocket = Convert.ToUInt16(_target_workload.vcore), coresPerSocketSpecified = true };
                _vm.memoryGb = Convert.ToUInt16(_target_workload.vmemory);
                _vm.start = false;
                _vm.disk = _disks.ToArray();
                _vm.administratorPassword = _target_workload.credential.encrypted_password;
                _vm.primaryDns = String.IsNullOrEmpty(_target_workload.primary_dns) ? null : _target_workload.primary_dns;
                _vm.secondaryDns = String.IsNullOrEmpty(_target_workload.secondary_dns) ? null : _target_workload.secondary_dns;
                _vm.microsoftTimeZone = (_target_workload.ostype == "windows" ? _target_workload.timezone : null);
                Logger.log(String.Format("Attempting the creation of [{0}] in [{1}]: {2}", _vm.name, _dc.displayName, JsonConvert.SerializeObject(_vm)), Logger.Severity.Debug);

                //create virtual server
                ResponseType _status = new ResponseType();
                int _deploy_retries = 3;
                while (true)
                {
                    try
                    {
                        _status = CaaS.ServerManagement.Server.DeployServer(_vm).Result;
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (--_deploy_retries == 0)
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";

                                Logger.log(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)), Logger.Severity.Error);
                                throw new Exception(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)));
                            }
                        }
                        else Thread.Sleep(5000);
                    }
                }
                //create newly created server in local database
                var serverInfo = _status.info.Single(info => info.name == "serverId");
                Guid _newvm_platform_guid = Guid.Parse(serverInfo.value);

                //track progress of server creation and report updates

                if (_status.responseCode == "IN_PROGRESS")
                {
                    ServerType deployedServer = null;
                    {
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                    }
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("{0} provisioning started in {1} ({2})", _vm.name, _dc.displayName, _dc.id), ReportProgress.Progress(_start_progress, _end_progress, 20));
                    }
                    int _progress_base = 0;
                    while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                    {
                        if (deployedServer.state.Contains("FAILED"))
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                                _mrp_api.task().progress(_task_id, String.Format("Provisioning Failed: {0}. Cleaning failed server", deployedServer.progress.failureReason), ReportProgress.Progress(_start_progress, _end_progress, 45));
                                _mrp_api.task().progress(_task_id, "Attempting to redeploy server", ReportProgress.Progress(_start_progress, _end_progress, 46));
                            }
                            var result = CaaS.ServerManagement.Server.CleanServer(_newvm_platform_guid).Result;

                            //redeploy the server
                            _deploy_retries = 3;
                            while (true)
                            {
                                try
                                {
                                    _status = CaaS.ServerManagement.Server.DeployServer(_vm).Result;
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    if (--_deploy_retries == 0)
                                    {
                                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                        {
                                            String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";

                                            Logger.log(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)), Logger.Severity.Error);
                                            throw new Exception(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)));
                                        }
                                    }
                                    else Thread.Sleep(5000);
                                }
                            }

                            serverInfo = _status.info.Single(info => info.name == "serverId");
                            _newvm_platform_guid = Guid.Parse(serverInfo.value);
                            deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;

                            _progress_base = 15;
                        }
                        if (deployedServer.progress != null)
                        {
                            if (deployedServer.progress.step != null)
                            {
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(_task_id, String.Format("Provisioning step: {0}", deployedServer.progress.step.name), ReportProgress.Progress(_start_progress, _end_progress, _progress_base + 30 + deployedServer.progress.step.number));
                                }
                            }
                        }
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (int _disk_index in _target_workload.workloadvolumes_attributes.OrderBy(x => x.diskindex).Select(x => x.diskindex).Distinct())
                    {
                        //increase disk by 5GB
                        long _disk_size = _target_workload.workloadvolumes_attributes.Where(x => x.diskindex == _disk_index).Sum(x => x.volumesize) + 1;

                        MRPPlatformStorageTierType _disk_tier = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.diskindex == _disk_index).platformstoragetier;
                        if (deployedServer.disk.ToList().Exists(x => x.scsiId == _disk_index))
                        {
                            if (deployedServer.disk.ToList().FirstOrDefault(x => x.scsiId == _disk_index).sizeGb < _disk_size)
                            {
                                String _disk_guid = deployedServer.disk.ToList().Find(x => x.scsiId == _disk_index).id;
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(_task_id, String.Format("Extending storage: {0} : {1}GB", _disk_index, _disk_size), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                                }
                                Status _create_status = CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(deployedServer.id, _disk_guid, _disk_size.ToString()).Result;
                            }
                        }
                        else
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                                _mrp_api.task().progress(_task_id, String.Format("Adding storage: {0} : {1}GB on {2}", _disk_index, _disk_size, _disk_tier.storagetier), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                            }
                            Status _create_status = CaaS.ServerManagementLegacy.Server.AddServerDisk(deployedServer.id, _disk_size.ToString(), _disk_tier.shortname).Result;
                        }
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                        {
                            deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Workload
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Power on workload"), ReportProgress.Progress(_start_progress, _end_progress, 70));
                    }
                    ResponseType _start_server = CaaS.ServerManagement.Server.StartServer(_newvm_platform_guid).Result;
                    deployedServer = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                    while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                    {
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        Thread.Sleep(5000);
                    }
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Workload powered on"), ReportProgress.Progress(_start_progress, _end_progress, 71));
                    }
                    deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;

                    //give workload some time to become available
                    Thread.Sleep(new TimeSpan(0, 0, 30));

                    //Get updated workload object from portal and update the moid,credential_id,provisioned on the portal
                    MRPWorkloadType _update_workload = new MRPWorkloadType();
                    _update_workload.id = _target_workload.id;
                    _update_workload.moid = deployedServer.id;
                    _update_workload.hostname = deployedServer.name;
                    _update_workload.provisioned = true;
                    _update_workload.deleted = false;
                    _update_workload.enabled = true;
                    _update_workload.iplist = String.Join(",", deployedServer.networkInfo.primaryNic.ipv6, deployedServer.networkInfo.primaryNic.privateIpv4);

                    //update first network interface with the newly provisioned server's information
                    _update_workload.workloadinterfaces_attributes = new List<MRPWorkloadInterfaceType>();
                    var _interface = new MRPWorkloadInterfaceType();
                    _interface.id = _target_workload.workloadinterfaces_attributes.FirstOrDefault(x => x.vnic == 0).id;
                    _interface.moid = deployedServer.networkInfo.primaryNic.id;
                    _interface.ipaddress = deployedServer.networkInfo.primaryNic.privateIpv4;
                    _interface.ipassignment = "manual_ip";
                    _interface.ipv6address = deployedServer.networkInfo.primaryNic.ipv6;
                    _update_workload.workloadinterfaces_attributes.Add(_interface);

                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.workload().updateworkload(_update_workload);
                        _target_workload = _mrp_api.workload().get_by_id(_target_workload.id);
                    }
                    //update Platform inventory for server
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Updating platform information for {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 80));                     
                    }
                    PlatformInventoryWorkloadDo.UpdateMCPWorkload(_newvm_platform_guid.ToString(), _platform);

                    //run OS customization code
                    if (_os_customization)
                    {
                        switch (deployedServer.operatingSystem.family)
                        {
                            case "UNIX":
                                LinuxCustomization(_task_id, _platform, _target_workload, _protectiongroup, ReportProgress.Progress(_start_progress, _end_progress, 85), ReportProgress.Progress(_start_progress, _end_progress, 90));
                                //update OS information or newly provisioned server
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(_task_id, String.Format("Updating operating system information for {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 92));
                                }
                                WorkloadInventory.WorkloadInventoryLinuxDo(_target_workload);
                                break;
                            case "WINDOWS":
                                WindowsCustomization(_task_id, _platform, _target_workload, _protectiongroup, ReportProgress.Progress(_start_progress, _end_progress, 85), ReportProgress.Progress(_start_progress, _end_progress, 90));
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(_task_id, String.Format("Updating operating system information for {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 92));
                                }
                                WorkloadInventory.WorkloadInventoryWindowsDo(_target_workload);
                                break;
                        }
                    }

                    //log the success
                    Logger.log(String.Format("Successfully provisioned VM [{0}] in [{1}]: {2}", deployedServer.name, _dc.displayName, JsonConvert.SerializeObject(deployedServer)), Logger.Severity.Debug);
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(_task_id, String.Format("Successfully provisioned VM [{0}] in [{1}]", deployedServer.name, _dc.displayName), ReportProgress.Progress(_start_progress, _end_progress, 95));
                    }
                }
                else
                {
                    Logger.log(String.Format("Failed to create target virtual machine: {0}", _status.error), Logger.Severity.Error);
                    throw new Exception(String.Format("Failed to create target virtual machine: {0}", _status.error));
                }
            }
            catch (Exception e)
            {
                Logger.log(e.ToString(), Logger.Severity.Error);
                throw new Exception(e.Message);
            }
        }
    }
}
