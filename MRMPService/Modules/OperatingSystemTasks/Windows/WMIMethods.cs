using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using Microsoft.Win32;
using MRMPService.Exceptions;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Utilities;
using static MRMPService.Modules.OperatingSystemTasks.Windows.ServiceManagement;
using System.Linq;

namespace MRMPService.Modules.OperatingSystemTasks.Windows
{
    public class WMIMethods
    {
        private MRMPWorkloadBaseType workload;
        private Stopwatch _watch;
        public WMIMethods(MRMPWorkloadBaseType _workload)
        {
            workload = _workload;
        }
        public void RunCommand(string _base_path, string _command)
        {
            try
            {
                Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                installCmdParams["CommandLine"] = _command;
                installCmdParams["CurrentDirectory"] = _base_path;

                ObjectGetOptions ogo = new ObjectGetOptions();
                ManagementBaseObject returnValue;
                using (ManagementClass mc = workload.GetManagementClass("Win32_Process"))
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
                if (returnValue != null)
                {
                    processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                }

                if (processId == 0)
                {
                    throw new MRMPExecuteCommandException(String.Format("Remote command died prematurely on {0}", workload.hostname));
                }
                DateTime installEndTime = DateTime.UtcNow.AddSeconds(MRMPServiceBase.remote_exec_timeout);
                Process process;
                while (DateTime.UtcNow < installEndTime)
                {
                    try
                    {
                        process = Process.GetProcessById(processId, workload.GetContactibleIP());
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
                if (installEndTime <= DateTime.UtcNow)
                {
                    throw ExceptionFactory.MRMPExecuteCommandException(workload.hostname, _command, $"Remote command timed out after {MRMPServiceBase.remote_exec_timeout}");
                }
            }
            catch (Exception _ex)
            {
                throw ExceptionFactory.MRMPExecuteCommandException(_ex, workload.hostname, _command);
            }
        }

        public void CopyLocalToRemoteFile(string _local_file, string _path, string _contactable_ip = null)
        {
            using (new Impersonator(workload.GetCredentials().username, (String.IsNullOrWhiteSpace(workload.GetCredentials().domain) ? "." : workload.GetCredentials().domain), workload.GetCredentials().decrypted_password))
            {
                if (MRMPServiceBase.debug) { _watch = Stopwatch.StartNew(); }

                _contactable_ip = String.IsNullOrEmpty(_contactable_ip) ? workload.GetContactibleIP(true) : _contactable_ip;
                string _remote_location = Path.Combine(@"\\", _contactable_ip, _path.Replace(':', '$'), Path.GetFileName(_local_file));
                if (!Directory.Exists(Path.GetDirectoryName(_remote_location)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_remote_location));
                }

                if (!File.Exists(_local_file))
                {
                    throw new MRMPIOException(String.Format("Couldn't locate local file {0} on manager", _local_file));
                }
                if (!File.Exists(_remote_location))
                {
                    try
                    {
                        File.Copy(_local_file, _remote_location, true);
                    }
                    catch (Exception _ex)
                    {
                        throw ExceptionFactory.MRMPIOException(_ex, _local_file);
                    }
                }
            }
        }
        public void CopyStringArrayToRemoteFile(string[] _string_list, string _remote_file_path, string _contactable_ip = null)
        {
            using (new Impersonator(workload.GetCredentials().username, (String.IsNullOrWhiteSpace(workload.GetCredentials().domain) ? "." : workload.GetCredentials().domain), workload.GetCredentials().decrypted_password))
            {
                _contactable_ip = String.IsNullOrEmpty(_contactable_ip) ? workload.GetContactibleIP(true) : _contactable_ip;
                string _remote_file_location = Path.Combine(@"\\", _contactable_ip, _remote_file_path.Replace(':', '$'));
                if (!Directory.Exists(Path.GetDirectoryName(_remote_file_location)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_remote_file_location));
                }
                try
                {
                    File.WriteAllLines(_remote_file_location, _string_list);
                }
                catch (Exception _ex)
                {
                    throw ExceptionFactory.MRMPIOException(_ex, _remote_file_location);
                }
            }
        }
        public void CopyRemoteToRemoteFile(string _remote_source_file, string _remote_target_path)
        {
            using (new Impersonator(workload.GetCredentials().username, (String.IsNullOrWhiteSpace(workload.GetCredentials().domain) ? "." : workload.GetCredentials().domain), workload.GetCredentials().decrypted_password))
            {
                string _remote_location = Path.Combine(@"\\", workload.GetContactibleIP(true), @"\", _remote_target_path.Replace(':', '$'));
                string _remote_file = Path.Combine(@"\\", workload.GetContactibleIP(true), @"\", _remote_source_file.Replace(':', '$'));
                if (!Directory.Exists(_remote_target_path))
                {
                    Directory.CreateDirectory(_remote_target_path);
                }

                if (!File.Exists(_remote_file))
                {
                    throw ExceptionFactory.MRMPIOFileNotFound(_remote_file);
                }
                if (!File.Exists(_remote_file))
                {
                    try
                    {
                        File.Copy(_remote_file, _remote_location, true);
                    }
                    catch (Exception _ex)
                    {
                        throw ExceptionFactory.MRMPIOException(_ex, _remote_file);
                    }
                }
            }
        }
        public Dictionary<string, object> GetRemoteRegistryKeys(string _subkey, string[] _keys)
        {
            using (new Impersonator(workload.GetCredentials().username, (String.IsNullOrWhiteSpace(workload.GetCredentials().domain) ? "." : workload.GetCredentials().domain), workload.GetCredentials().decrypted_password))
            {

                RegistryKey _dt_rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, workload.GetContactibleIP(true));
                RegistryKey _dt_key = _dt_rk.OpenSubKey(_subkey);
                Dictionary<string, object> _retrieved_values = new Dictionary<string, object>();
                if (_dt_key != null)
                {
                    foreach (var _key in _keys)
                    {
                        try
                        {
                            _retrieved_values[_key] = _dt_key.GetValue(_key);
                        }
                        catch (Exception _ex)
                        {
                            throw ExceptionFactory.MRMPRemoteRegistryException(_ex, workload.GetContactibleIP(true), String.Join(@"\", _subkey, _key));
                        }
                    }
                }
                return _retrieved_values;

            }
        }

        public void CopyLocalToRemoteDirectory(string _local_source_path, string _remote_target_path, bool _recursive, string[] _skip_folders = null)
        {
            using (new Impersonator(workload.GetCredentials().username, (String.IsNullOrWhiteSpace(workload.GetCredentials().domain) ? "." : workload.GetCredentials().domain), workload.GetCredentials().decrypted_password))
            {
                _remote_target_path = Path.Combine(@"\\", workload.GetContactibleIP(true), @"\", _remote_target_path.Replace(':', '$'));
                try
                {
                    DirectoryCopy(_local_source_path, _remote_target_path, _recursive, _skip_folders);
                }
                catch (Exception _ex)
                {
                    throw ExceptionFactory.MRMPIOException(_ex, _remote_target_path);
                }
            }
        }


        public ServiceManagementReturnValue UninstallService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("delete", null, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                    outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceManagementReturnValue.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }
        public void KillProcess(string _process_name)
        {
            var query = new SelectQuery("select * from Win32_process where name = '" + _process_name + "'");
            using (var searcher = new ManagementObjectSearcher(workload.GetManagementScope(), query))
            {
                foreach (ManagementObject process in searcher.Get())
                {
                    process.InvokeMethod("Terminate", null);
                }
            }
        }
        public ServiceManagementReturnValue StartService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("StartService",
                        null, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                    outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceManagementReturnValue.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }

        public ServiceManagementReturnValue StopService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("StopService",
                        null, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                    outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceManagementReturnValue.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }

        public ServiceManagementReturnValue ResumeService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("ResumeService",
                        null, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                        outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceManagementReturnValue.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }

        public ServiceManagementReturnValue PauseService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("PauseService",
                        null, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                        outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceManagementReturnValue.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }

        public ServiceManagementReturnValue ChangeStartMode(string svcName, ServiceManagementStartMode startMode)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                ManagementBaseObject inParams = service.GetMethodParameters("ChangeStartMode");
                inParams["StartMode"] = startMode.ToString();
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("ChangeStartMode",
                        inParams, null);
                    return (ServiceManagementReturnValue)Enum.Parse(typeof(ServiceManagementReturnValue),
                        outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                { throw ex; }
            }
        }

        public bool IsServiceInstalled(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("InterrogateService",
                        null, null);
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return false;
                    else
                        throw ex;
                }
            }
        }

        public ServiceManagementServiceState GetServiceState(string svcName)
        {
            ServiceManagementServiceState toReturn = ServiceManagementServiceState.Stopped;
            string _state = string.Empty;
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    _state = service.Properties["State"].Value.ToString().Trim();
                    switch (_state)
                    {
                        case "Running":
                            toReturn = ServiceManagementServiceState.Running;
                            break;
                        case "Stopped":
                            toReturn = ServiceManagementServiceState.Stopped;
                            break;
                        case "Paused":
                            toReturn = ServiceManagementServiceState.Paused;
                            break;
                        case "Start Pending":
                            toReturn = ServiceManagementServiceState.StartPending;
                            break;
                        case "Stop Pending":
                            toReturn = ServiceManagementServiceState.StopPending;
                            break;
                        case "Continue Pending":
                            toReturn = ServiceManagementServiceState.ContinuePending;
                            break;
                        case "Pause Pending":
                            toReturn = ServiceManagementServiceState.PausePending;
                            break;
                    }
                }
                catch (Exception ex)
                { throw ex; }
            }
            return toReturn;
        }

        public bool CanStop(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    return bool.Parse(service.Properties["AcceptStop"].Value.ToString());
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CanPauseAndContinue(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    return bool.Parse(service.Properties["AcceptPause"].Value.ToString());
                }
                catch
                {
                    return false;
                }
            }
        }

        public int GetProcessId(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    return int.Parse(service.Properties["ProcessId"].Value.ToString());
                }
                catch
                {
                    return 0;
                }
            }
        }

        public string GetPath(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = workload.GetManagementObject(objPath))
            {
                try
                {
                    return service.Properties["PathName"].Value.ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string[] _skip_folders = null)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                {
                    File.SetAttributes(temppath, FileAttributes.Normal);
                }
                file.CopyTo(temppath, true);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (_skip_folders != null)
                    {
                        if (_skip_folders.Any(x => x == subdir.Name))
                        {
                            continue;
                        }
                    }
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
