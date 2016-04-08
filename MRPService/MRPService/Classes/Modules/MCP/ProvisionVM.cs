using DD.CBU.Compute.Api.Client;
using DD.CBU.Compute.Api.Contracts.Network20;
using DD.CBU.Compute.Api.Contracts.Requests.Infrastructure;
using MRPService.LocalDatabase;
using MRPService.MRPService.Log;
using MRPService.MRPService.Types.API;
using MRPService.API.Classes;
using MRPService.API.Types.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using MRPService.Utilities;
using MRPService.PlatformInventory;

namespace MRPService.Tasks.MCP
{
    class MCP_Platform
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_SERVICE = 3;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool LogonUser(
            String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public static async void ProvisionVM(MRPTaskType payload)
        {
            //Get workload object from portal to perform updates once provisioned
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();

            MRPTaskSubmitpayloadType _payload = payload.submitpayload;
            MRPDatabase db = new MRPDatabase();
            Platform _platform;
            using (PlatformSet _platform_db = new PlatformSet())
            {
                _platform = _platform_db.ModelRepository.GetById(_payload.platform.id);
            }
            if (_platform == null)
            {
                throw new System.ArgumentException(String.Format("Cannot find platform {0} in local database", _payload.platform.platform));
            }


            //Retrieve or create new standalone credentials
            Credential _stadalone_credential;
            Credential _platform_credentail;
            using (CredentialSet _credential_db = new CredentialSet())
            {
                //get credential for platform
                _platform_credentail = _credential_db.ModelRepository.GetById(_platform.credential_id);

                //get or create credentials for standalone workload
                if (_credential_db.ModelRepository.Get(x => x.description == "Internal Windows Credentials").Count > 0)
                {
                    _stadalone_credential = _credential_db.ModelRepository.Get(x => x.description == "Internal Windows Credentials").FirstOrDefault();
                }
                else
                {
                    _stadalone_credential = new Credential();
                    _stadalone_credential.credential_type = 1;
                    _stadalone_credential.description = "Internal Windows Credentials";
                    _stadalone_credential.username = "Administrator";
                    _stadalone_credential.password = new Random().GetSHA1Hash().Substring(0, 12);
                    _stadalone_credential.id = Objects.RamdomGuid();
                    _credential_db.ModelRepository.Insert(_stadalone_credential);
                }
            }
            if (_stadalone_credential == null)
            {
                throw new System.ArgumentException("Cannot find standalone credential for workload deployment");
            }




            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform_credentail.username, _platform_credentail.password));
            CaaS.Login().Wait();

            try
            {
                DataCenterListOptions _dc_options = new DataCenterListOptions();
                _dc_options.Id = _platform.moid;
                DatacenterType _dc = CaaS.Infrastructure.GetDataCenters(null, _dc_options).Result.FirstOrDefault();


                MRPTaskWorkloadType _target = _payload.target;

                DeployServerType _vm = new DeployServerType();

                List<DeployServerTypeDisk> _disks = new List<DeployServerTypeDisk>();

                //Set Tier for first disk being deployed
                MRPTaskDiskType _first_disk = _target.disks.FirstOrDefault(x => x.diskindex == 0);
                _disks.Add(new DeployServerTypeDisk() { scsiId = 0, speed = _first_disk.platformstoragetier.shortname });

                _vm.name = _target.hostname;
                _vm.description = String.Format("Workload Created by Dimension Data MRP [{0}]", DateTime.Now);
                DeployServerTypeNetwork _network = new DeployServerTypeNetwork();
                _network.Item = _target.interfaces[0].platformnetwork.moid;
                _network.ItemElementName = NetworkIdOrPrivateIpv4ChoiceType.networkId;
                _network.networkId = _target.interfaces[0].platformnetwork.moid;
                DeployServerTypeNetworkInfo _networkInfo = new DeployServerTypeNetworkInfo();
                _networkInfo.networkDomainId = _target.interfaces[0].platformnetwork.networkdomain_moid;
                _networkInfo.primaryNic = new VlanIdOrPrivateIpType()
                {
                    vlanId = _target.interfaces[0].platformnetwork.moid != null ? _target.interfaces[0].platformnetwork.moid : null,
                    privateIpv4 = _target.interfaces[0].ipassignment != "auto_ip" ? _target.interfaces[0].ipaddress : null
                };
                _vm.network = _network;
                _vm.networkInfo = _networkInfo;
                _vm.imageId = _target.platform_template.image_moid;
                _vm.cpu = new DeployServerTypeCpu() { count = Convert.ToUInt16(_target.vcpu), countSpecified = true, coresPerSocket = Convert.ToUInt16(_target.vcore), coresPerSocketSpecified = true };
                _vm.memoryGb = Convert.ToUInt16(_target.vmemory);
                _vm.start = false;
                _vm.disk = _disks.ToArray();
                _vm.administratorPassword = _stadalone_credential.password;

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
                            using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                            {
                                _mrp_api.task().failcomplete(payload, String.Format("Error submitting workload creation task:", ex.Message));
                                Logger.log(String.Format("Error submitting workload creation task:", ex.Message), Logger.Severity.Error);
                                return;
                            }
                        }
                        else Thread.Sleep(5000);
                    }
                }


                if (_status.responseCode == "IN_PROGRESS")
                {
                    var serverInfo = _status.info.Single(info => info.name == "serverId");
                    ServerType deployedServer = null;
                    {
                        deployedServer = await CaaS.ServerManagement.Server.GetServer(Guid.Parse(serverInfo.value));
                    }
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("{0} provisioning started in {1} ({2})", _vm.name, _dc.displayName, _dc.id), 20);
                    }
                    ServerType _newvm = await CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id));
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        if (_newvm.progress != null)
                        {
                            if (_newvm.progress.step != null)
                            {
                                using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                                {
                                    _mrp_api.task().progress(payload, String.Format("Provisioning step: {0}", _newvm.progress.step.name), 30 + _newvm.progress.step.number);
                                }
                            }
                        }
                        _newvm = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                        Thread.Sleep(5000);
                    }

                    //Expand C: drive and Add additional disks if required
                    int count = 0;
                    foreach (MRPTaskDiskType _volume in _target.disks)
                    {
                        if (_newvm.disk.ToList().Exists(x => x.scsiId == _volume.diskindex))
                        {
                            if (_newvm.disk.ToList().Find(x => x.scsiId == _volume.diskindex).sizeGb < _volume.disksize)
                            {
                                using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                                {
                                    _mrp_api.task().progress(payload, String.Format("Extending storage: {0} : {1}GB", _volume.diskindex, _volume.disksize), 60 + count);
                                }
                                await CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(deployedServer.id, _volume.diskindex.ToString(), _volume.disksize.ToString());
                            }
                        }
                        else
                        {
                            using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                            {
                                _mrp_api.task().progress(payload, String.Format("Adding storage: {0} : {1}GB on {2}", _volume.diskindex, _volume.disksize, _volume.platformstoragetier.storagetier), 60 + count);
                            }
                            await CaaS.ServerManagementLegacy.Server.AddServerDisk(deployedServer.id, _volume.disksize.ToString(), _volume.platformstoragetier.shortname);
                        }
                        _newvm = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                        while (_newvm.state != "NORMAL" && _newvm.started == false)
                        {
                            _newvm = CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id)).Result;
                            Thread.Sleep(5000);
                        }
                        count += 1;
                    }

                    //Start Workload
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Power on workload"), 70);
                    }
                    await CaaS.ServerManagement.Server.StartServer(Guid.Parse(deployedServer.id));
                    _newvm = await CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id));
                    while (_newvm.state != "NORMAL" && _newvm.started == false)
                    {
                        _newvm = await CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id));
                        Thread.Sleep(5000);
                    }
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Workload powered on"), 71);
                    }
                    _newvm = await CaaS.ServerManagement.Server.GetServer(Guid.Parse(deployedServer.id));


                    //create diskpart file for virtual machine and copy it to the remote workload
                    List<String> _driveletters = new List<String>();
                    List<String> _systemdriveletters = new List<String>();
                    _systemdriveletters.AddRange("DEFGHIJKLMNOPQRSTUVWXYZ".Select(d => d.ToString()));
                    List<String> _diskpart_struct = new List<string>();
                    _diskpart_struct.Add("select volume c");
                    _diskpart_struct.Add("extend noerr");
                    _diskpart_struct.Add("");
                    List<String> _availabledriveletters = _systemdriveletters.Except(_driveletters).ToList<String>();
                    _diskpart_struct.Add("select volume d");
                    _diskpart_struct.Add(String.Format("assign letter={0} noerr", _availabledriveletters.Last()));
                    _diskpart_struct.Add("");
                    foreach (ServerTypeDisk _disk in _newvm.disk.ToList().FindAll(x => x.scsiId != 0).OrderBy(x => x.scsiId))
                    {
                        string _driveletter = _target.volumes.Find(x => x.diskindex == _disk.scsiId).driveletter.Substring(0, 1);
                        _driveletters.Add(_driveletter.ToString());
                        _diskpart_struct.Add(String.Format("select disk {0}", _disk.scsiId));
                        _diskpart_struct.Add("ATTRIBUTES DISK CLEAR READONLY");
                        _diskpart_struct.Add("ONLINE DISK");
                        _diskpart_struct.Add("clean");
                        _diskpart_struct.Add("create partition primary");
                        _diskpart_struct.Add("select partition 1");
                        _diskpart_struct.Add("format fs=ntfs quick");
                        _diskpart_struct.Add(String.Format("assign letter={0} noerr", _driveletter));
                        _diskpart_struct.Add("active");
                        _diskpart_struct.Add("");
                    }

                    Thread.Sleep(new TimeSpan(0, 0, 30));

                    string _ip_list = String.Join(",", _newvm.networkInfo.primaryNic.ipv6, _newvm.networkInfo.primaryNic.privateIpv4);
                    string _working_ip = Connection.FindConnection(_ip_list, true);

                    Logger.log(String.Format("Found working ip: {0}", _working_ip), Logger.Severity.Info);

                    IntPtr userHandle = new IntPtr(0);

                    try
                    {
                        LogonUser("Administrator", ".", _stadalone_credential.password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref userHandle);
                        WindowsIdentity identity = new WindowsIdentity(userHandle);
                        WindowsImpersonationContext context = identity.Impersonate();
                    }
                    catch (Exception ex)
                    {
                        using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, String.Format("Error impersonating Administrator user: {0}", ex.Message));
                        }
                        Logger.log(ex.Message, Logger.Severity.Error);
                        return;
                    }
                    string workloadPath = null;
                    string remoteInstallFiles = @"C:\";
                    remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                    workloadPath = @"\\" + Path.Combine(_working_ip, remoteInstallFiles, "diskpart.txt");
                    int _copy_retries = 30;
                    while (true)
                    {
                        try
                        {
                            File.WriteAllLines(workloadPath, _diskpart_struct.ConvertAll(Convert.ToString));
                            Logger.log(String.Format("Successfully copied diskpart disk after {0} retries", _copy_retries), Logger.Severity.Info);
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (--_copy_retries == 0)
                            {
                                Logger.log(String.Format("Error creating disk layout file on workload {0}: {1} : {2}", _working_ip, ex.Message, workloadPath), Logger.Severity.Info);
                                using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                                {
                                    _mrp_api.task().failcomplete(payload, String.Format("Error creating disk layout file on workload: {0}", ex.Message));
                                }
                                return;
                            }
                            else Thread.Sleep(new TimeSpan(0, 0, 30));
                        }
                    }

                    //Run Diskpart Command on Workload
                    //Create connection object to remote machine
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Volume setup process on {0}", _newvm.name), 80);
                    }
                    ConnectionOptions connOptions = new ConnectionOptions() { EnablePrivileges = true, Username = "Administrator", Password = _stadalone_credential.password };
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    ManagementScope scope = new ManagementScope(@"\\" + _working_ip + @"\root\CIMV2", connOptions);
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
                                Logger.log(String.Format("Error running diskpart on workload {0}: {1} : {2}", _working_ip, ex.Message, workloadPath), Logger.Severity.Info);
                                using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                                {
                                    _mrp_api.task().failcomplete(payload, String.Format("Error running diskpart on workload: {0}", ex.Message));
                                }
                                return;
                            }
                            else Thread.Sleep(5000);
                        }
                    }

                    string diskpartCmd = @"diskpart /s C:\diskpart.txt";
                    Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                    installCmdParams["CommandLine"] = diskpartCmd;
                    installCmdParams["CurrentDirectory"] = @"C:\Windows";

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
                        using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                        {
                            _mrp_api.task().failcomplete(payload, String.Format("Failed diskpart process on {0} ({1})", _newvm.name, _exitcode));
                        }
                        return;
                    }
                    else
                    {
                        using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                        {
                            _mrp_api.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
                        }
                    }
                    //Add or update workload record in database
                    Workload _new_workload = new Workload() { id = _target.id, moid = _newvm.id, enabled = true, hostname = _target.hostname, platform_id = _platform.id, credential_id = _stadalone_credential.id };
                    if (db.Workloads.Any(x => x.id == _target.id))
                    {
                        Workload _current_workload = db.Workloads.FirstOrDefault(x => x.id == _target.id);
                        Objects.Copy(_new_workload, _current_workload);
                    }
                    else
                    {
                        db.Workloads.Add(_new_workload);
                    }
                    db.SaveChanges();

                    //Get updated workload object from portal and update the moid,credential_id,provisioned on the portal
                    MRPWorkloadType _workload = _cloud_movey.workload().getworkload(_target.id);
                    MRPWorkloadCRUDType _update_workload = new MRPWorkloadCRUDType();
                    _update_workload.id = _target.id;
                    _update_workload.moid = _newvm.id;
                    _update_workload.workloadtype = _workload.workloadtype;
                    _update_workload.credential_id = _stadalone_credential.id;
                    _update_workload.provisioned = true;

                    //clear all disks, volumes and interfaces and force new discovery
                    _update_workload.workloaddisks_attributes = _workload.disks.Select(x => new MRPWorkloadDiskType() { id = x.id, _destroy = true }).ToList();
                    _update_workload.workloadvolumes_attributes = _workload.volumes.Select(x => new MRPWorkloadVolumeType() { id = x.id, _destroy = true }).ToList();
                    _update_workload.workloadinterfaces_attributes = _workload.interfaces.Select(x => new MRPWorkloadInterfaceType() { id = x.id, _destroy = true }).ToList();
                    _cloud_movey.workload().updateworkload(_update_workload);

                    //update Platform inventory for server
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Updating platform information for {0}", _target.hostname), 91);
                    }
                    PlatformInventoryWorkloadDo.UpdateMCPWorkload(_newvm.id, _newvm.datacenterId);

                    //update OS information or newly provisioned server
                    _workload = _cloud_movey.workload().getworkload(_target.id);
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().progress(payload, String.Format("Updating operating system information for {0}", _target.hostname), 92);
                    }
                    WorkloadInventory.WorkloadInventoryDo(_workload.id);

                    //log the success
                    Logger.log(String.Format("Successfully provinioned VM [{0}] in [{1}]: {2}", _newvm.name, _dc.displayName, JsonConvert.SerializeObject(_newvm)), Logger.Severity.Debug);
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().successcomplete(payload, JsonConvert.SerializeObject(_newvm));
                    }
                }
                else
                {
                    using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
                    {
                        _mrp_api.task().failcomplete(payload, String.Format("Failed to create target virtual machine: {0}", _status.ToString()));
                    }
                    Logger.log(String.Format("Failed to create target virtual machine: {0}", _status.error), Logger.Severity.Error);

                }
            }
            catch (Exception e)
            {
                using (API.MRP_ApiClient _mrp_api = new API.MRP_ApiClient())
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
