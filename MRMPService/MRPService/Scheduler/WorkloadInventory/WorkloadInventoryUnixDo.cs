using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.IO;
using System.Linq;
using MRMPService.Utilities;
using MRMPService.MRMPService.Log;
using System.Collections.Generic;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadInventory
    {
        static string _password;
        static private void HandleKeyEvent(object sender, AuthenticationPromptEventArgs e)
        {
            foreach (AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = _password;
                }
            }
        }
        static public void WorkloadInventoryLinuxDo(MRPWorkloadType _workload)
        {

            MRPWorkloadType _updated_workload = new MRPWorkloadType()
            {
                id = _workload.id,
                workloadinterfaces = _workload.workloadinterfaces,
                workloadvolumes = _workload.workloadvolumes,
                workloaddisks = _workload.workloaddisks
            };
            _updated_workload.workloaddisks.ForEach(x => x.deleted = true);
            _updated_workload.workloadvolumes.ForEach(x => x.deleted = true);
            _updated_workload.workloadinterfaces.ForEach(x => x.deleted = true);

            //check for credentials
            MRPCredentialType _credential = _workload.get_credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }
            _password = _credential.decrypted_password;
            string workload_ip = _workload.working_ipaddress(false);
            Logger.log(String.Format("Inventory: Started inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            //string _privateSshKeyLocation = "";
            //string _privateSshKeyPhrase = "";
            //var authenticationMethod = new PrivateKeyConnectionInfo(workload_ip, _credential.username, new PrivateKeyFile[] { new PrivateKeyFile(_privateSshKeyLocation, _privateSshKeyPhrase) });

            KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_credential.username);
            _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
            PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_credential.username, _password);
            ConnectionInfo conInfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });

            string locationlocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string localscripts = Path.Combine(locationlocation, "Scripts");

            string remotepath = @"mrmp";
            try
            {
                using (var sftp = new SftpClient(conInfo))
                {
                    sftp.Connect();
                    try
                    {
                        sftp.ChangeDirectory(remotepath);
                    }
                    catch (SftpPathNotFoundException)
                    {
                        sftp.CreateDirectory(remotepath);
                        sftp.ChangeDirectory(remotepath);

                        //setup collector as it is a new installation
                        foreach (String _file in Directory.GetFiles(localscripts))
                        {
                            FileStream fileStream = null;
                            int _retries = 10;
                            while (true)
                            {
                                try
                                {
                                    fileStream = new FileStream(_file, FileMode.Open, FileAccess.Read, FileShare.Read);
                                }
                                catch (IOException e)
                                {
                                    if (e.HResult == 32) // 32 = Sharing violation
                                    {
                                        if (_retries-- == 0)
                                        {
                                            throw new Exception(String.Format("Cannot open file for read {0} : {1}", _file, e.Message));
                                        }
                                        Thread.Sleep(new TimeSpan(0, 0, 5));
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception(String.Format("Cannot open file for read {0} : {1}", _file, e.Message));
                                    }
                                }
                                finally
                                {
                                    if (fileStream != null)
                                    {
                                        //If you have a folder located at sftp://ftp.example.com/share
                                        //then you can add this like:
                                        sftp.UploadFile(fileStream, Path.GetFileName(_file), null);
                                        fileStream.Close();
                                    }
                                }
                                break;
                            }

                        }
                        //Run setup script for the collector
                        using (var sshclient = new SshClient(conInfo))
                        {
                            sshclient.Connect();
                            using (var cmd = sshclient.CreateCommand("cd ~/mrmp; sh mrmp_setup.sh"))
                            {
                                cmd.Execute();
                                if (cmd.ExitStatus != 0)
                                {
                                    sshclient.Disconnect();
                                    throw new Exception(String.Format("Error while running unix setup script: {0}", cmd.Result));
                                }

                            }
                            //run the first inventory collection
                            using (var cmd = sshclient.CreateCommand("cd ~/mrmp/bin/; sh mrmp_inv_cron.sh; sh mrmp_perf_cron.sh"))
                            {
                                cmd.Execute();
                                if (cmd.ExitStatus != 0)
                                {
                                    sshclient.Disconnect();
                                    throw new Exception(String.Format("Error while running unix setup script: {0}", cmd.Result));
                                }

                            }
                            sshclient.Disconnect();
                        }

                    }


                    if (sftp.Exists("output"))
                    {
                        foreach (SftpFile _file in sftp.ListDirectory("output"))
                        {
                            List<String> _inv_file = new List<string>();
                            if (_file.Name.StartsWith("Inv_"))
                            {
                                if (_file.Length > 0)
                                {
                                    try
                                    {
                                        //get OS edition name
                                        _inv_file = sftp.ReadAllLines(_file.FullName).ToList();

                                        //Remove Inventory file from server
                                        sftp.DeleteFile(_file.FullName);

                                        string _os = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Producer")).Split('=').Last();
                                        //if the workload is SuSE we want to use the DAPP_NAME and not the Producer
                                        if (_os.Contains("SuSE"))
                                        {
                                            _os = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Name")).Split('=').Last();
                                        }
                                        if (_os.Contains("CentOS"))
                                        {
                                            _os = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Name")).Split('=').Last();
                                        }
                                        if (_os.Contains("Red Hat"))
                                        {
                                            _os = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Name")).Split('=').Last();
                                        }
                                        string _display_version = _inv_file.FirstOrDefault(x => x.Contains("DAPP_DisplayVersion")).Split('=').Last();
                                        if (!_os.Any(c => char.IsDigit(c)))
                                        {
                                            _os = _os + " " + _display_version;
                                        }

                                        string _arch = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Architecture")).Split('=').Last();
                                        string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
                                        string output = Regex.Replace(_os, regex, "");

                                        _updated_workload.osedition = OSEditionSimplyfier.Simplyfier(String.Format("{0} {1}", output, _arch));

                                        _updated_workload.model = _os = String.Format("{0} {1}", _inv_file.FirstOrDefault(x => x.Contains("DCH_Make")).Split('=').Last(), _inv_file.FirstOrDefault(x => x.Contains("DCH_Model")).Split('=').Last());
                                        if (_updated_workload.model.ToLower().Contains("virtual"))
                                        {
                                            _updated_workload.hardwaretype = "virtual";
                                        }
                                        else if (!_updated_workload.model.ToLower().Contains("virtual") && !String.IsNullOrEmpty(_updated_workload.model))
                                        {
                                            _updated_workload.hardwaretype = "physical";
                                        }
                                        else
                                        {
                                            _updated_workload.hardwaretype = "unknown";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.log(String.Format("Error collecting Caption (OS Type) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                                    }
                                    //get system resources (hostname, CPU, speed, Memory)
                                    var _temp_hostname = "";
                                    try { _temp_hostname = _inv_file.FirstOrDefault(x => x.Contains("ISRV_HostName")).Split('=').Last(); } catch (Exception) { }

                                    //check if we got the FQDN and only get the hostname
                                    if (_temp_hostname.Contains("."))
                                    {
                                        _updated_workload.hostname = _temp_hostname.Split('.').First();
                                    }
                                    else
                                    {
                                        _updated_workload.hostname = _temp_hostname;
                                    }

                                    try { _updated_workload.vcpu = _inv_file.Count(x => x.Contains("<CPU>")); } catch (Exception) { }
                                    try { _updated_workload.vcore = Int32.Parse(_inv_file.FirstOrDefault(x => x.Contains("DCPU_Cores")).Split('=').Last()); } catch (Exception) { }

                                    Double _temp_cpuspeed_value;
                                    string _value_string = _inv_file.FirstOrDefault(x => x.Contains("DCPU_RatedSpeed")).Split('=').Last();
                                    if (Double.TryParse(_value_string, NumberStyles.Float, CultureInfo.InvariantCulture, out _temp_cpuspeed_value))
                                    {
                                        _updated_workload.vcpu_speed = _temp_cpuspeed_value;
                                    }
                                    else
                                    {
                                        Logger.log(String.Format("Cannot parse {0} for cpu speed", _value_string), Logger.Severity.Warn);
                                        _updated_workload.vcpu_speed = 0.0;
                                    }

                                    try
                                    {
                                        decimal _vmemory_sum = 0;
                                        foreach (string _memory_string in _inv_file.FindAll(x => x.Contains("DRAM_Size")))
                                        {
                                            _vmemory_sum += Int32.Parse(_memory_string.Split('=').Last());
                                        }
                                        _updated_workload.vmemory = (int)(_vmemory_sum / 1024);
                                    }
                                    catch (Exception) { }

                                    //Process disks
                                    for (var i = 0; i < _inv_file.Count; i++)
                                    {
                                        if (_inv_file[i] == "<DISKINFO>")
                                        {
                                            bool _valid_disk = false;
                                            MRPWorkloadDiskType _tmpdisk = new MRPWorkloadDiskType();
                                            while (_inv_file[i] != "</DISKINFO>")
                                            {
                                                i++;
                                                if (_inv_file[i].Contains("ISDR_DeviceId"))
                                                {
                                                    try { _tmpdisk.diskindex = Int16.Parse(Regex.Replace(_inv_file[i].Split('=').Last(), "[^0-9]", "")); _tmpdisk.diskindex -= 1; } catch (Exception) { }
                                                }
                                                if (_inv_file[i].Contains("ISDR_Space"))
                                                {
                                                    try { _tmpdisk.disksize = Int16.Parse(_inv_file[i].Split('=').Last()); _tmpdisk.disksize = _tmpdisk.disksize / 1024; } catch (Exception) { }
                                                    _valid_disk = true;
                                                }
                                            }
                                            if (_valid_disk)
                                            {
                                                MRPWorkloadDiskType _disk = new MRPWorkloadDiskType();

                                                if (_updated_workload.workloaddisks.Any(x => x.diskindex == _tmpdisk.diskindex))
                                                {
                                                    _disk = _updated_workload.workloaddisks.FirstOrDefault(x => x.diskindex == _tmpdisk.diskindex);
                                                }
                                                else
                                                {
                                                    _updated_workload.workloaddisks.Add(_disk);
                                                }
                                                _disk.deleted = false;
                                                _disk.diskindex = _tmpdisk.diskindex;
                                                _disk.disksize = _tmpdisk.disksize;

                                            }
                                        }
                                    }

                                    //Process filesystems
                                    for (var i = 0; i < _inv_file.Count; i++)
                                    {
                                        if (_inv_file[i] == "<FILESYS>")
                                        {
                                            bool _valid_fs = false;
                                            MRPWorkloadVolumeType _tmpvolume = new MRPWorkloadVolumeType();

                                            _updated_workload.workloadvolumes = _workload.workloadvolumes;


                                            while (_inv_file[i] != "</FILESYS>")
                                            {
                                                i++;
                                                if (_inv_file[i].Contains("ISFS_Device") && _inv_file[i].Contains("/dev/"))
                                                {
                                                    _tmpvolume.deviceid = _inv_file[i].Split('=').Last();
                                                }
                                                if (_inv_file[i].Contains("ISFS_Type"))
                                                {
                                                    _tmpvolume.filesystem_type = _inv_file[i].Split('=').Last();
                                                    string[] _fs_types = new string[] { "EXT4", "EXT3", "EXT2", "XFS", "ZFS" };
                                                    if (_fs_types.Any(x => x == _tmpvolume.filesystem_type.ToUpper()))
                                                    {
                                                        _valid_fs = true;
                                                    }
                                                }
                                                if (_inv_file[i].Contains("ISFS_Size"))
                                                {
                                                    _tmpvolume.volumesize = Int64.Parse(_inv_file[i].Split('=').Last());
                                                    _tmpvolume.volumesize = _tmpvolume.volumesize / 1024;
                                                }
                                                if (_inv_file[i].Contains("ISFS_SpaceFree"))
                                                {
                                                    _tmpvolume.volumefreespace = Int64.Parse(_inv_file[i].Split('=').Last());
                                                    _tmpvolume.volumefreespace = _tmpvolume.volumefreespace / 1024;
                                                }
                                                if (_inv_file[i].Contains("ISFS_Path"))
                                                {
                                                    _tmpvolume.driveletter = _inv_file[i].Split('=').Last();
                                                }
                                            }
                                            if (_valid_fs)
                                            {
                                                //if volume already exists in portal, just update it  
                                                MRPWorkloadVolumeType _volume = new MRPWorkloadVolumeType();
                                                if (_updated_workload.workloadvolumes.Exists(x => x.driveletter == _tmpvolume.driveletter))
                                                {
                                                    _volume = _updated_workload.workloadvolumes.FirstOrDefault(x => x.driveletter == _tmpvolume.driveletter);
                                                }
                                                else
                                                {
                                                    _updated_workload.workloadvolumes.Add(_volume);
                                                }
                                                _volume.blocksize = _tmpvolume.blocksize;
                                                _volume.deviceid = _tmpvolume.deviceid;
                                                _volume.diskindex = _tmpvolume.diskindex;
                                                _volume.driveletter = _tmpvolume.driveletter;
                                                _volume.filesystem_type = _tmpvolume.filesystem_type;
                                                _volume.provisioned = true;
                                                _volume.serialnumber = _tmpvolume.serialnumber;
                                                _volume.volumefreespace = _tmpvolume.volumefreespace;
                                                _volume.volumename = _tmpvolume.volumename;
                                                _volume.volumesize = _tmpvolume.volumesize;
                                                _volume.deleted = false;
                                            }
                                        }
                                    }

                                    //Process Network
                                    for (var i = 0; i < _inv_file.Count; i++)
                                    {
                                        if (_inv_file[i] == "<NETWORK>")
                                        {
                                            bool _valid_net = false;
                                            MRPWorkloadInterfaceType _tmpinterface = new MRPWorkloadInterfaceType();
                                            while (_inv_file[i] != "</NETWORK>")
                                            {
                                                i++;
                                                if (_inv_file[i].Contains("ISN_DeviceID"))
                                                {
                                                    try { _tmpinterface.connection_index = Int16.Parse(Regex.Replace(_inv_file[i].Split('=').Last(), "[^0-9]", "")); } catch (Exception) { }
                                                }
                                                if (_inv_file[i].Contains("ISN_MACAddress"))
                                                {
                                                    try { _tmpinterface.macaddress = _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                    _valid_net = true;
                                                }
                                                if (_inv_file[i].Contains("ISN_SubnetMask"))
                                                {
                                                    try { _tmpinterface.netmask += _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                }
                                                if (_inv_file[i].Contains("ISN_IPAddress"))
                                                {
                                                    try { _tmpinterface.ipaddress = _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                }
                                            }
                                            if (_valid_net)
                                            {
                                                MRPWorkloadInterfaceType _interface = new MRPWorkloadInterfaceType();

                                                if (_updated_workload.workloadinterfaces.Any(x => x.connection_index == _tmpinterface.connection_index))
                                                {
                                                    _interface = _updated_workload.workloadinterfaces.FirstOrDefault(x => x.connection_index == _tmpinterface.connection_index);
                                                }
                                                else
                                                {
                                                    _updated_workload.workloadinterfaces.Add(_interface);
                                                }
                                                _interface.ipassignment = "manual_ip";
                                                _interface.connection_index = _tmpinterface.connection_index;
                                                _interface.macaddress = _tmpinterface.macaddress;
                                                _interface.ipaddress = _tmpinterface.ipaddress;
                                                _interface.netmask = _tmpinterface.netmask;
                                                _interface.deleted = false;

                                            }
                                        }
                                    }
                                    _updated_workload.ostype = "unix";
                                    _updated_workload.provisioned = true;

                                    MRMPServiceBase._mrmp_api.workload().updateworkload(_updated_workload);
                                    MRMPServiceBase._mrmp_api.workload().InventoryUpdateStatus(_updated_workload, "Success", true);
                                }
                                else
                                {
                                    throw new Exception(String.Format("Inventory file {0} has a zero size. Please make sure the username has the correct permissions.", _file.FullName));
                                }
                            }
                        }

                    }
                    sftp.Disconnect();
                }

            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Inventory:Error collecting inventory information: {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                throw new ArgumentException(String.Format("Error collecting inventory information: {0}", ex.Message));
            }
            Logger.log(String.Format("Inventory: Completed inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Debug);
        }
    }
}