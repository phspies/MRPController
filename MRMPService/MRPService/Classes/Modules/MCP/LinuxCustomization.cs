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
using Renci.SshNet;
using DD.CBU.Compute.Api.Client;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
    {
        static public void LinuxCustomization(MRPTaskType payload, ServerType _newvm, ComputeApiClient _caas)
        {
            MRPTaskSubmitpayloadType _payload = payload.submitpayload;
            MRPPlatformType _platform = _payload.platform;
            MRPWorkloadType _target_workload = _payload.target;
            MRPCredentialType _credential = _target_workload.credential;
            MRPCredentialType _platform_credentail = _platform.credential;

            MRPWorkloadVolumeType _root_volume_object = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.driveletter == "/");

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(String.Join(",", _newvm.networkInfo.primaryNic.ipv6, _newvm.networkInfo.primaryNic.privateIpv4), true);
            }
            if (workload_ip == null)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(payload, String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                    throw new ArgumentException(String.Format("Error contacting workwork {0} after 3 tries", _newvm.name));
                }
            }

            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(_credential.username, _credential.encrypted_password) });

            float root_lvm_size = 0;
            string root_lvm_volume = "";
            Double root_disk_free = 0;
            string _volumegroup = "";
            string _part_fs_type = "";

            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();

                using (var cmd = sshclient.CreateCommand("lsblk -lb"))
                {
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        sshclient.Disconnect();
                        throw new ArgumentException(String.Format("Error while running unix setup script: %1", cmd.Result));
                    }

                    string[] lines = cmd.Result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string _line in lines)
                    {
                        string[] vars = _line.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (vars.Count() > 6)
                        {
                            if (vars[6] == _root_volume_object.driveletter)
                            {
                                string _string = vars[0];
                                _volumegroup = _string.Split('-')[0];
                                root_lvm_volume = _string.Split('-')[1];
                                root_lvm_size = float.Parse(vars[3].ToString()) / 1024 / 1024 / 1024;
                            }
                        }
                        if (vars.Count() > 7)
                        {
                            if (vars[7] == _root_volume_object.driveletter)
                            {
                                string _string = vars[0];
                                _volumegroup = _string.Split('-')[0];
                                root_lvm_volume = _string.Split('-')[1];
                                root_lvm_size = float.Parse(vars[4].ToString()) / 1024 / 1024 / 1024;
                            }
                        }
                    }
                }
                var _diskfree_cmd = sshclient.CreateCommand("parted /dev/sda unit GB print free | grep 'Free Space' | tail -n1 | awk '{print $3}'");
                _diskfree_cmd.Execute();
                root_disk_free = Convert.ToDouble(Math.Floor(Double.Parse(_diskfree_cmd.Result.Replace("GB", ""))));

                var _fs_type_cmd = sshclient.CreateCommand("df -T / | grep -v 'Filesystem' | awk '{print $2}'");
                _fs_type_cmd.Execute();
                if (_fs_type_cmd.ExitStatus == 0)
                {
                    _part_fs_type = _fs_type_cmd.Result.Trim();
                }

                sshclient.Disconnect();
            }

            //calculate capacity shortfall
            double _0_shortfall = Math.Ceiling(_root_volume_object.volumesize - root_lvm_size - root_disk_free);

            //if we need to expand disk 0, lets do that first...
            if (_0_shortfall > 0)
            {
                _caas.ServerManagementLegacy.Server.ChangeServerDiskSize(_newvm.id, _newvm.disk[0].id, (_0_shortfall + _newvm.disk[0].sizeGb).ToString()).Wait();
                _newvm = _caas.ServerManagement.Server.GetServer(new Guid(_newvm.id)).Result;
                while (_newvm.state != "NORMAL" && _newvm.started == true)
                {
                    Thread.Sleep(5000);
                    _newvm = _caas.ServerManagement.Server.GetServer(new Guid(_newvm.id)).Result;
                }
                _caas.ServerManagement.Server.RebootServer(new Guid(_newvm.id)).Wait();
                _newvm = _caas.ServerManagement.Server.GetServer(new Guid(_newvm.id)).Result;
                while (_newvm.state != "NORMAL" && _newvm.started == true)
                {
                    Thread.Sleep(5000);
                    _newvm = _caas.ServerManagement.Server.GetServer(new Guid(_newvm.id)).Result;
                }
            }
            using (var sshclient = new SshClient(ConnNfo))
            {
                int retry = 3;
                while (retry > 0)
                {
                    retry--;
                    try
                    {
                        sshclient.Connect();
                        break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (!sshclient.IsConnected)
                {
                    throw new Exception("Error connecting to linux server");
                }
                //create extended partition
                var cmd = sshclient.RunCommand("echo -e 'n\ne\n4\n\n\n\nw' | fdisk /dev/sda");

                //create partitions for all the other volumes if they exist
                string[] _disk_id = new string[] { "b", "c", "d", "e", "f", "h", "k", "l", "m", "n" };

                foreach (int _disk_index in _target_workload.workloadvolumes_attributes.Where(x => x.diskindex > 0).Select(x => x.diskindex).Distinct())
                {
                    cmd = sshclient.RunCommand(String.Format("echo -e 'n\n\n\n\n\nw' | fdisk /dev/sd{0}", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("pvcreate /dev/sd{0}1", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("vgcreate {0} /dev/sd{1}1", String.Format("vg-{0}", _disk_id[_disk_index]), _disk_id[_disk_index]));
                }

                //create new partition on number 5
                int _create_part_number = 5;
                cmd = sshclient.RunCommand("echo -e 'n\nl\n\n\n\nw' | fdisk /dev/sda");

                //rescan linux device files
                cmd = sshclient.RunCommand("partprobe /dev/sda");

                //create physical volume for new partition
                cmd = sshclient.RunCommand("pvcreate /dev/sda" + _create_part_number);
                cmd = sshclient.RunCommand(String.Format("vgextend {0} /dev/sda{1}", _volumegroup, _create_part_number));
                cmd = sshclient.RunCommand(String.Format("lvextend -L{3}G /dev/{0}/{1} /dev/sda{2}", _volumegroup, root_lvm_volume, _create_part_number, _root_volume_object.volumesize));
                if (_part_fs_type == "xfs")
                {
                    cmd = sshclient.RunCommand(String.Format("xfs_growfs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
                }
                else if (_part_fs_type == "ext4")
                {
                    cmd = sshclient.RunCommand(String.Format("resize2fs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
                }
                foreach (int _disk_index in _target_workload.workloadvolumes_attributes.Where(x => x.diskindex > 0).Select(x => x.diskindex).Distinct())
                {
                    foreach (MRPWorkloadVolumeType _volume in _target_workload.workloadvolumes_attributes.ToList().Where(x => x.diskindex == _disk_index).OrderBy(x => x.driveletter))
                    {
                        String _volume_group = String.Format("vg-{0}", _disk_id[_disk_index]);
                        String _volume_name = String.Format("logical_vol{0}", _volume.driveletter.Replace("/", "_"));
                        //create physical volume for new partition
                        cmd = sshclient.RunCommand(String.Format("lvcreate -L{0}G -n {1} {2}", _volume.volumesize, _volume_name, _volume_group));
                        if (_part_fs_type == "xfs")
                        {
                            cmd = sshclient.RunCommand(String.Format("mkfs.xfs /dev/{0}/{1}", _volume_group, _volume_name));
                        }
                        else if (_part_fs_type == "ext4")
                        {
                            cmd = sshclient.RunCommand(String.Format("mkfs.ext4 /dev/{0}/{1}", _volume_group, _volume_name));
                        }
                        cmd = sshclient.RunCommand(String.Format("exho '/dev/{0}/{1} \t {2} \t {3} \t defaults 0 2' >> /dev/fstab",
                            _volume_group,
                            _volume_name,
                            _volume.driveletter,
                            _part_fs_type));
                        cmd = sshclient.RunCommand(String.Format("mount {0}", _volume.driveletter));
                    }
                }
            }
        }
    }
}
