using LukeSkywalker.IPNetwork;
using MRMPService.Modules.MRMPPortal.Contracts;
using MRMPService.Modules.OperatingSystemTasks.Windows;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.Modules.MRMPPortal.Contracts
{
    public class MRMPWorkloadBaseType : MRMPWorkloadType
    {
        public MRMPWorkloadBaseType()
        {

        }
        public WMIMethods WMIMethods { get { return new WMIMethods(this); } }

        public string GetContactibleIP(bool literal = false)
        {
            string _contatible_ip = null;
            String[] _iplist = new string[] { };

            if (String.IsNullOrEmpty(this.iplist))
            {
                throw new ArgumentNullException(String.Format("No IP list defined for workload"));
            }
            _iplist = this.iplist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (string ip in _iplist)
                {
                    PingReply _test_ping_reply = null;
                    Ping _test_ping = new Ping();

                    int retry = 3;
                    while (retry-- > 0)
                    {
                        try
                        {
                            _test_ping_reply = _test_ping.Send(ip);
                            if (_test_ping_reply?.Status == IPStatus.Success)
                            {
                                _contatible_ip = ip;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(String.Format("Error during ping attempt : {0}", ex.GetBaseException().Message));
                        }
                    }
                    if (_test_ping_reply?.Status == IPStatus.Success)
                    {
                        break;
                    }
                }
                if (literal == true)
                {
                    IPAddress _check_ip;
                    if (IPAddress.TryParse(_contatible_ip, out _check_ip))
                    {
                        if (_check_ip.AddressFamily.ToString() == AddressFamily.InterNetworkV6.ToString())
                        {
                            String _workingip = _contatible_ip;
                            _workingip = _workingip.Replace(":", "-");
                            _workingip = _workingip.Replace("%", "s");
                            _workingip = _workingip + ".ipv6-literal.net";
                            _contatible_ip = _workingip;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unknown error trying to contact workload : {1}", ex.GetBaseException().Message));
            }
            finally
            {
                if (_contatible_ip == null)
                {
                    throw new Exception(String.Format("Does not respond to ping after 3 attempts"));
                }
            }
            return _contatible_ip;
        }
        public MRPCredentialType GetCredentials()
        {
            if (this.credential == null)
            {
                throw new ArgumentNullException(String.Format("Credential object for {0} is empty", this.hostname));
            }
            return credential;
        }
        public string GetFullCredentialUsername()
        {
            return String.Format(@"{0}\{1}", (String.IsNullOrEmpty(this.GetCredentials().domain) ? "." : this.GetCredentials().domain), this.GetCredentials().username);
        }
        public string GetCredentialPassword()
        {
            return this.GetCredentials().decrypted_password;
        }
        public string GetCredentialUsername()
        {
            return this.GetCredentials().username;
        }
        public string GetCredentialDomain()
        {
            return (String.IsNullOrEmpty(this.GetCredentials().domain) ? "." : this.GetCredentials().domain);
        }
        public ManagementScope GetManagementScope()
        {
            return new WMIHelper().ConnectionScope(GetContactibleIP(true), new WMIHelper().ProcessConnectionOptions(GetFullCredentialUsername(), GetCredentials().decrypted_password));
        }
        public ManagementClass GetManagementClass(String _managementPath, ObjectGetOptions _get_options = null)
        {
            return new ManagementClass(this.GetManagementScope(), new ManagementPath(_managementPath), _get_options);
        }
        public ManagementObject GetManagementObject(String _managementPath, ObjectGetOptions _get_options = null)
        {
            return new ManagementObject(this.GetManagementScope(), new ManagementPath(_managementPath), _get_options);
        }
        public ManagementObjectSearcher GetManagementObjectSearcher(SelectQuery _query)
        {
            return new ManagementObjectSearcher(this.GetManagementScope(), _query);
        }
        public ManagementEventWatcher GetManagementEventWatcher(WqlEventQuery _query)
        {
            return new ManagementEventWatcher(this.GetManagementScope(), _query);
        }

        public void ExecuteProcessWMI(string _workingpath, string _arguments)
        {
            try
            {
                using (ManagementClass processClass = GetManagementClass("Win32_Process"))
                {
                    using (ManagementBaseObject inParams = processClass.GetMethodParameters("Create"))
                    {
                        inParams["CommandLine"] = _arguments;
                        inParams["CurrentDirectory"] = _workingpath;
                        using (ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, new InvokeMethodOptions() { Timeout = new TimeSpan(0, 0, MRMPServiceBase.remote_exec_timeout) }))
                        {
                            if ((uint)outParams["ReturnValue"] != 0)
                            {
                                throw new Exception("Error while starting process " + _arguments + " creation returned an exit code of " + outParams["returnValue"] + ". It was launched as " + this.GetFullCredentialUsername() + " on " + this.hostname);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Execute process failed Workload {0}, Process {1}, RunAs {2}, Error is {3}, Stack trace {4}", this.hostname, _arguments, this.GetFullCredentialUsername(), e.Message, e.StackTrace), e);
            }
        }
        public void WorkloadCustomizationWMI()
        {
            long _c_volume_actual_size = 0;
            long _c_volume_actual_free = 0;
            string _cdrom_drive_letter = "";
            SelectQuery VolumeQuery = new SelectQuery("SELECT Size, FreeSpace FROM Win32_LogicalDisk WHERE DeviceID = 'C:'");
            SelectQuery CdromQuery = new SelectQuery("SELECT Drive FROM Win32_CDROMDrive");
            foreach (var item in this.GetManagementObjectSearcher(CdromQuery).Get())
            {
                try
                {
                    _cdrom_drive_letter = item["Drive"].ToString();
                }
                catch (Exception)
                {

                }
            }
            foreach (var item in GetManagementObjectSearcher(VolumeQuery).Get())
            {
                try
                {
                    _c_volume_actual_size = Convert.ToInt64(item["Size"].ToString());
                    _c_volume_actual_free = Convert.ToInt64(item["FreeSpace"].ToString());
                }
                catch (Exception)
                {
                    throw new ArgumentException(String.Format("Error collecting C: volume space information for {0}", this.hostname));
                }
            }


            MRPWorkloadVolumeType _c_volume_object = this.workloadvolumes.FirstOrDefault(x => x.driveletter == "C");
            long _c_volume_to_add = 0;
            if (_c_volume_object != null)
            {
                _c_volume_to_add = (long)(_c_volume_object.volumesize * 1024 * 1024 * 1024) - _c_volume_actual_size;
            }
            else
            {
                throw new ArgumentException("Cannot find C: drive in volume list for partition mapping");
            }


            List<String> _used_drive_letters = this.workloadvolumes.Select(x => x.driveletter.Substring(0, 1)).ToList();
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
            foreach (int _disk_index in this.workloadvolumes.Select(x => x.diskindex).Distinct())
            {
                if (_disk_index != 0)
                {
                    _diskpart_struct.Add(String.Format("select disk {0}", _disk_index));
                    _diskpart_struct.Add("clean");
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
                foreach (MRPWorkloadVolumeType _volume in this.workloadvolumes.ToList().Where(x => x.diskindex == _disk_index && !x.driveletter.Contains("C")).OrderBy(x => x.driveletter))
                {
                    string _driveletter = _volume.driveletter.Substring(0, 1);

                    _diskpart_struct.Add(String.Format("select disk {0}", _disk_index));
                    _diskpart_struct.Add(String.Format("create partition primary size={0} noerr", _volume.volumesize * 1024));
                    _diskpart_struct.Add("format fs=ntfs quick");
                    _diskpart_struct.Add(String.Format("assign letter={0} noerr", _driveletter));
                    _diskpart_struct.Add("");
                    _vol_index++;
                }
            }
            string[] diskpart_bat_content = new String[] { @"C:\Windows\System32\diskpart.exe /s C:\diskpart.txt > C:\diskpart.log" };
            string _local_location = @"C:\";
            string _remote_location = _local_location.Replace(':', '$');
            string _diskpart_remote_commands = @"\\" + Path.Combine(GetContactibleIP(true), _remote_location, "diskpart.txt");
            string _diskpart_remote_bat = @"\\" + Path.Combine(GetContactibleIP(true), _remote_location, "diskpart.bat");
            CopyRemoteFileWindows(_diskpart_remote_commands, _diskpart_struct.ToArray());
            CopyRemoteFileWindows(_diskpart_remote_bat, diskpart_bat_content);
            ExecuteProcessWMI(_local_location, @"C:\diskpart.bat");
            if (this.workloadinterfaces.Where(x => x.vnic != 0).Count() > 0)
            {
                SetInterfaceAddressesWMI();
            }
        }
        public void SetInterfaceAddressesWMI()
        {
            ManagementClass objMC = GetManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            //exclude vnic 0 from 
            foreach (MRPWorkloadInterfaceType _interface in this.workloadinterfaces.Where(x => x.vnic != 0))
            {
                bool _found_interface = false;
                foreach (ManagementObject objMO in objMOC)
                {
                    if ((bool)objMO["IPEnabled"])
                    {
                        var _workload_nix_mac = objMO["MACAddress"].ToString().ToLower();
                        if (_workload_nix_mac == _interface.macaddress.ToLower())
                        {
                            _found_interface = true;
                            try
                            {
                                int _nic_index = Convert.ToInt16(objMO["InterfaceIndex"].ToString());

                                IPNetwork _ipv4network = IPNetwork.Parse(_interface.ipaddress, Convert.ToByte(_interface.platformnetwork.ipv4netmask));

                                if (!String.IsNullOrEmpty(_interface.ipaddress))
                                {
                                    this.ExecuteProcessWMI(@"C:\Windows\System32", String.Format("netsh interface ip set address name={0} static  {1} {2}", _nic_index, _interface.ipaddress, _ipv4network.Netmask));
                                }
                                if (!String.IsNullOrEmpty(_interface.ipv6address))
                                {
                                    this.ExecuteProcessWMI(@"C:\Windows\System32", String.Format("netsh interface ipv6 set address interface={0} {1}/{2} ", _nic_index, _interface.ipv6address, _interface.platformnetwork.ipv6netmask));
                                    //this.ExecuteProcessWMI(@"C:\Windows\System32", String.Format("netsh interface ipv6 add route interface={0} ::/0 {1} ", _nic_index, _interface.platformnetwork.ipv6gateway));

                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(String.Format("Error updating workload network information: {0}", ex.GetBaseException().Message));
                            }
                        }
                    }
                }
                if (!_found_interface)
                {
                    throw new Exception(String.Format("Error finding interface {0} : {1}", _interface.ipaddress, _interface.macaddress));
                }
            }
        }
        public List<string> ReadRemoteTXTWindows(string _path)
        {
            using (new Impersonator(this.GetCredentialUsername(), this.GetCredentialDomain(), this.GetCredentials().decrypted_password))
            {
                return new List<string>(File.ReadAllLines(_path));
            }
        }
        public void CopyRemoteFileWindows(string _local_path, string _remote_path)
        {
            using (new Impersonator(this.GetCredentialUsername(), this.GetCredentialDomain(), this.GetCredentials().decrypted_password))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_remote_path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_remote_path));
                }
                int _copy_retries = 0;
                while (true)
                {
                    try
                    {
                        File.Copy(_local_path, _remote_path, true);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (++_copy_retries > MRMPServiceBase.remote_file_create_retries)
                        {
                            Logger.log(String.Format("Error copying file to workload {0} : {1} : {2}", this.hostname, ex.Message, _remote_path), Logger.Severity.Info);
                            throw new Exception(String.Format("Error copying file to workload: {0}", ex.Message));
                        }
                        else Thread.Sleep(new TimeSpan(0, 0, MRMPServiceBase.remote_file_create_retrywait));
                    }
                }
            }
        }
        public void CopyRemoteFileWindows(string _remote_path, string[] _string_list)
        {
            using (new Impersonator(this.GetCredentialUsername(), this.GetCredentialDomain(), this.GetCredentials().decrypted_password))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_remote_path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_remote_path));
                }
                int _copy_retries = 0;
                while (true)
                {
                    try
                    {
                        File.WriteAllLines(_remote_path, _string_list);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (++_copy_retries > MRMPServiceBase.remote_file_create_retries)
                        {
                            Logger.log(String.Format("Error copying file to workload {0} : {1} : {2}", this.hostname, ex.Message, _remote_path), Logger.Severity.Info);
                            throw new Exception(String.Format("Error copying file to workload: {0}", ex.Message));
                        }
                        else Thread.Sleep(new TimeSpan(0, 0, MRMPServiceBase.remote_file_create_retrywait));
                    }
                }
            }
        }


    }
}
