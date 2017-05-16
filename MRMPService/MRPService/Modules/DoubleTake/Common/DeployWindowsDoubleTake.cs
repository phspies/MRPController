﻿using Microsoft.Win32;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using MRMPService.Utilities;
using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;
using MRMPService.Modules.MRMPPortal.Contracts;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void DeployWindowsDoubleTake(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            dt_server_type server_type = dt_server_type.source;
            FileVersionInfo localFileVersion;
            string localFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Double-Take", @"Windows", @"setup.exe");
            try
            {
                localFileVersion = FileVersionInfo.GetVersionInfo(localFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error reading DT installation information on Manager {0}", ex.GetBaseException().Message));
            }

            MRPWorkloadType _working_workload = new MRPWorkloadType();
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

                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Starting datamover deploying process on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 5));
                string _contactable_ip = null;
                using (Connection _connection = new Connection())
                {
                    _contactable_ip = _connection.FindConnection(_working_workload.iplist, true);
                }
                if (_contactable_ip == null)
                {
                    throw new Exception(String.Format("Cannot contact workload {0}", _working_workload.hostname));
                }
                string remoteTempLocation = _working_workload.deploymentpolicy.dt_windows_temppath;
                if (_working_workload.credential == null)
                {
                    throw new Exception(String.Format("Cannot determine credentials for {0}", _working_workload.hostname));
                }


                MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Check datamover availability {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 6));
                using (Doubletake _dt = new Doubletake(null, _working_workload))
                {
                    try
                    {
                        var _dt_version = _dt.management().GetProductInfo().ManagementServiceVersion;
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Datamover found on {0} [{1}]", _working_workload.hostname, _dt.management().GetProductVersion(_dt_version)), ReportProgress.Progress(_start_progress, _end_progress, _counter + 7));
                        if (server_type == dt_server_type.target)
                        {
                            break;
                        }
                        else
                        {
                            server_type = dt_server_type.target;
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Datamover not contatable on {0}. Proceeding with fresh install", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 8));
                    }

                }
                MRMPServiceBase._mrmp_api.task().progress(_task_id, "Get remote architecture", ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));
                string systemArchitecture = null;
                //Determine if the setup to be installed is 32 bit or 64 bit
                string keyString = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                using (new Impersonator(_working_workload.credential.username, (String.IsNullOrWhiteSpace(_working_workload.credential.domain) ? "." : _working_workload.credential.domain), _working_workload.credential.encrypted_password))
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
                            MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("32Bit Workloads not supported {0} ", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));
                            throw new Exception(String.Format("32Bit workloads not supported {0}", _working_workload.hostname));
                        }
                    }
                    else
                    {
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Cannot determine remote achitecture for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                        server_type = dt_server_type.target;
                        continue;
                    }
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("{0} is of type {1} architecture", _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));

                    //In case of an upgrade scenario, check if the version being install is same as the one on remote machine
                    //In case the two versions are the same, throw an error

                    string RemoteFilePath = @"\\" + _contactable_ip + @"\C$\Program Files\Vision Solutions\Double-Take\" + systemArchitecture + @"\setup.exe";

                    FileVersionInfo remoteFileVersion = null;
                    if (File.Exists(RemoteFilePath))
                    {

                        int filesccess_restries = 3;
                        try
                        {
                            remoteFileVersion = FileVersionInfo.GetVersionInfo(RemoteFilePath);
                            MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Datamover found on {0} : {1}", _working_workload.hostname, remoteFileVersion.ProductVersion), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                        }
                        catch (Exception ex)
                        {
                            if (filesccess_restries-- == 0)
                            {
                                throw new Exception(String.Format("Cannot connect to remote workload {0} : {1}", _working_workload.hostname, ex.GetBaseException().Message));
                            }
                        }
                        if (server_type == dt_server_type.source)
                        {
                            if (remoteFileVersion.FileVersion == localFileVersion.FileVersion)
                            {
                                server_type = dt_server_type.target;
                                continue;
                            }
                            else
                            {
                                MRMPServiceBase._mrmp_api.task().progress(_task_id, string.Format("Upgrading DT on {0} to {1}", _working_workload.hostname, localFileVersion.FileVersion), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                            }
                        }
                        else if (server_type == dt_server_type.target)
                        {

                            if (remoteFileVersion.FileVersion == localFileVersion.FileVersion)
                            {
                                break;
                            }
                            else
                            {
                                MRMPServiceBase._mrmp_api.task().progress(_task_id, string.Format("Upgrading DT on {0} to {1}", _working_workload.hostname, localFileVersion.FileVersion), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                            }
                        }
                    }
                    else
                    {
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, string.Format("It's a fresh install; no Double-Take version found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                    }

                    if (File.Exists(localFilePath))
                    {
                        localFileVersion = FileVersionInfo.GetVersionInfo(localFilePath);
                        MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Double-Take {0} being installed on {1}", localFileVersion.FileVersion, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 17));
                    }
                    else
                    {
                        MRMPServiceBase._mrmp_api.task().failcomplete(_task_id, String.Format("Couldn't locate required install file(s) {0}", localFilePath));
                        throw new Exception(String.Format("Couldn't locate required install file(s) {0}", localFilePath));
                    }
                    if (remoteFileVersion != null)
                    {
                        int versionCompare = Versions.Compare(localFileVersion.ProductVersion, remoteFileVersion.ProductVersion);
                        if (versionCompare <= 0)
                        {
                            MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Product version being PushInstalled is same or less than the version ({0}) installed on {1}", localFileVersion.ProductVersion, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 18));
                            ProductVersionModel _installed_dt_version;
                            using (Doubletake _dt = new Doubletake(null, _working_workload))
                            {
                                try
                                {
                                    _installed_dt_version = (_dt.management().GetProductInfo()).ManagementServiceVersion;
                                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Double-Take installed and running on {0} with version {1}", _working_workload.hostname, _dt.management().GetProductVersion(_installed_dt_version)), ReportProgress.Progress(_start_progress, _end_progress, _counter + 19));
                                }
                                catch (Exception ex)
                                {
                                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Double-Take installed on {0} but cannot be contacted: {1}", _working_workload.hostname, ex.Message), ReportProgress.Progress(_start_progress, _end_progress, _counter + 19));
                                }
                            }
                            if (server_type == dt_server_type.target)
                            {
                                break;
                            }
                            else if (server_type == dt_server_type.source)
                            {
                                server_type = dt_server_type.target;
                                continue;
                            }
                        }
                    }
                    #endregion

                    #region Copy files process
                    //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Copy binaries to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 20));
                    remoteTempLocation = remoteTempLocation.Replace(':', '$');
                    string _target_workload_temp_path = Path.Combine(_contactable_ip, remoteTempLocation);
                    _target_workload_temp_path = @"\\" + _target_workload_temp_path + @"\" + systemArchitecture;
                    var endTime = DateTime.UtcNow.AddMinutes(1);
                    if (!Directory.Exists(_target_workload_temp_path))
                    {
                        Directory.CreateDirectory(_target_workload_temp_path);
                    }

                    if (!File.Exists(localFilePath))
                    {
                        MRMPServiceBase._mrmp_api.task().failcomplete(_task_id, String.Format("Couldn't locate required installation file(s) {0}", localFilePath));
                        throw new Exception(String.Format("Couldn't locate required installation file(s) {0}", localFilePath));
                    }

                    remoteTempLocation = remoteTempLocation.Replace(':', '$');
                    string setupFileOnWorkload = _target_workload_temp_path + @"\setup.exe";

                    if (File.Exists(setupFileOnWorkload))
                        File.SetAttributes(setupFileOnWorkload, FileAttributes.Normal);

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    try
                    {
                        File.Copy(localFilePath, setupFileOnWorkload, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("Error copying setup file {0} to workload {1} : {2}", localFilePath, _working_workload.hostname, ex.GetBaseException().Message));
                    }
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Complete binaries copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 21));

                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Copy configuration file to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 25));
                    string configFileOnWorkload = @"\\" + _target_workload_temp_path + @"\DTSetup.ini";
                    try
                    {
                        File.WriteAllLines(configFileOnWorkload, BuildINI.BuildINIFile(_working_workload.deploymentpolicy, server_type).ConvertAll(Convert.ToString));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("Error creating setup file {0} to workload {1} : {2}", configFileOnWorkload, _working_workload.hostname, ex.GetBaseException().Message));
                    }
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Complete configuration copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));

                    #endregion

                    #region Start Remote Installer
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Starting installer on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 30));

                    //Invoke install process on the remote machine
                    remoteTempLocation = remoteTempLocation.Replace('$', ':');
                    var setupPath = Path.Combine(remoteTempLocation, systemArchitecture);

                    //Create connection object to remote machine
                    ConnectionOptions connOptions = new ConnectionOptions();
                    connOptions.Impersonation = ImpersonationLevel.Impersonate;
                    connOptions.Authentication = AuthenticationLevel.Default;
                    connOptions.EnablePrivileges = true;
                    connOptions.Username = (_working_workload.credential.domain == null ? "." : _working_workload.credential.domain) + @"\" + _working_workload.credential.username;
                    connOptions.Password = _working_workload.credential.encrypted_password;

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
                        MRMPServiceBase._mrmp_api.task().failcomplete(_task_id, String.Format("Process ID not found on {0}", _working_workload.hostname));
                        return;
                    }
                    //Wait for the process to complete
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, "Wait for remote installer to complete", ReportProgress.Progress(_start_progress, _end_progress, _counter + 31));

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
                            MRMPServiceBase._mrmp_api.task().progress(_task_id, "Remote installer to completed", ReportProgress.Progress(_start_progress, _end_progress, _counter + 32));
                            break;
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                    if (installEndTime <= DateTime.UtcNow)
                    {
                        MRMPServiceBase._mrmp_api.task().failcomplete(_task_id, String.Format("Timeout waiting for install process to complete on {0}", _working_workload.hostname));
                        return;
                    }

                    #endregion

                    #region Verify DT Installation
                    MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Verify DT connectivity on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 40));

                    //Verify if the management service of Double-Take is running
                    // to determine that the software is installed properly
                    ProductVersionModel _dt_version;
                    int restries = 0;
                    using (Doubletake _dt = new Doubletake(null, _working_workload))
                    {
                        while (true)
                        {
                            try
                            {
                                _dt_version = _dt.management().GetProductInfo().ManagementServiceVersion;
                                break;
                            }
                            catch (Exception)
                            {
                                if (restries++ >= 3)
                                {
                                    throw new Exception("Cannot determine installed version of Double-Take");
                                }
                            }
                            Thread.Sleep(new TimeSpan(0, 0, 10));
                            MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Verify DT connectivity on {0} [{1}]", _working_workload.hostname, restries), ReportProgress.Progress(_start_progress, _end_progress, _counter + 41 + restries));
                        }
                        if (server_type == dt_server_type.target)
                        {
                            break;
                        }
                        else
                        {
                            server_type = dt_server_type.target;
                            continue;
                        }
                    }

                    #endregion

                }
            }
            MRMPServiceBase._mrmp_api.task().progress(_task_id, "Completed Double-Take deployment", ReportProgress.Progress(_start_progress, _end_progress, _counter + 47));
        }
    }
}
