using Microsoft.Win32;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security;
using System.Threading;
using MRMPService.Utilities;
using DoubleTake.Web.Models;
using MRMPService.DoubleTake;

namespace MRMPService.Tasks.DoubleTake
{
    class Deploy
    {
        static dt_server_type server_type = dt_server_type.source;
        public static async void DeployDoubleTake(MRPTaskType payload)
        {
            API.MRP_ApiClient _mrp_portal = new API.MRP_ApiClient();
            
            try
            {
                MRPDatabase _db = new MRPDatabase();
                MRPTaskWorkloadType _source_workload = payload.submitpayload.source;
                MRPTaskWorkloadType _target_workload = payload.submitpayload.target;

                MRPTaskWorkloadType _working_workload = new MRPTaskWorkloadType();
                int _counter = 0;
                while (true)
                { 
                    switch (server_type)
                    {
                        case dt_server_type.source:
                            _working_workload = _source_workload;
                            _counter = 1;
                            break;
                        case dt_server_type.target:
                            _working_workload = _target_workload;
                            _counter = 50;
                            break;
                    }

                    _mrp_portal.task().progress(payload, String.Format("Starting DT deploying process on {0}", _working_workload.hostname), _counter + 5);

                    string remoteTempLocation = _working_workload.deploymentpolicy.dt_temppath;
                    LocalDatabase.Workload _db_workload;
                    using (WorkloadSet _local_db = new WorkloadSet())
                    {
                        _db_workload = _local_db.ModelRepository.GetById(_working_workload.id);
                    }
                    Credential _workload_credentials;
                    using (CredentialSet _local_db = new CredentialSet())
                    {
                        _workload_credentials = _local_db.ModelRepository.GetById(_db_workload.credential_id);
                    }
                    if (_workload_credentials == null)
                    {
                        _mrp_portal.task().progress(payload, String.Format("Cannot determine credentials for {0}", _working_workload.hostname), _counter + 16);
                        server_type = dt_server_type.target;
                        continue;
                    }
                    string _contactable_ip = null;
                    using (Connection _connection = new Connection())
                    {
                        _contactable_ip = _connection.FindConnection(_working_workload.iplist, true);
                    }
                    if (_contactable_ip == null)
                    {
                        _mrp_portal.task().failcomplete(payload, String.Format("Cannot contant workload {0}", _working_workload.hostname));
                        server_type = dt_server_type.target;
                        continue;
                    }

                    _mrp_portal.task().progress(payload, "Get remote architecture", _counter + 10);
                    string systemArchitecture = null;
                    //Determine if the setup to be installed is 32 bit or 64 bit
                    string keyString = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                    using (new Impersonator(_workload_credentials.username, (String.IsNullOrWhiteSpace(_workload_credentials.domain) ? "." : _workload_credentials.domain), _workload_credentials.password))
                    {
                        #region Detect Target Architecture
                        RegistryKey rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _contactable_ip);
                        RegistryKey key = rk.OpenSubKey(keyString);
                        if (key != null)
                        {
                            string architecture = (string)key.GetValue("PROCESSOR_ARCHITECTURE");
                            if (architecture.Contains("64"))
                            {
                                systemArchitecture = "X64";
                            }
                            else
                            {
                                _mrp_portal.task().progress(payload, String.Format("32Bit Workloads not supported {0} ", _working_workload.hostname), _counter + 10);
                                server_type = dt_server_type.target;
                                continue;
                            }
                        }
                        else
                        {
                            _mrp_portal.task().progress(payload, String.Format("Cannot determine remote achitecture for {0}", _working_workload.hostname), _counter + 11);
                            server_type = dt_server_type.target;
                            continue;
                        }
                        _mrp_portal.task().progress(payload, String.Format("{0} is of type {1} architecture", _working_workload.hostname, systemArchitecture), _counter + 15);

                        //In case of an upgrade scenario, check if the version being install is same as the one on remote machine
                        //In case the two versions are the same, throw an error

                        string RemoteFilePath = @"\\" + _contactable_ip + @"\C$\Program Files\Vision Solutions\Double-Take\" + systemArchitecture + @"\setup.exe";

                        FileVersionInfo remoteFileVersion = null;
                        if (File.Exists(RemoteFilePath))
                        {
                            remoteFileVersion = FileVersionInfo.GetVersionInfo(RemoteFilePath);
                            _mrp_portal.task().progress(payload, String.Format("Double-Take found on {0} : {1}", _working_workload.hostname, remoteFileVersion.ProductVersion), _counter + 16);
                        }
                        else
                        {
                            _mrp_portal.task().progress(payload, string.Format("It's a fresh install; no Double-Take version found on {0}", _working_workload.hostname), _counter + 16);
                        }

                        string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\" + systemArchitecture;
                        string LocalPath = Path.Combine(localConfigFilePath, @"setup.exe");

                        FileVersionInfo localFileVersion;
                        if (File.Exists(LocalPath))
                        {
                            localFileVersion = FileVersionInfo.GetVersionInfo(LocalPath);
                            _mrp_portal.task().progress(payload, String.Format("Double-Take {0} being installed on {1}", localFileVersion.FileVersion, _working_workload.hostname), _counter + 17);
                        }
                        else
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Couldn't locate required install file(s) {0}", LocalPath));
                            return;
                        }
                        if (remoteFileVersion != null)
                        {
                            int versionCompare = Versions.Compare(localFileVersion.ProductVersion, remoteFileVersion.ProductVersion);
                            if (versionCompare <= 0)
                            {
                                _mrp_portal.task().progress(payload, String.Format("Product version being PushInstalled is same or less than the version ({0}) installed on {1}", localFileVersion.ProductVersion, _working_workload.hostname), _counter + 18);
                                ProductVersionModel _installed_dt_version;
                                using (Doubletake _dt = new Doubletake(null, _working_workload.id))
                                {
                                    try
                                    {
                                        _installed_dt_version = (_dt.management().GetProductInfo().Result).ManagementServiceVersion;
                                        _mrp_portal.task().progress(payload, String.Format("Double-Take installed and running on {0} with version {1}.{2}.{3}", _working_workload.hostname, _installed_dt_version.Major, _installed_dt_version.Minor, _installed_dt_version.Build), _counter + 19);

                                    }
                                    catch (Exception ex)
                                    {
                                        _mrp_portal.task().progress(payload, String.Format("Double-Take installed on {0} but cannot be contacted: {1}", _working_workload.hostname, ex.Message), _counter + 19);
                                    }
                                }
                                server_type = dt_server_type.target;
                                continue;
                            }
                        }
                        #endregion

                        #region Copy files process
                        //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
                        _mrp_portal.task().progress(payload, String.Format("Copy binaries to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), _counter + 20);
                        remoteTempLocation = remoteTempLocation.Replace(':', '$');
                        string _target_workload_temp_path = Path.Combine(_contactable_ip, remoteTempLocation);
                        _target_workload_temp_path = @"\\" + _target_workload_temp_path + @"\" + systemArchitecture;
                        var endTime = DateTime.UtcNow.AddMinutes(1);
                        if (!Directory.Exists(_target_workload_temp_path))
                        {
                            Directory.CreateDirectory(_target_workload_temp_path);
                        }
                        string path = @"C:\Program Files\Vision Solutions\Double-Take";

                        string localFilePath = Path.Combine(path, systemArchitecture);
                        localFilePath = Path.Combine(localFilePath, @"setup.exe");
                        if (!File.Exists(localFilePath))
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Couldn't locate required installation file(s) {0}", localFilePath));
                            return;
                        }

                        remoteTempLocation = remoteTempLocation.Replace(':', '$');
                        string setupFileOnWorkload = _target_workload_temp_path + @"\setup.exe";

                        if (File.Exists(setupFileOnWorkload))
                            File.SetAttributes(setupFileOnWorkload, FileAttributes.Normal);

                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        File.Copy(localFilePath, setupFileOnWorkload, true);
                        _mrp_portal.task().progress(payload, String.Format("Complete binaries copy process for {0}", _working_workload.hostname), _counter + 21);

                        _mrp_portal.task().progress(payload, String.Format("Copy configuration file to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), _counter + 25);
                        string configFileOnWorkload = @"\\" + _target_workload_temp_path + @"\DTSetup.ini";
                        File.WriteAllLines(configFileOnWorkload, BuildINI.BuildINIFile(_working_workload.deploymentpolicy, server_type).ConvertAll(Convert.ToString));
                        _mrp_portal.task().progress(payload, String.Format("Complete configuration copy process for {0}", _working_workload.hostname), _counter + 26);

                        #endregion

                        #region Start Remote Installer
                        _mrp_portal.task().progress(payload, String.Format("Starting installer on {0}", _working_workload.hostname), _counter + 30);


                        //Invoke install process on the remote machine
                        remoteTempLocation = remoteTempLocation.Replace('$', ':');
                        var setupPath = Path.Combine(remoteTempLocation, systemArchitecture);

                        //Create connection object to remote machine
                        ConnectionOptions connOptions = new ConnectionOptions();
                        connOptions.Impersonation = ImpersonationLevel.Impersonate;
                        connOptions.Authentication = AuthenticationLevel.Default;
                        connOptions.EnablePrivileges = true;
                        connOptions.Username = (_workload_credentials.domain == null ? "." : _workload_credentials.domain) + @"\" + _workload_credentials.username;
                        connOptions.Password = _workload_credentials.password;

                        //var configPath = @"C:\DTSetup";
                        ManagementScope scope = new ManagementScope(@"\\" + _contactable_ip + @"\root\CIMV2", connOptions);
                        scope.Connect();

                        string installCmd = @"cmd.exe /c " + setupPath + "\\setup.exe /s /v\"DTSETUPINI=\\\"" + setupPath + "\\" + "DTSetup.ini\\\" /qn /l*v+ " + setupPath + "\\Repinst.log";

                        MRMPService.Log.Logger.log(installCmd, Logger.Severity.Info);
                        Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                        installCmdParams["CommandLine"] = installCmd;
                        installCmdParams["CurrentDirectory"] = setupPath;

                        Dictionary<string, object> returnValues = new Dictionary<string, object>();
                        ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                        ObjectGetOptions ogo = new ObjectGetOptions();
                        ManagementBaseObject returnValue;
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
                        int processId = 0;
                        if (returnValues != null)
                        {
                            processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                        }

                        if (processId == 0)
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Process ID not found on {0}", _working_workload.hostname));
                            return;
                        }
                        //Wait for the process to complete
                        _mrp_portal.task().progress(payload, "Wait for remote installer to complete", _counter + 31);

                        DateTime installEndTime = DateTime.UtcNow.AddSeconds(2700);
                        Process process;

                        while (DateTime.UtcNow < installEndTime)
                        {
                            try
                            {
                                process = Process.GetProcessById(processId, _contactable_ip);
                            }
                            catch (Exception)
                            {
                                _mrp_portal.task().progress(payload, "Remote installer to completed", _counter + 31);
                                break;
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                        if (installEndTime <= DateTime.UtcNow)
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Timeout waiting for install process to complete on {0}", _working_workload.hostname));
                            return;
                        }

                        #endregion

                        #region Verify DT Installation
                        _mrp_portal.task().progress(payload, String.Format("Verify DT connectivity on {0}", _working_workload.hostname), _counter + 40);

                        //Verify if the management service of Double-Take is running
                        // to determine that the software is installed properly
                        ProductVersionModel _dt_version;
                        using (Doubletake _dt = new Doubletake(null, _working_workload.id))
                        {
                            _dt_version = (await _dt.management().GetProductInfo()).ManagementServiceVersion;
                            if (_dt_version == null)
                            {
                                _mrp_portal.task().failcomplete(payload, "Cannot determine installed version of Double-Take");
                                server_type = dt_server_type.target;
                                continue;
                            }
                        }
                        _mrp_portal.task().progress(payload, String.Format("Double-Take version {0}.{1}.{2} has successfully installed on workload {3} ", _dt_version.Major, _dt_version.Minor, _dt_version.Build, _working_workload.hostname), _counter + 45);

                        #endregion

                    }
                    //if we done with the target, then we done with the job...
                    if (server_type == dt_server_type.target)
                    {
                        break;
                    }
                    //once we have the source deployed, move to the target
                    server_type = dt_server_type.target;
                }
                _mrp_portal.task().successcomplete(payload, "Completed Double-Take deployment");
            }
            catch (SecurityException ex)
            {
                _mrp_portal.task().failcomplete(payload, string.Format("Permission denied; Cannot access install process on remote machine; {0}", ex.Message));
                Logger.log(string.Format("Permission denied; Cannot access install process on remote machine; {0}", ex.Message), Logger.Severity.Error);
            }
            catch (Exception ex)
            {
                _mrp_portal.task().failcomplete(payload, ex.Message);
                Logger.log(string.Format("Cannot access install process on remote machine; {0}", ex.ToString()), Logger.Severity.Error);
            }
        }








    }
}
