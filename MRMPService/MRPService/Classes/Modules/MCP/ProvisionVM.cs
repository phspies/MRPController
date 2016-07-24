using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPAPI.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading;
using MRMPService.Utilities;
using MRMPService.PlatformInventory;
using DD.CBU.Compute.Api.Contracts.General;

namespace MRMPService.Tasks.MCP
{
    class MCP_Platform
    {
        public static void ProvisionVM(MRPTaskType payload)
        {
            //Get workload object from portal to perform updates once provisioned
            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
            {
                _mrp_api.task().progress(payload, String.Format("Starting provisioning process"));
            }

            MRPTaskSubmitpayloadType _payload = payload.submitpayload;
            MRPPlatformType _platform = _payload.platform;
            if (_platform == null)
            {
                throw new System.ArgumentException(String.Format("Cannot find platform {0} in local database", _payload.platform.platform));
            }


            MRPWorkloadType _target_workload = _payload.target;
            MRPCredentialType _stadalone_credential = _target_workload.credential;
            MRPCredentialType _platform_credentail = _platform.credential;

            if (_stadalone_credential == null)
            {
                throw new System.ArgumentException("Cannot find standalone credential for workload deployment");
            }

            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credentail.username, _platform_credentail.encrypted_password));
            CaaS.Login().Wait();

            try
            {
                DataCenterListOptions _dc_options = new DataCenterListOptions();
                _dc_options.Id = _platform.moid;
                DatacenterType _dc = CaaS.Infrastructure.GetDataCenters(null, _dc_options).Result.FirstOrDefault();

                DeployServerType _vm = new DeployServerType();

                List<DeployServerTypeDisk> _disks = new List<DeployServerTypeDisk>();

                //Set Tier for first disk being deployed
                MRPWorkloadVolumeType _first_disk = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.diskindex == 0);
                _disks.Add(new DeployServerTypeDisk() { scsiId = 0, speed = _first_disk.platformstoragetier.shortname });

                _vm.name = _target_workload.hostname;
                _vm.description = String.Format("{0} MRMP : {1}", DateTime.UtcNow, payload.submitpayload.protectiongroup.service);
                DeployServerTypeNetwork _network = new DeployServerTypeNetwork();
                _network.Item = _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid;
                _network.ItemElementName = NetworkIdOrPrivateIpv4ChoiceType.networkId;
                _network.networkId = _target_workload.workloadinterfaces_attributes[0].platformnetwork.moid;
                DeployServerTypeNetworkInfo _networkInfo = new DeployServerTypeNetworkInfo();
                _networkInfo.networkDomainId = _target_workload.workloadinterfaces_attributes[0].platformnetwork.networkdomain_moid;
                _networkInfo.primaryNic = new VlanIdOrPrivateIpType()
                {
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
                _vm.administratorPassword = _stadalone_credential.encrypted_password;
                _vm.primaryDns = _target_workload.primary_dns;
                _vm.secondaryDns = _target_workload.secondary_dns;
                _vm.microsoftTimeZone = _target_workload.timezone;
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
                                _mrp_api.task().failcomplete(payload, String.Format("Error submitting workload creation task: {0} : {1}", ex.Message, _status.error));
                                Logger.log(String.Format("Error submitting workload creation task: {0} : {1}", ex.Message, _status.error), Logger.Severity.Error);
                                return;
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
                        _mrp_api.task().progress(payload, String.Format("{0} provisioning started in {1} ({2})", _vm.name, _dc.displayName, _dc.id), 20);
                    }
                    ServerType _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        if (_newvm.progress != null)
                        {
                            if (_newvm.progress.step != null)
                            {
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(payload, String.Format("Provisioning step: {0}", _newvm.progress.step.name), 30 + _newvm.progress.step.number);
                                }
                            }
                        }
                        _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (int _disk_index in _target_workload.workloadvolumes_attributes.OrderBy(x => x.diskindex).Select(x => x.diskindex).Distinct())
                    {
                        //increase disk by 5GB
                        long _disk_size = _target_workload.workloadvolumes_attributes.Where(x => x.diskindex == _disk_index).Sum(x => x.volumesize) + 1;

                        MRPPlatformStorageTierType _disk_tier = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.diskindex == _disk_index).platformstoragetier;
                        if (_newvm.disk.ToList().Exists(x => x.scsiId == _disk_index))
                        {
                            if (_newvm.disk.ToList().Find(x => x.scsiId == _disk_index).sizeGb < _disk_size)
                            {
                                String _disk_guid = _newvm.disk.ToList().Find(x => x.scsiId == _disk_index).id;
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().progress(payload, String.Format("Extending storage: {0} : {1}GB", _disk_index, _disk_size), 60 + count);
                                }
                                Status _create_status = CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(deployedServer.id, _disk_guid, _disk_size.ToString()).Result;
                            }
                        }
                        else
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                                _mrp_api.task().progress(payload, String.Format("Adding storage: {0} : {1}GB on {2}", _disk_index, _disk_size, _disk_tier.storagetier), 60 + count);
                            }
                            Status _create_status = CaaS.ServerManagementLegacy.Server.AddServerDisk(deployedServer.id, _disk_size.ToString(), _disk_tier.shortname).Result;
                        }
                        _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        while (_newvm.state != "NORMAL" && _newvm.started == false)
                        {
                            _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Workload
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Power on workload"), 70);
                    }
                    ResponseType _start_server = CaaS.ServerManagement.Server.StartServer(_newvm_platform_guid).Result;
                    _newvm = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;
                        Thread.Sleep(5000);
                    }
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Workload powered on"), 71);
                    }
                    _newvm = CaaS.ServerManagement.Server.GetServer(_newvm_platform_guid).Result;

                    //gibe workload some time to become available
                    Thread.Sleep(new TimeSpan(0, 0, 30));

                    //find working ip of server
                    string new_workload_ip = null;
                    int ip_find_try = 0;
                    using (Connection _connection = new Connection())
                    {
                        while (ip_find_try < 3)
                        {
                            new_workload_ip = _connection.FindConnection(String.Join(",", _newvm.networkInfo.primaryNic.ipv6, _newvm.networkInfo.primaryNic.privateIpv4), true);
                            if (new_workload_ip != null)
                            {
                                break;
                            }
                            ip_find_try++;
                            Thread.Sleep(new TimeSpan(0, 0, 30));
                        }
                    }
                    if (new_workload_ip == null)
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                        }
                        return;
                    }

                    long _c_volume_actual_size = 0;
                    long _c_volume_actual_free = 0;
                    string _cdrom_drive_letter = "";
                    ConnectionOptions options = WMIHelper.ProcessConnectionOptions((String.IsNullOrWhiteSpace(_stadalone_credential.domain) ? (@".\" + _stadalone_credential.username) : (_stadalone_credential.domain + @"\" + _stadalone_credential.username)), _stadalone_credential.encrypted_password);
                    ManagementScope connectionScope = WMIHelper.ConnectionScope(new_workload_ip, options);
                    SelectQuery VolumeQuery = new SelectQuery("SELECT Size, FreeSpace FROM Win32_LogicalDisk WHERE DeviceID = 'C:'");
                    SelectQuery CdromQuery = new SelectQuery("SELECT Drive FROM Win32_CDROMDrive");
                    foreach (var item in new ManagementObjectSearcher(connectionScope, CdromQuery).Get())
                    {
                        try
                        {
                            _cdrom_drive_letter = item["Drive"].ToString();
                        }
                        catch (Exception ex)
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                            }
                        }
                    }
                    foreach (var item in new ManagementObjectSearcher(connectionScope, VolumeQuery).Get())
                    {
                        try
                        {
                            _c_volume_actual_size = Convert.ToInt64(item["Size"].ToString());
                            _c_volume_actual_free = Convert.ToInt64(item["FreeSpace"].ToString());
                        }
                        catch (Exception ex)
                        {
                            using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                            {
                                _mrp_api.task().failcomplete(payload, String.Format("Error collecting C: volume space information for {0}", _newvm.name));
                            }
                            return;
                        }
                    }
                    MRPWorkloadVolumeType _c_volume_object = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.driveletter == "C");
                    long _c_volume_to_add = 0;
                    if (_c_volume_object != null)
                    {
                        _c_volume_to_add = (_c_volume_object.volumesize * 1024 * 1024 * 1024) - _c_volume_actual_size;
                    }
                    else
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, "Cannot find C: drive in volume list for partition mapping");
                        }
                        return;
                    }


                    List<String> _used_drive_letters = _target_workload.workloadvolumes_attributes.Select(x => x.driveletter.Substring(0, 1)).ToList();
                    List<String> _systemdriveletters = new List<String>();
                    _systemdriveletters.AddRange("DEFGHIJKLMNOPQRSTUVWXYZ".Select(d => d.ToString()));
                    List<String> _availabledriveletters = _systemdriveletters.Except(_used_drive_letters).ToList<String>();

                    List<String> _diskpart_struct = new List<string>();
                    //convert number MB from bytes
                    _c_volume_to_add = _c_volume_to_add / 1024 / 1024;

                    if (_c_volume_to_add > 0)
                    {
                        _diskpart_struct.Add("select volume c");
                        _diskpart_struct.Add(String.Format("extend size={0} disk=0 noerr", _c_volume_to_add));
                        _diskpart_struct.Add("");
                    }

                    //change cdrom drive letter
                    if (_cdrom_drive_letter != "")
                    {
                        _diskpart_struct.Add(String.Format("select volume {0}", _cdrom_drive_letter.Substring(0, 1)));
                        _diskpart_struct.Add(String.Format("assign letter={0} noerr", _availabledriveletters.Last()));
                        _diskpart_struct.Add("");
                    }
                    foreach (int _disk_index in _target_workload.workloadvolumes_attributes.Select(x => x.diskindex).Distinct())
                    {
                        if (_disk_index != 0)
                        {
                            _diskpart_struct.Add(String.Format("select disk {0}", _disk_index));
                            _diskpart_struct.Add("ATTRIBUTES DISK CLEAR READONLY noerr");
                            _diskpart_struct.Add("ONLINE DISK noerr");
                        }
                        int _vol_index = 0;
                        if (_disk_index == 0)
                        {
                            _vol_index = 2;
                        }
                        else
                        {
                            _vol_index = 1;
                        }
                        foreach (MRPWorkloadVolumeType _volume in _target_workload.workloadvolumes_attributes.ToList().Where(x => x.diskindex == _disk_index && !x.driveletter.Contains("C")).OrderBy(x => x.driveletter))
                        {
                            string _driveletter = _volume.driveletter.Substring(0, 1);
                            _diskpart_struct.Add("clean");
                            _diskpart_struct.Add(String.Format("create partition primary size={0} noerr", _volume.volumesize * 1024));
                            _diskpart_struct.Add(String.Format("select partition {0}", _vol_index));
                            _diskpart_struct.Add("format fs=ntfs quick");
                            _diskpart_struct.Add(String.Format("assign letter={0} noerr", _driveletter));
                            _diskpart_struct.Add("active");
                            _diskpart_struct.Add("");
                            _vol_index++;
                        }
                    }
                    string workloadPath = null;
                    string diskpart_bat = null;
                    string[] diskpart_bat_content = new String[] { @"C:\Windows\System32\diskpart.exe /s C:\diskpart.txt > C:\diskpart.log" };
                    try
                    {
                        using (new Impersonator(_stadalone_credential.username, (String.IsNullOrWhiteSpace(_stadalone_credential.domain) ? "." : _stadalone_credential.domain), _stadalone_credential.encrypted_password))
                        {
                            string remoteInstallFiles = @"C:\";
                            remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                            workloadPath = @"\\" + Path.Combine(new_workload_ip, remoteInstallFiles, "diskpart.txt");
                            diskpart_bat = @"\\" + Path.Combine(new_workload_ip, remoteInstallFiles, "diskpart.bat");

                            int _copy_retries = 30;
                            while (true)
                            {
                                try
                                {
                                    File.WriteAllLines(workloadPath, _diskpart_struct.ConvertAll(Convert.ToString));
                                    File.WriteAllLines(diskpart_bat, diskpart_bat_content);
                                    Logger.log(String.Format("Successfully copied diskpart disk after {0} retries", _copy_retries), Logger.Severity.Info);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    if (--_copy_retries == 0)
                                    {
                                        Logger.log(String.Format("Error creating disk layout file on workload {0}: {1} : {2}", new_workload_ip, ex.Message, workloadPath), Logger.Severity.Info);
                                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                        {
                                            _mrp_api.task().failcomplete(payload, String.Format("Error creating disk layout file on workload: {0}", ex.Message));
                                        }
                                        return;
                                    }
                                    else Thread.Sleep(new TimeSpan(0, 0, 30));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, String.Format("Error impersonating Administrator user: {0}", ex.Message));
                        }
                        Logger.log(ex.Message, Logger.Severity.Error);
                        return;
                    }
                    //Run Diskpart Command on Workload
                    //Create connection object to remote machine
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Volume setup process on {0}", _newvm.name), 80);
                    }
                    ConnectionOptions connOptions = new ConnectionOptions() { EnablePrivileges = true, Username = "Administrator", Password = _stadalone_credential.encrypted_password };
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    ManagementScope scope = new ManagementScope(@"\\" + new_workload_ip + @"\root\CIMV2", connOptions);
                    int _connect_retries = 3;
                    while (true)
                    {
                        try
                        {
                            scope.Connect();
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (--_connect_retries == 0)
                            {
                                Logger.log(String.Format("Error running diskpart on workload {0}: {1} : {2}", new_workload_ip, ex.Message, workloadPath), Logger.Severity.Info);
                                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                                {
                                    _mrp_api.task().failcomplete(payload, String.Format("Error running diskpart on workload: {0}", ex.Message));
                                }
                                return;
                            }
                            else Thread.Sleep(5000);
                        }
                    }

                    string diskpartCmd = @"C:\diskpart.bat";
                    Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                    installCmdParams["CommandLine"] = diskpartCmd;
                    installCmdParams["CurrentDirectory"] = @"C:\";

                    Dictionary<string, object> returnValues = new Dictionary<string, object>();
                    ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                    ObjectGetOptions ogo = new ObjectGetOptions();
                    ManagementBaseObject returnValue;
                    int processId = 0;
                    using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
                    {
                        ManagementBaseObject inparams = mc.GetMethodParameters("Create");
                        if (installCmdParams != null)
                        {
                            foreach (var p in installCmdParams)
                            {
                                inparams[p.Key] = p.Value;
                            }
                        }
                        returnValue = mc.InvokeMethod("Create", inparams, null);
                    }

                    if (returnValues != null)
                    {
                        processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                    }
                    int _exitcode = Convert.ToInt32(returnValue.Properties["ReturnValue"].Value);
                    if (_exitcode != 0)
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, String.Format("Failed diskpart process on {0} ({1})", _newvm.name, _exitcode));
                        }
                        return;
                    }
                    else
                    {
                        using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                        {
                            _mrp_api.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
                        }
                    }


                    //Get updated workload object from portal and update the moid,credential_id,provisioned on the portal
                    MRPWorkloadType _update_workload = new MRPWorkloadType();
                    _update_workload.id = _target_workload.id;
                    _update_workload.moid = _newvm.id;
                    _update_workload.workloadtype = _target_workload.workloadtype;
                    _update_workload.credential_id = _stadalone_credential.id;
                    _update_workload.provisioned = true;

                    //update first network interface with the newly provisioned server's information
                    var _interface = _update_workload.workloadinterfaces_attributes.First();
                    _interface.moid = _newvm.networkInfo.primaryNic.id;
                    _interface.ipaddress = _newvm.networkInfo.primaryNic.privateIpv4;
                    _interface.ipassignment = "manual";
                    _interface.ipv6address = _newvm.networkInfo.primaryNic.ipv6;

                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.workload().updateworkload(_update_workload);
                    }

                    //update Platform inventory for server
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Updating platform information for {0}", _target_workload.hostname), 91);
                    }
                    (new PlatformInventoryWorkloadDo()).UpdateMCPWorkload(_newvm_platform_guid.ToString(), _target_workload.platform);

                    //update OS information or newly provisioned server
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Updating operating system information for {0}", _target_workload.hostname), 92);
                    }
                    (new WorkloadInventory()).WorkloadInventoryWindowsDo(_target_workload);

                    //log the success
                    Logger.log(String.Format("Successfully provisioned VM [{0}] in [{1}]: {2}", _newvm.name, _dc.displayName, JsonConvert.SerializeObject(_newvm)), Logger.Severity.Debug);
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().successcomplete(payload, JsonConvert.SerializeObject(_newvm));
                    }
                }
                else
                {
                    using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                    {
                        _mrp_api.task().failcomplete(payload, String.Format("Failed to create target virtual machine: {0}", _status.ToString()));
                    }
                    Logger.log(String.Format("Failed to create target virtual machine: {0}", _status.error), Logger.Severity.Error);

                }
            }
            catch (Exception e)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(payload, e.Message);
                }
                Logger.log(e.ToString(), Logger.Severity.Error);
            }
        }
        public static string mcp_getimage_name(String _ostype)
        {
            String _template = "WIN";
            bool X64 = false;
            if (_ostype.Contains("2012"))
            {
                _template = _template + "2012";
                if (_ostype.Contains("R2"))
                {
                    _template = _template + "R2";
                    X64 = true;
                }
            }
            else if (_ostype.Contains("2008"))
            {
                _template = _template + "2008";
                if (_ostype.Contains("R2"))
                {
                    _template = _template + "R2";
                    X64 = true;
                }
            }
            if (_ostype.Contains("Standard"))
            {
                _template = _template + "S";
                X64 = true;

            }
            else if (_ostype.Contains("Enterprise"))
            {
                _template = _template + "E";
                X64 = true;

            }
            else if (_ostype.Contains("Datacenter"))
            {
                _template = _template + "DC";
                X64 = true;

            }
            switch (X64)
            {
                case true:
                    _template = _template + "64";
                    break;
                case false:
                    _template = _template + "32";
                    break;
            }
            return _template;
        }
    }
}
