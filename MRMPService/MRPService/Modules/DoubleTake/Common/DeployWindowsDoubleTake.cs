using Microsoft.Win32;
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
using System.Net.Sockets;
using System.Net;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
    {
        public static void DeployWindowsDoubleTake(MRPTaskType _task, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            dt_server_type server_type = dt_server_type.target;
            FileVersionInfo localFileVersion;
            string localFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Double-Take", @"Windows", @"setup.exe");
            try
            {
                localFileVersion = FileVersionInfo.GetVersionInfo(localFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error finding datamover installation file on Manager {0}", ex.GetBaseException().Message));
            }

            MRPWorkloadType _working_workload = new MRPWorkloadType();
            int _counter = 0;
            while (true)
            {
                bool _install_dm = false;
                switch (server_type)
                {
                    case dt_server_type.target:
                        _working_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_target_workload.id);
                        _counter = 1;
                        break;
                    case dt_server_type.source:
                        _working_workload = MRMPServiceBase._mrmp_api.workload().get_by_id(_source_workload.id);
                        _counter = 50;
                        break;

                }

                _task.progress(String.Format("Starting datamover deploying process on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 5));
                string _contactable_ip = _working_workload.working_ipaddress(true);
                string remoteTempLocation = _working_workload.deploymentpolicy.dt_windows_temppath;
                if (_working_workload.get_credential == null)
                {
                    throw new Exception(String.Format("Cannot determine credentials for {0}", _working_workload.hostname));
                }


                //Determine if the setup to be installed is 32 bit or 64 bit
                string _environment_reg_key = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                string _dt_reg_key = @"SOFTWARE\NSI Software\Double-Take\CurrentVersion";

                using (new Impersonator(_working_workload.get_credential.username, (String.IsNullOrWhiteSpace(_working_workload.get_credential.domain) ? "." : _working_workload.get_credential.domain), _working_workload.get_credential.decrypted_password))
                {

                    _task.progress("Confirming remote CPU architecture", ReportProgress.Progress(_start_progress, _end_progress, _counter + 6));
                    string systemArchitecture = "X64";
                    try
                    {
                        #region Detect Target Architecture
                        RegistryKey rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _contactable_ip);
                        RegistryKey key = rk.OpenSubKey(_environment_reg_key);
                        if (key != null)
                        {
                            string architecture = (string)key.GetValue("PROCESSOR_ARCHITECTURE");
                            if (!architecture.Contains("64"))
                            {
                                throw new Exception(String.Format("32Bit workloads not supported : {0}", _working_workload.hostname));
                            }
                        }
                        else
                        {
                            throw new Exception(String.Format("Cannot connect to registry on {0}", _working_workload.hostname));

                        }
                        _task.progress(String.Format("{0} is of type {1} architecture", _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 7));
                    }
                    catch (Exception _reg_exeception)
                    {
                        throw new Exception(String.Format("Cannot connect to registry on {0} : {1}", _working_workload.hostname, _reg_exeception.GetBaseException().Message));
                    }


                    try
                    {
                        RegistryKey _dt_rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _contactable_ip);
                        RegistryKey _dt_key = _dt_rk.OpenSubKey(_dt_reg_key);
                        if (_dt_key != null)
                        {
                            string _dt_installed_version = (string)_dt_key.GetValue("InstallVersionInfo");
                            string _dt_istalled_path = (string)_dt_key.GetValue("InstallPath");
                            if (_dt_installed_version == null || _dt_istalled_path == null)
                            {
                                _install_dm = true;
                                _task.progress(String.Format("Datamover not found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                            }
                            else
                            {
                                _task.progress(String.Format("Datamover Found on {0} : {1} [{2}] ", _working_workload.hostname, _dt_installed_version, _dt_istalled_path), ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));
                                if (Versions.Compare(localFileVersion.ProductVersion, _dt_installed_version) < 0)
                                {
                                    _install_dm = true;
                                    _task.progress(String.Format("Uprading datamover from {0} to {1} on {2}", _dt_installed_version, localFileVersion.ProductVersion, _working_workload.hostname, _dt_installed_version, _dt_istalled_path), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                                }
                                else
                                {
                                    _task.progress(String.Format("Testing datamover connectivity on {0} using {1}", _working_workload.hostname, _contactable_ip), ReportProgress.Progress(_start_progress, _end_progress, _counter + 14));
                                    ProductVersionModel _installed_dt_version;
                                    using (Doubletake _dt = new Doubletake(null, _working_workload))
                                    {
                                        try
                                        {
                                            _installed_dt_version = (_dt.management().GetProductInfo()).ManagementServiceVersion;
                                            _task.progress(String.Format("Datamover installed and running on {0} with version {1}", _working_workload.hostname, _dt.management().GetProductVersion(_installed_dt_version)), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));
                                        }
                                        catch (Exception ex)
                                        {
                                            _install_dm = true;
                                            _task.progress(String.Format("Datamover installed on {0} but cannot be contacted: {1} : {2}", _working_workload.hostname, ex.Message, _working_workload, _contactable_ip), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _install_dm = true;
                            _task.progress(String.Format("Datamover not found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));
                        }
                    }
                    catch (Exception _reg_exeception)
                    {
                        throw new Exception(String.Format("Cannot connect to registry on {0} : {1}", _working_workload.hostname, _reg_exeception.GetBaseException().Message));
                    }

                    #endregion
                    if (_install_dm)
                    {

                        #region Copy files process
                        //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
                        _task.progress(String.Format("Copy installation file to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 20));
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
                        _task.progress(String.Format("Complete binaries copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 21));

                        _task.progress(String.Format("Copy configuration file to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), ReportProgress.Progress(_start_progress, _end_progress, _counter + 25));
                        string configFileOnWorkload = @"\\" + _target_workload_temp_path + @"\DTSetup.ini";
                        try
                        {
                            File.WriteAllLines(configFileOnWorkload, BuildINI.BuildINIFile(_working_workload.deploymentpolicy, server_type).ConvertAll(Convert.ToString));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error creating setup file {0} to workload {1} : {2}", configFileOnWorkload, _working_workload.hostname, ex.GetBaseException().Message));
                        }
                        _task.progress(String.Format("Complete configuration copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));

                        #endregion

                        #region Start Remote Installer
                        _task.progress(String.Format("Starting installer on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 30));

                        //Invoke install process on the remote machine
                        remoteTempLocation = remoteTempLocation.Replace('$', ':');
                        var setupPath = Path.Combine(remoteTempLocation, systemArchitecture);

                        //Create connection object to remote machine
                        ConnectionOptions connOptions = new ConnectionOptions();
                        connOptions.Impersonation = ImpersonationLevel.Impersonate;
                        connOptions.Authentication = AuthenticationLevel.Default;
                        connOptions.EnablePrivileges = true;
                        connOptions.Username = (_working_workload.get_credential.domain == null ? "." : _working_workload.get_credential.domain) + @"\" + _working_workload.get_credential.username;
                        connOptions.Password = _working_workload.get_credential.decrypted_password;

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
                            _task.failcomplete(String.Format("Process ID not found on {0}", _working_workload.hostname));
                            return;
                        }
                        //Wait for the process to complete
                        _task.progress("Wait for remote installer to complete", ReportProgress.Progress(_start_progress, _end_progress, _counter + 31));

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
                                _task.progress("Remote installer to completed", ReportProgress.Progress(_start_progress, _end_progress, _counter + 32));
                                break;
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                        if (installEndTime <= DateTime.UtcNow)
                        {
                            _task.failcomplete(String.Format("Timeout waiting for install process to complete on {0}", _working_workload.hostname));
                            return;
                        }

                        #endregion

                        #region Verify DT Installation
                        _task.progress(String.Format("Verify DT connectivity on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 40));

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
                                    _task.progress(String.Format("Datamover {0} found on {1}", _dt.management().GetProductVersion(_dt_version), _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 45));
                                    break;
                                }
                                catch (Exception)
                                {
                                    if (restries++ >= 3)
                                    {
                                        throw new Exception(String.Format("Cannot determine installed version of datamover on {0}", _working_workload.hostname));
                                    }
                                }
                                Thread.Sleep(new TimeSpan(0, 0, 10));
                                _task.progress(String.Format("Verify datamover connectivity on {0} [{1}]", _working_workload.hostname, restries), ReportProgress.Progress(_start_progress, _end_progress, _counter + 41 + restries));
                            }

                        }

                        #endregion
                    }
                    if (server_type == dt_server_type.source)
                    {
                        break;
                    }
                    else
                    {
                        server_type = dt_server_type.source;
                    }

                }
            }
            _task.progress("Completed Double-Take deployment", ReportProgress.Progress(_start_progress, _end_progress, _counter + 47));
        }
    }
}
