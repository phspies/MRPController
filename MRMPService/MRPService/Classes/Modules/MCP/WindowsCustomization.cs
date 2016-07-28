﻿using DD.CBU.Compute.Api.Contracts.Network20;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPService.Types.API;
using MRMPService.MRMPAPI.Types.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using MRMPService.Utilities;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        static public void WindowsCustomization(MRPTaskType payload, ServerType _newvm)
        {
            MRPTaskSubmitpayloadType _payload = payload.submitpayload;
            MRPPlatformType _platform = _payload.platform;
            MRPWorkloadType _target_workload = _payload.target;
            MRPCredentialType _stadalone_credential = _target_workload.credential;
            MRPCredentialType _platform_credentail = _platform.credential;

            string new_workload_ip = null;
            using (Connection _connection = new Connection())
            {
                new_workload_ip = _connection.FindConnection(String.Join(",", _newvm.networkInfo.primaryNic.ipv6, _newvm.networkInfo.primaryNic.privateIpv4), true);
            }
            if (new_workload_ip == null)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(payload, String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                    throw new ArgumentException(String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                }
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
                        throw new ArgumentException(String.Format("Error collecting C: volume space information for {0}", _newvm.name));

                    }
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
                    throw new ArgumentException("Cannot find C: drive in volume list for partition mapping");
                }
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
                                    throw new ArgumentException(String.Format("Error creating disk layout file on workload: {0}", ex.Message));
                                }
                            }
                            else Thread.Sleep(new TimeSpan(0, 0, 30));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(ex.Message, Logger.Severity.Error);
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(payload, String.Format("Error impersonating Administrator user: {0}", ex.Message));
                    throw new ArgumentException(String.Format("Error impersonating Administrator user: {0}", ex.Message));
                }
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
                            throw new ArgumentException(String.Format("Error running diskpart on workload: {0}", ex.Message));
                        }
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
                    throw new ArgumentException(String.Format("Failed diskpart process on {0} ({1})", _newvm.name, _exitcode));
                }
            }
            else
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().progress(payload, String.Format("Volume setup process exit code: {0}", _exitcode), 81);
                }
            }
        }
    }
}
