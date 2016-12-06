using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using System.Threading;
using MRMPService.Utilities;
using Renci.SshNet;
using DD.CBU.Compute.Api.Client;
using System.Net;
using DD.CBU.Compute.Api.Contracts.Network20;
using Renci.SshNet.Common;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;

namespace MRMPService.Modules.MCP
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
        static public async Task LinuxCustomization(String _task_id, MRPPlatformType _platform, MRPWorkloadType _target_workload, MRPProtectiongroupType _protectiongroup, float _start_progress, float _end_progress)
        {
            MRPCredentialType _credential = _target_workload.credential;

            MRPWorkloadVolumeType _root_volume_object = _target_workload.workloadvolumes.FirstOrDefault(x => x.driveletter == "/");

            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_target_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                await MRMPServiceBase._mrmp_api.task().failcomplete(_task_id, String.Format("Error contacting workwork {0} after 3 tries", _target_workload.hostname));
                throw new ArgumentException(String.Format("Error contacting workwork {0} after 3 tries", _target_workload.hostname));
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
            bool _can_do_root = true;
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();

                using (var cmd = sshclient.CreateCommand("lsblk -lb"))
                {
                    cmd.Execute();
                    if (cmd.ExitStatus != 0)
                    {
                        _can_do_root = false;
                        sshclient.Disconnect();

                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Error expanding 0 disk : {0}", cmd.Error), ReportProgress.Progress(_start_progress, _end_progress, 10));
                    }

                    if (_can_do_root)
                    {
                        string[] lines = cmd.Result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string _line in lines)
                        {
                            string[] vars = _line.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            int pos = Array.IndexOf(vars, _root_volume_object.driveletter);
                            if (pos > 0)
                            {
                                string _string = vars[0];
                                _volumegroup = _string.Split('-')[0];
                                root_lvm_volume = _string.Split('-')[1];
                                root_lvm_size = float.Parse(vars[pos - 3].ToString()) / 1024 / 1024 / 1024;
                                break;
                            }
                        }
                    }
                }
                if (_can_do_root)
                {
                    var _diskfree_cmd = sshclient.CreateCommand("parted /dev/sda unit GB print free | grep 'Free Space' | tail -n1 | awk '{print $3}'");
                    _diskfree_cmd.Execute();
                    if (_diskfree_cmd.ExitStatus == 0)
                    {
                        root_disk_free = Convert.ToDouble(Math.Floor(Double.Parse(_diskfree_cmd.Result.Replace("GB", ""))));
                        var _fs_type_cmd = sshclient.CreateCommand("df -T / | grep -v 'Filesystem' | awk '{print $2}'");
                        _fs_type_cmd.Execute();
                        if (_fs_type_cmd.ExitStatus == 0)
                        {
                            _part_fs_type = _fs_type_cmd.Result.Trim();
                        }
                    }
                    else
                    {
                        _can_do_root = false;

                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Error running parted : {0}", _diskfree_cmd.Error), ReportProgress.Progress(_start_progress, _end_progress, 11));
                    }

                    sshclient.Disconnect();
                }
            }
            double _0_volumeshortfall = 0;
            //calculate capacity shortfall
            if (_can_do_root)
            {
                double _0_shortfall = Math.Ceiling((long)_root_volume_object.volumesize - root_lvm_size - root_disk_free);
                _0_volumeshortfall = Math.Ceiling((long)_root_volume_object.volumesize - root_lvm_size);
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
                    catch (Exception)
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 30));
                    }
                }
                if (!sshclient.IsConnected)
                {
                    throw new Exception("Error connecting to linux workload");
                }
                SshCommand cmd;
                if (_can_do_root)
                {
                    //create extended partition
                    if (_0_volumeshortfall > 0)
                    {
                        //we need to get a list of all the partitions to see where we can add a partition for the additional space on root
                        cmd = sshclient.RunCommand("fdisk -l /dev/sda | grep '^/dev' | cut -d' ' -f1");
                        int _free_part_number = 0;
                        if (cmd.ExitStatus == 0)
                        {
                            string[] partitions = cmd.Result.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            int[] _possible_parts = { 1, 2, 3, 4, 5 };
                            for (int i = 0; i < _possible_parts.Length; i++)
                            {
                                if (i <= partitions.Count() - 1)
                                {
                                    var _number = partitions[i].Substring(8, 1);
                                    if (Int16.Parse(_number) != _possible_parts[i])
                                    {
                                        _free_part_number = _possible_parts[i];
                                        break;
                                    }
                                }
                                else
                                {
                                    _free_part_number = _possible_parts[i];
                                    break;
                                }
                            }
                        }
                        if (_free_part_number <= 3)
                        {
                            cmd = sshclient.RunCommand(String.Format("echo -e 'n\np\n{0}\n\n\n\nw' | fdisk /dev/sda", _free_part_number));
                        }
                        else
                        {
                            cmd = sshclient.RunCommand(String.Format("echo -e 'n\np\n{0}\n\n\n\nw' | fdisk /dev/sda", _free_part_number));
                            cmd = sshclient.RunCommand("echo -e 'n\nl\n\n\n\nw' | fdisk /dev/sda");
                        }


                        //rescan linux device files
                        cmd = sshclient.RunCommand("partprobe /dev/sda");

                        //create physical volume for new partition
                        cmd = sshclient.RunCommand("pvcreate /dev/sda" + _free_part_number);
                        cmd = sshclient.RunCommand(String.Format("vgextend {0} /dev/sda{1}", _volumegroup, _free_part_number));
                        cmd = sshclient.RunCommand(String.Format("lvextend -L{3}G /dev/{0}/{1} /dev/sda{2}", _volumegroup, root_lvm_volume, _free_part_number, _root_volume_object.volumesize - 2));

                        if (_part_fs_type == "xfs")
                        {
                            cmd = sshclient.RunCommand(String.Format("xfs_growfs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
                        }
                        else if (_part_fs_type == "ext4" || _part_fs_type == "ext3")
                        {
                            cmd = sshclient.RunCommand(String.Format("resize2fs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
                        }
                    }
                }
                //create partitions for all the other volumes if they exist
                string[] _disk_id = new string[] { "a", "b", "c", "d", "e", "f", "h", "k", "l", "m", "n" };

                foreach (int _disk_index in _target_workload.workloadvolumes.Where(x => x.diskindex > 0).Select(x => x.diskindex).Distinct())
                {

                    cmd = sshclient.RunCommand(String.Format("echo -e 'n\np\n1\n\n\nw' | fdisk /dev/sd{0}", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("partprobe /dev/sd{0}", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("pvcreate /dev/sd{0}1", _disk_id[_disk_index]));
                    cmd = sshclient.RunCommand(String.Format("vgcreate {0} /dev/sd{1}1", String.Format("vg-{0}", _disk_id[_disk_index]), _disk_id[_disk_index]));

                    foreach (MRPWorkloadVolumeType _volume in _target_workload.workloadvolumes.ToList().Where(x => x.diskindex == _disk_index).OrderBy(x => x.driveletter))
                    {

                        String _volume_group = String.Format("vg-{0}", _disk_id[_disk_index]);
                        String _volume_name = String.Format("logical_vol{0}", _volume.driveletter.Replace("/", "_"));

                        await MRMPServiceBase._mrmp_api.task().progress(_task_id, String.Format("Creating volume /dev/{0}/{1}", _volume_group, _volume_name), ReportProgress.Progress(_start_progress, _end_progress, 20 + _disk_index));

                        //create physical volume for new partition
                        cmd = sshclient.RunCommand(String.Format("lvcreate -L{0}G -n {1} {2}", _volume.volumesize, _volume_name, _volume_group));
                        if (_part_fs_type == "")
                        {
                            _part_fs_type = "ext4";
                        }
                        cmd = sshclient.RunCommand(String.Format("mkfs.{2} /dev/{0}/{1}", _volume_group, _volume_name, _part_fs_type));

                        cmd = sshclient.RunCommand(String.Format("mkdir {0}", _volume.driveletter));
                        cmd = sshclient.RunCommand(String.Format("echo '/dev/{0}/{1} {2} {3} defaults 0 2' >> /etc/fstab",
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
