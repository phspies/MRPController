using MRMPService.MRMPAPI.Types.API;
using System;
using System.Linq;
using System.Threading;
using MRMPService.Utilities;
using Renci.SshNet;
using MRMPService.MRMPService.Types.API;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using Renci.SshNet.Common;

namespace MRMPService.Tasks.MCP
{
    partial class MCP_Platform
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
        static public void LinuxCustomization(String _task_id, MRPPlatformType _platform, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, float _start_progress, float _end_progress)
        {
            MRPCredentialType _credential = _target_workload.credential;

            MRPWorkloadVolumeType _root_volume_object = _target_workload.workloadvolumes_attributes.FirstOrDefault(x => x.driveletter == "/");

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                using (MRMPAPI.MRMP_ApiClient _mrp_api = new MRMPAPI.MRMP_ApiClient())
                {
                    _mrp_api.task().failcomplete(_task_id, String.Format("Error contacting workwork {0} after 3 tries", _target_workload.hostname));
                    throw new ArgumentException(String.Format("Error contacting workwork {0} after 3 tries", _target_workload.hostname));
                }
            }
            _password = _credential.encrypted_password;


            KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_credential.username);
            _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
            PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_credential.username, _password);
            ConnectionInfo ConnNfo = new ConnectionInfo(workload_ip, 22, _credential.username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });

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
                        if (vars.Count() == 6)
                        {
                            if (vars[5] == _root_volume_object.driveletter)
                            {
                                string _string = vars[0];
                                _volumegroup = _string.Split('-')[0];
                                root_lvm_volume = _string.Split('-')[1];
                                root_lvm_size = float.Parse(vars[3].ToString()) / 1024 / 1024 / 1024;
                            }
                        }
                        if (vars.Count() == 7)
                        {
                            if (vars[6] == _root_volume_object.driveletter)
                            {
                                string _string = vars[0];
                                _volumegroup = _string.Split('-')[0];
                                root_lvm_volume = _string.Split('-')[1];
                                root_lvm_size = float.Parse(vars[3].ToString()) / 1024 / 1024 / 1024;
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
                ComputeApiClient _caas = ComputeApiClient.GetComputeApiClient(new Uri(_platform.url), new NetworkCredential(_platform.credential.username, _platform.credential.encrypted_password));
                _caas.Login().Wait();
                ServerType _mcp_workload = _caas.ServerManagement.Server.GetServer(new Guid(_target_workload.moid)).Result;
                _caas.ServerManagementLegacy.Server.ChangeServerDiskSize(_mcp_workload.id, _mcp_workload.disk[0].id, (_0_shortfall + _mcp_workload.disk[0].sizeGb).ToString()).Wait();
                _mcp_workload = _caas.ServerManagement.Server.GetServer(new Guid(_mcp_workload.id)).Result;
                while (_mcp_workload.state != "NORMAL" && _mcp_workload.started == true)
                {
                    Thread.Sleep(5000);
                    _mcp_workload = _caas.ServerManagement.Server.GetServer(new Guid(_mcp_workload.id)).Result;
                }
                _caas.ServerManagement.Server.RebootServer(new Guid(_mcp_workload.id)).Wait();
                _mcp_workload = _caas.ServerManagement.Server.GetServer(new Guid(_mcp_workload.id)).Result;
                while (_mcp_workload.state != "NORMAL" && _mcp_workload.started == true)
                {
                    Thread.Sleep(5000);
                    _mcp_workload = _caas.ServerManagement.Server.GetServer(new Guid(_mcp_workload.id)).Result;
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
                var cmd = new Object();
                //create extended partition
                if (_0_shortfall > 0)
                {
                    cmd = sshclient.RunCommand("echo -e 'n\ne\n4\n\n\n\nw' | fdisk /dev/sda");

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
                    else if (_part_fs_type == "ext4" || _part_fs_type == "ext3")
                    {
                        cmd = sshclient.RunCommand(String.Format("resize2fs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
                    }
                }
                //create partitions for all the other volumes if they exist
                string[] _disk_id = new string[] { "b", "c", "d", "e", "f", "h", "k", "l", "m", "n" };

                foreach (int _disk_index in _target_workload.workloadvolumes_attributes.Where(x => x.diskindex > 0).Select(x => x.diskindex).Distinct())
                {
                    cmd = sshclient.RunCommand(String.Format("echo -e 'n\n\n\n\n\nw' | fdisk /dev/sd{0}", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("pvcreate /dev/sd{0}1", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("vgcreate {0} /dev/sd{1}1", String.Format("vg-{0}", _disk_id[_disk_index]), _disk_id[_disk_index]));
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
                        else if (_part_fs_type == "ext4" || _part_fs_type == "ext3")
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
