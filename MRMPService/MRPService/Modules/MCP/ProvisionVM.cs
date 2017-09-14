using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI.Classes;
using MRMPService.Modules.MRMPPortal.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using MRMPService.Utilities;
using DD.CBU.Compute.Api.Contracts.General;
using MRMPService.MRPService.Classes.Utilities;
using System.Threading.Tasks;
using MRMPService.Scheduler.PlatformInventory.DimensionData;
using MRMPService.MRPService.Modules.MCP;

namespace MRMPService.Modules.MCP
{
    public partial class MCP_Platform
    {
        public static void ProvisionVM(MRPTaskType _task, MRPPlatformType _platform, MRMPWorkloadBaseType _target_workload, MRPProtectiongroupType _protectiongroup, float _start_progress, float _end_progress, bool _os_customization = false)
        {
            _task.progress(String.Format("Starting provisioning process"), ReportProgress.Progress(_start_progress, _end_progress, 1));
            MRMPWorkloadBaseType _temp_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);
            _platform = MRMPServiceBase._mrmp_api.platform().get_by_id(_platform.id);
            _target_workload.moid = _temp_workload.moid;
            _target_workload.iplist = _temp_workload.iplist;

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.decrypted_password));
            CaaS.Login().Wait();

            //test to see if the workload exists in the platform
            if (!String.IsNullOrEmpty(_target_workload.moid))
            {
                ServerType _caas_server = null;
                _task.progress(String.Format("Workload contains a platform management id: {0}", _target_workload.moid), ReportProgress.Progress(_start_progress, _end_progress, 10));
                try
                {
                    _caas_server = CaaS.ServerManagement.Server.GetServer(new Guid(_target_workload.moid)).Result;
                }
                catch (Exception)
                {

                }
                if (_caas_server != null)
                {

                    if (_caas_server.started)
                    {
                        _task.progress(String.Format("Reusing available workload which was deployed {0}", _caas_server.createTime), ReportProgress.Progress(_start_progress, _end_progress, 11));
                        if (_os_customization)
                        {
                            _task.progress(String.Format("Not rerunning Operating System Customization. Please make sure the server the network and storage components are configured correctly."), ReportProgress.Progress(_start_progress, _end_progress, 12));
                        }
                        return;
                    }
                    else
                    {
                        _task.progress(String.Format("Workload {0} is offline. Starting workload.", _target_workload.moid), ReportProgress.Progress(_start_progress, _end_progress, 11));
                        CaaS.ServerManagement.Server.StartServer(new Guid(_target_workload.moid)).Wait();
                        return;
                    }
                }
                else
                {
                    _task.progress(String.Format("Workload {0} does not exist in platform. New workload will be provisioned", _target_workload.moid), ReportProgress.Progress(_start_progress, _end_progress, 11));
                }
            }

            //deploy workload in platform
            DataCenterListOptions _dc_options = new DataCenterListOptions();
            _dc_options.Id = _platform.platformdatacenter.moid;
            DatacenterType _dc = CaaS.Infrastructure.GetDataCenters(null, _dc_options).Result.FirstOrDefault();
            DeployServerType _vm = new DeployServerType();
            List<DeployServerTypeDisk> _disks = new List<DeployServerTypeDisk>();

            if (_os_customization)
            {
                var _first_disk = _target_workload.workloadvolumes.FirstOrDefault(x => x.diskindex == 0);
                _disks.Add(new DeployServerTypeDisk() { scsiId = 0, speed = _first_disk.platformstoragetier_id });

            }
            else
            {
                var _first_disk = _target_workload.workloaddisks.FirstOrDefault(x => x.diskindex == 0);
                _disks.Add(new DeployServerTypeDisk() { scsiId = 0, speed = _first_disk.platformstoragetier_id });
            }

            _vm.name = _target_workload.hostname;
            _vm.description = String.Format("{0} MRMP : {1}", DateTime.UtcNow, _protectiongroup.group);
            DeployServerTypeNetwork _network = new DeployServerTypeNetwork();
            _network.Item = _target_workload.workloadinterfaces[0].platformnetwork.moid;
            _network.ItemElementName = NetworkIdOrPrivateIpv4ChoiceType.networkId;
            _network.networkId = _target_workload.workloadinterfaces[0].platformnetwork.moid;
            DeployServerTypeNetworkInfo _networkInfo = new DeployServerTypeNetworkInfo();
            _networkInfo.networkDomainId = _target_workload.workloadinterfaces[0].platformnetwork.networkdomain_moid;
            _networkInfo.primaryNic = new NewNicType()
            {
                ItemElementName = _target_workload.workloadinterfaces[0].ipassignment == "auto_ip" ? PrivateIpv4OrVlanIdChoiceType.vlanId : PrivateIpv4OrVlanIdChoiceType.privateIpv4,
                vlanId = _target_workload.workloadinterfaces[0].platformnetwork.moid != null ? _target_workload.workloadinterfaces[0].platformnetwork.moid : null,
                privateIpv4 = _target_workload.workloadinterfaces[0].ipassignment != "auto_ip" ? _target_workload.workloadinterfaces[0].ipaddress : null
            };

            _vm.network = _network;
            _vm.networkInfo = _networkInfo;
            _vm.imageId = _target_workload.platformtemplate.image_moid;
            _vm.cpu = new DeployServerTypeCpu() { count = Convert.ToUInt16(_target_workload.vcpu), countSpecified = true, coresPerSocket = Convert.ToUInt16(_target_workload.vcore), coresPerSocketSpecified = true };
            _vm.memoryGb = Convert.ToUInt16(_target_workload.vmemory);
            _vm.start = false;
            _vm.disk = _disks.ToArray();
            _vm.administratorPassword = _os_customization ? _target_workload.GetCredentials().decrypted_password : "dummy_password";
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
                        String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";

                        Logger.log(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)), Logger.Severity.Error);
                        throw new Exception(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)));
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

                _task.progress(String.Format("{0} provisioning started in {1} ({2})", _vm.name, _dc.displayName, _dc.id), ReportProgress.Progress(_start_progress, _end_progress, 20));
                int _progress_base = 0;
                while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                {
                    if (deployedServer.state.Contains("FAILED"))
                    {

                        _task.progress(String.Format("Provisioning Failed: {0}. Cleaning failed server", deployedServer.progress.failureReason), ReportProgress.Progress(_start_progress, _end_progress, 45));
                        _task.progress("Attempting to redeploy server", ReportProgress.Progress(_start_progress, _end_progress, 46));
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
                                    Logger.log(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)), Logger.Severity.Error);
                                    throw new Exception(String.Format("Error submitting workload creation task after 3 tries: {0}", ExceptionExtensions.GetFullMessage(ex)));
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

                            _task.progress(String.Format("Provisioning step: {0}", deployedServer.progress.step.name), ReportProgress.Progress(_start_progress, _end_progress, _progress_base + 30 + deployedServer.progress.step.number));
                        }
                    }
                    deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                    Thread.Sleep(5000);
                }

                //add network interfaces
                int count = 0;
                foreach (MRPWorkloadInterfaceType _nic in _target_workload.workloadinterfaces.Where(x => x.vnic != 0))
                {
                    bool _add_nic = false;
                    if (deployedServer.networkInfo.additionalNic == null)
                    {
                        _add_nic = true;
                    }
                    else if (!deployedServer.networkInfo.additionalNic.ToList().Exists(x => x.key == _nic.vnic))
                    {
                        _add_nic = true;
                    }
                    if (_add_nic)
                    {
                        _task.progress(String.Format("Adding Network Interface: {0} : {1}", _nic.vnic, _nic.platformnetwork.network), ReportProgress.Progress(_start_progress, _end_progress, 50 + count));
                        ResponseType _create_status = CaaS.ServerManagement.Server.AddNic(new Guid(deployedServer.id), new Guid(_nic.platformnetwork.moid), _nic.ipaddress).Result;
                        WorkloadWaitUntilComplete.Invoke(CaaS, _newvm_platform_guid);
                    }
                    count += 1;
                }



                //Add disks
                count = 0;
                if (_os_customization)
                {
                    foreach (int _disk_index in _target_workload.workloadvolumes.OrderBy(x => x.diskindex).Select(x => x.diskindex).Distinct())
                    {
                        //increase disk by 5GB
                        long _disk_size = 0;
                        int _vm_scsi_id = (_disk_index > 6) ? (_disk_index + 1) : _disk_index;
                        _disk_size = (long)_target_workload.workloadvolumes.Where(x => x.diskindex == _disk_index).Sum(x => x.volumesize) + 1;
                        String _disk_tier = _target_workload.workloadvolumes.FirstOrDefault(x => x.diskindex == _disk_index).platformstoragetier_id;
                        if (deployedServer.disk.ToList().Exists(x => x.scsiId == _vm_scsi_id))
                        {
                            if (deployedServer.disk.ToList().FirstOrDefault(x => x.scsiId == _vm_scsi_id).sizeGb < _disk_size)
                            {
                                String _disk_guid = deployedServer.disk.ToList().Find(x => x.scsiId == _vm_scsi_id).id;
                                _task.progress(String.Format("Extending storage: {0} : {1}GB", _vm_scsi_id, _disk_size), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                                Status _create_status = CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(deployedServer.id, _disk_guid, _disk_size.ToString()).Result;
                            }
                        }
                        else
                        {
                            _task.progress(String.Format("Adding storage: {0} : {1}GB on {2}", _vm_scsi_id, _disk_size, _disk_tier), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                            Status _create_status = CaaS.ServerManagementLegacy.Server.AddServerDisk(deployedServer.id, _disk_size.ToString(), _disk_tier).Result;
                        }
                        WorkloadWaitUntilComplete.Invoke(CaaS, _newvm_platform_guid);
                        count += 1;
                    }
                }
                else
                {
                    foreach (MRPWorkloadDiskType _disk in _target_workload.workloaddisks)
                    {
                        if (deployedServer.disk.ToList().Exists(x => x.scsiId == _disk.diskindex))
                        {
                            if (deployedServer.disk.ToList().FirstOrDefault(x => x.scsiId == _disk.diskindex).sizeGb < _disk.disksize)
                            {
                                String _disk_guid = deployedServer.disk.ToList().Find(x => x.scsiId == _disk.diskindex).id;
                                _task.progress(String.Format("Extending storage: {0} : {1}GB", _disk.diskindex, _disk.disksize), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                                Status _create_status = CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(deployedServer.id, _disk_guid, _disk.disksize.ToString()).Result;
                            }
                        }
                        else
                        {
                            _task.progress(String.Format("Adding storage: {0} : {1}GB on {2}", _disk.diskindex, _disk.disksize, _disk.platformstoragetier_id), ReportProgress.Progress(_start_progress, _end_progress, 60 + count));
                            Status _create_status = CaaS.ServerManagementLegacy.Server.AddServerDisk(deployedServer.id, _disk.disksize.ToString(), _disk.platformstoragetier_id).Result;
                        }
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                        {
                            deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }
                }

                //Start Workload
                if (_os_customization)
                {
                    _task.progress(String.Format("Power on workload"), ReportProgress.Progress(_start_progress, _end_progress, 70));
                    ResponseType _start_server = CaaS.ServerManagement.Server.StartServer(_newvm_platform_guid).Result;
                    deployedServer = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                    while (deployedServer.state != "NORMAL" && deployedServer.started == false)
                    {
                        deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        Thread.Sleep(5000);
                    }

                    _task.progress(String.Format("Workload powered on"), ReportProgress.Progress(_start_progress, _end_progress, 71));
                }
                deployedServer = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;

                //give workload some time to become available
                Thread.Sleep(new TimeSpan(0, 0, 30));

                //Get updated workload object from portal and update the moid,credential_id,provisioned on the portal
                MRMPWorkloadBaseType _update_workload = new MRMPWorkloadBaseType();
                _update_workload.id = _target_workload.id;
                _update_workload.moid = deployedServer.id;
                _update_workload.hostname = deployedServer.name;
                _update_workload.provisioned = true;
                _update_workload.deleted = false;
                _update_workload.enabled = true;
                _update_workload.iplist = String.Join(",", deployedServer.networkInfo.primaryNic.ipv6, deployedServer.networkInfo.primaryNic.privateIpv4);

                _update_workload.workloadinterfaces = new List<MRPWorkloadInterfaceType>();

                MRPWorkloadInterfaceType _primary_nic = _target_workload.workloadinterfaces.FirstOrDefault(x => x.vnic == 0);
                _primary_nic.ipassignment = "manual_ip";
                _primary_nic.ipv6address = deployedServer.networkInfo.primaryNic.ipv6;
                _primary_nic.ipaddress = deployedServer.networkInfo.primaryNic.privateIpv4;
                _primary_nic.moid = deployedServer.networkInfo.primaryNic.id;
                _primary_nic.macaddress = deployedServer.networkInfo.primaryNic.macAddress;
                _primary_nic.deleted = false;
                _primary_nic._destroy = false;
                _update_workload.workloadinterfaces.Add(_primary_nic);

                if (deployedServer.networkInfo.additionalNic != null)
                {
                    foreach(var _deployed_nic in deployedServer.networkInfo.additionalNic)
                    {
                        MRPWorkloadInterfaceType _additional_nic = _target_workload.workloadinterfaces.FirstOrDefault(x => x.vnic == (_deployed_nic.key - 4000));
                        _additional_nic.ipassignment = "manual_ip";
                        _additional_nic.ipv6address = _deployed_nic.ipv6;
                        _additional_nic.ipaddress = _deployed_nic.privateIpv4;
                        _additional_nic.moid = _deployed_nic.id;
                        _additional_nic.macaddress = _deployed_nic.macAddress;
                        _additional_nic.deleted = false;
                        _additional_nic._destroy = false;
                        _update_workload.workloadinterfaces.Add(_additional_nic);
                    }
                }
                MRMPServiceBase._mrmp_api.workload().updateworkload(_update_workload);
                _task.progress(String.Format("Updating platform information for {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 80));
                PlatformInventoryWorkloadDo.UpdateMCPWorkload(_newvm_platform_guid.ToString(), _platform);
                _target_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);
                if (_os_customization)
                {
                    switch (deployedServer.guest.operatingSystem.family)
                    {
                        case "UNIX":
                            _task.progress(String.Format("Executing Operating System Customization"), ReportProgress.Progress(_start_progress, _end_progress, 90));
                            LinuxCustomization(_task, _platform, _target_workload, _protectiongroup, ReportProgress.Progress(_start_progress, _end_progress, 85), ReportProgress.Progress(_start_progress, _end_progress, 90));
                            _task.progress(String.Format("Updating operating system information for {0}", _target_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, 92));
                            WorkloadInventory.WorkloadInventoryLinuxDo(_target_workload);
                            break;
                        case "WINDOWS":
                            _task.progress(String.Format("Executing Operating System Customization"), ReportProgress.Progress(_start_progress, _end_progress, 90));
                            _target_workload.WorkloadCustomizationWMI();
                            _task.progress(String.Format("Executing Operating System Inventory"), ReportProgress.Progress(_start_progress, _end_progress, 92));
                            WorkloadInventory.WorkloadInventoryWindowsDo(_target_workload);
                            break;
                    }
                }
                _task.progress(String.Format("Successfully provisioned VM [{0}] in [{1}]", deployedServer.name, _dc.displayName), ReportProgress.Progress(_start_progress, _end_progress, 95));
            }
            else
            {
                throw new Exception(String.Format("Failed to create target virtual machine: {0}", _status.error));
            }
        }
    }
}

