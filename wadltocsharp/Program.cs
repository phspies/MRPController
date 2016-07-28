
//using DD.CBU.Compute.Api.Client;
//using DD.CBU.Compute.Api.Contracts.Network20;
//using MRMPService.Utiliies;
//using Renci.SshNet;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace wadltocsharp
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            ComputeApiClient CaaS = ComputeApiClient.GetComputeApiClient(new Uri("https://api-na.dimensiondata.com"), new NetworkCredential("dd_drs_dev", "Freetrial01!"));
//            CaaS.Login().Wait();

//            ServerType _workload = CaaS.ServerManagement.Server.GetServer(new Guid("48722984-3927-4b93-a1d1-a6dcac93e6a7")).Result;
//            ConnectionInfo ConnNfo = new ConnectionInfo(_workload.networkInfo.primaryNic.ipv6, 22, "root", new AuthenticationMethod[] { new PasswordAuthenticationMethod("root", "Qwerty1234") });


//            float root_lvm_size = 0;
//            string root_lvm_volume = "";
//            string root_portal_volume = "/";
//            float root_portal_size = 20;
//            Double root_disk_free = 0;
//            string _volumegroup = "";
//            string _part_fs_type = "";

//            using (var sshclient = new SshClient(ConnNfo))
//            {
//                sshclient.Connect();

//                using (var cmd = sshclient.CreateCommand("lsblk -lb"))
//                {
//                    cmd.Execute();
//                    if (cmd.ExitStatus != 0)
//                    {
//                        sshclient.Disconnect();
//                        throw new ArgumentException(String.Format("Error while running unix setup script: %1", cmd.Result));
//                    }

//                    string[] lines = cmd.Result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

//                    foreach (string _line in lines)
//                    {
//                        string[] vars = _line.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
//                        if (vars.Count() > 6)
//                        {
//                            if (vars[6] == root_portal_volume)
//                            {
//                                string _string = vars[0];
//                                _volumegroup = _string.Split('-')[0];
//                                root_lvm_volume = _string.Split('-')[1];
//                                root_lvm_size = float.Parse(vars[3].ToString()) / 1024 / 1024 / 1024;
//                            }
//                        }
//                        if (vars.Count() > 7)
//                        {
//                            if (vars[7] == root_portal_volume)
//                            {
//                                string _string = vars[0];
//                                _volumegroup = _string.Split('-')[0];
//                                root_lvm_volume = _string.Split('-')[1];
//                                root_lvm_size = float.Parse(vars[4].ToString()) / 1024 / 1024 / 1024;

//                            }
//                        }
//                    }

//                }
//                var _diskfree_cmd = sshclient.CreateCommand("parted /dev/sda unit GB print free | grep 'Free Space' | tail -n1 | awk '{print $3}'");
//                _diskfree_cmd.Execute();
//                root_disk_free = Convert.ToDouble(Math.Floor(Double.Parse(_diskfree_cmd.Result.Replace("GB", ""))));

//                var _fs_type_cmd = sshclient.CreateCommand("df -T / | grep -v 'Filesystem' | awk '{print $2}'");
//                _fs_type_cmd.Execute();
//                if (_fs_type_cmd.ExitStatus == 0)
//                {
//                    _part_fs_type = _fs_type_cmd.Result.Trim();
//                }

//                sshclient.Disconnect();
//            }

//            //calculate capacity shortfall
//            double _0_shortfall = Math.Ceiling(root_portal_size - root_lvm_size - root_disk_free);

//            //if we need to expand disk 0, lets do that first...
//            if (_0_shortfall > 0)
//            {
//                CaaS.ServerManagementLegacy.Server.ChangeServerDiskSize(_workload.id, _workload.disk[0].id, (_0_shortfall + _workload.disk[0].sizeGb).ToString()).Wait();
//                _workload = CaaS.ServerManagement.Server.GetServer(new Guid(_workload.id)).Result;
//                while (_workload.state != "NORMAL" && _workload.started == true)
//                {
//                    Thread.Sleep(5000);
//                    _workload = CaaS.ServerManagement.Server.GetServer(new Guid(_workload.id)).Result;
//                }
//                CaaS.ServerManagement.Server.RebootServer(new Guid(_workload.id)).Wait();
//                _workload = CaaS.ServerManagement.Server.GetServer(new Guid(_workload.id)).Result;
//                while (_workload.state != "NORMAL" && _workload.started == true)
//                {
//                    Thread.Sleep(5000);
//                    _workload = CaaS.ServerManagement.Server.GetServer(new Guid(_workload.id)).Result;
//                }
//            }
//            using (var sshclient = new SshClient(ConnNfo))
//            {
//                int retry = 3;
//                while (retry > 0)
//                {
//                    retry--;
//                    try
//                    {
//                        sshclient.Connect();
//                        break;
//                    }
//                    catch (Exception ex)
//                    {
//                    }
//                }
//                if (!sshclient.IsConnected)
//                {
//                    Console.WriteLine("Error connecting to linux server");
//                    throw new Exception("Error");
//                }
//                //create extended partition
//                var cmd = sshclient.RunCommand("echo -e 'n\ne\n4\n\n\n\nw' | fdisk /dev/sda");

//                //create new partition on number 5
//                int _create_part_number = 5;
//                cmd = sshclient.RunCommand("echo -e 'n\nl\n\n\n\nw' | fdisk /dev/sda");

//                //rescan linux device files
//                cmd = sshclient.RunCommand("partprobe /dev/sda");

//                //create physical volume for new partition
//                cmd = sshclient.RunCommand("pvcreate /dev/sda" + _create_part_number);
//                cmd = sshclient.RunCommand(String.Format("vgextend {0} /dev/sda{1}", _volumegroup, _create_part_number));
//                cmd = sshclient.RunCommand(String.Format("lvextend -L{3}G /dev/{0}/{1} /dev/sda{2}", _volumegroup, root_lvm_volume, _create_part_number, root_portal_size));                
//                if (_part_fs_type == "xfs")
//                {
//                    cmd = sshclient.RunCommand(String.Format("xfs_growfs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
//                }
//                else if (_part_fs_type == "ext4")
//                {
//                    cmd = sshclient.RunCommand(String.Format("resize2fs /dev/{0}/{1}", _volumegroup, root_lvm_volume));
//                }

//                Console.ReadKey();

//            }

//        }

//    }
//}
