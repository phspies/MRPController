using MRMPService.MRMPAPI.Types.API;
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

namespace MRMPService.MRMPAPI.Classes
{
    partial class WorkloadInventory
    {
        public void WorkloadInventoryUnixDo(MRPWorkloadType _workload)
        {
            MRPWorkloadType _updated_workload = new MRPWorkloadType() { id = _workload.id };

            //check for credentials
            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Error finding credentials"));
            }

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, true);
            }

            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Error contacting workload"));
            }

            Logger.log(String.Format("Inventory: Started inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username,
                 new AuthenticationMethod[] { new PasswordAuthenticationMethod(_credential.username, _credential.encrypted_password) }
             );
            string locationlocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string localscripts = Path.Combine(locationlocation, "Scripts");

            string remotepath = @"mrmp";
            bool remotepath_exist = false;
            try
            {
                using (var sftp = new SftpClient(ConnNfo))
                {
                    sftp.Connect();
                    try
                    {
                        sftp.ChangeDirectory(remotepath);
                        remotepath_exist = true;
                    }
                    catch (SftpPathNotFoundException)
                    {
                        sftp.CreateDirectory(remotepath);
                        sftp.ChangeDirectory(remotepath);
                    }
                    if (!remotepath_exist)
                    {
                        foreach (String _file in Directory.GetFiles(localscripts))
                        {
                            var fileStream = new FileStream(_file, FileMode.Open);
                            if (fileStream != null)
                            {
                                //If you have a folder located at sftp://ftp.example.com/share
                                //then you can add this like:
                                sftp.UploadFile(fileStream, Path.GetFileName(_file), null);
                            }
                        }
                        //Run setup script for the collector
                        using (var sshclient = new SshClient(ConnNfo))
                        {
                            sshclient.Connect();
                            using (var cmd = sshclient.CreateCommand(Path.Combine(remotepath, "mrmp_setup.sh")))
                            {
                                cmd.Execute();
                                if (cmd.ExitStatus != 0)
                                {
                                    sshclient.Disconnect();
                                    throw new ArgumentException(String.Format("Error while running unix setup script: %1", cmd.Result));
                                }

                            }
                            sshclient.Disconnect();
                        }
                    }
                    else
                    {
                        if (sftp.Exists("output"))
                        {
                            foreach (SftpFile _file in sftp.ListDirectory("output"))
                            {
                                List<String> _inv_file = new List<string>();
                                if (_file.Name.StartsWith("Inv_"))
                                {
                                    try
                                    {
                                        //get OS edition name
                                        _inv_file = sftp.ReadAllLines(_file.FullName).ToList();

                                        //Remove Inventory file from server
                                        sftp.DeleteFile(_file.FullName);

                                        string _os = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Producer")).Split('=').Last();
                                        string _arch = _inv_file.FirstOrDefault(x => x.Contains("DAPP_Architecture")).Split('=').Last();
                                        _updated_workload.osedition = OSEditionSimplyfier.Simplyfier(String.Format("{0} {1}", _os, _arch));
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.log(String.Format("Error collecting Caption (OS Type) from {0} : {1}", _workload.hostname, ex.Message), Logger.Severity.Error);
                                    }
                                    //get system resources (hostname, CPU, speed, Memory)
                                    try { _updated_workload.hostname = _inv_file.FirstOrDefault(x => x.Contains("ISRV_HostName")).Split('=').Last(); } catch (Exception) { }
                                    try { _updated_workload.vcpu = _inv_file.Count(x => x.Contains("<CPU>")); } catch (Exception) { }
                                    try { _updated_workload.vcore = Int32.Parse(_inv_file.FirstOrDefault(x => x.Contains("DCPU_Cores")).Split('=').Last()); } catch (Exception) { }
                                    String _string = _inv_file.FirstOrDefault(x => x.Contains("DCPU_RatedSpeed"));
                                    try { _updated_workload.vcpu_speed = (int)Decimal.Parse(_string.Split('=').Last()); } catch (Exception) { }

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



                                    //Process filesystems
                                    for (var i = 0; i < _inv_file.Count; i++)
                                    {
                                        if (_inv_file[i] == "<FILESYS>")
                                        {
                                            bool _valid_fs = false;
                                            MRPWorkloadVolumeType _volume = new MRPWorkloadVolumeType();

                                            while (_inv_file[i] != "</FILESYS>")
                                            {
                                                i++;
                                                if (_inv_file[i].Contains("ISFS_Device") && _inv_file[i].Contains("/dev/"))
                                                {
                                                    _volume.deviceid = _inv_file[i].Split('=').Last();
                                                }
                                                if (_inv_file[i].Contains("ISFS_Type"))
                                                {
                                                    _volume.filesystem_type = _inv_file[i].Split('=').Last();
                                                    switch (_volume.filesystem_type.ToUpper())
                                                    {
                                                        case "EXT4":
                                                            _valid_fs = true;
                                                            break;
                                                        case "EXT3":
                                                            _valid_fs = true;
                                                            break;
                                                        case "EXT2":
                                                            _valid_fs = true;
                                                            break;
                                                        case "ZFS":
                                                            _valid_fs = true;
                                                            break;
                                                        case "XFS":
                                                            _valid_fs = true;
                                                            break;
                                                    }
                                                }
                                                if (_inv_file[i].Contains("ISFS_Size"))
                                                {
                                                    _volume.volumesize = Int64.Parse(_inv_file[i].Split('=').Last());
                                                    _volume.volumesize = _volume.volumesize / 1024;
                                                }
                                                if (_inv_file[i].Contains("ISFS_SpaceFree"))
                                                {
                                                    _volume.volumefreespace = Int64.Parse(_inv_file[i].Split('=').Last());
                                                    _volume.volumefreespace = _volume.volumefreespace / 1024;
                                                }
                                                if (_inv_file[i].Contains("ISFS_Path"))
                                                {
                                                    _volume.driveletter = _inv_file[i].Split('=').Last();
                                                }
                                            }
                                            if (_valid_fs)
                                            {
                                                if (_updated_workload.workloadvolumes_attributes == null)
                                                {
                                                    _updated_workload.workloadvolumes_attributes = new List<MRPWorkloadVolumeType>();
                                                }
                                                //if volume already exists in portal, just update it   
                                                if (_workload.workloadvolumes_attributes.Exists(x => x.driveletter == _volume.driveletter))
                                                {
                                                    _volume.id = _workload.workloadvolumes_attributes.FirstOrDefault(x => x.driveletter == _volume.driveletter).id;
                                                }
                                                _updated_workload.workloadvolumes_attributes.Add(_volume);
                                            }
                                        }
                                    }

                                    //Process Network
                                    for (var i = 0; i < _inv_file.Count; i++)
                                    {
                                        if (_inv_file[i] == "<NETWORK>")
                                        {
                                            bool _valid_net = false;
                                            MRPWorkloadInterfaceType _interface = new MRPWorkloadInterfaceType();
                                            while (_inv_file[i] != "</NETWORK>")
                                            {
                                                i++;
                                                if (_inv_file[i].Contains("ISN_DeviceID"))
                                                {
                                                    try { _interface.connection_index = Int16.Parse(Regex.Replace(_inv_file[i].Split('=').Last(), "[^0-9]", "")); } catch (Exception) { }
                                                }
                                                if (_inv_file[i].Contains("ISN_MACAddress"))
                                                {
                                                    try { _interface.macaddress = _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                    _valid_net = true;
                                                }
                                                if (_inv_file[i].Contains("ISN_SubnetMask"))
                                                {
                                                    try { _interface.netmask += _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                }
                                                if (_inv_file[i].Contains("ISN_IPAddress"))
                                                {
                                                    try { _interface.ipaddress = _inv_file[i].Split('=').Last(); } catch (Exception) { }
                                                }
                                            }
                                            if (_valid_net)
                                            {
                                                if (_updated_workload.workloadinterfaces_attributes == null)
                                                {
                                                    _updated_workload.workloadinterfaces_attributes = new List<MRPWorkloadInterfaceType>();
                                                }
                                                _interface.ipassignment = "manual_ip";
                                                if (_workload.workloadinterfaces_attributes.Any(x => x.connection_index == _interface.connection_index))
                                                {
                                                    _interface.id = _workload.workloadinterfaces_attributes.FirstOrDefault(x => x.connection_index == _interface.connection_index).id;
                                                }
                                                _updated_workload.workloadinterfaces_attributes.Add(_interface);
                                            }
                                        }
                                    }
                                    //exit out of loop as we dont want o process more than one Inv file at a time.
                                    break;
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

            _updated_workload.ostype = "unix";
            _updated_workload.provisioned = true;

            using (MRMP_ApiClient _api = new MRMP_ApiClient())
            {
                _api.workload().updateworkload(_updated_workload);
                _api.workload().InventoryUpdateStatus(_updated_workload, "Success", true);
            }

            Logger.log(String.Format("Inventory: Completed inventory collection for {0} : {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

        }
    }
}