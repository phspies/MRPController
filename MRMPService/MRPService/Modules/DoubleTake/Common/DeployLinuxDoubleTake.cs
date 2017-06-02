﻿using MRMPService.MRMPService.Log;
using System;
using System.IO;
using System.Threading;
using MRMPService.Utilities;
using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;
using MRMPService.Modules.MRMPPortal.Contracts;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Threading.Tasks;

namespace MRMPService.Modules.DoubleTake.Common
{
    partial class ModuleCommon
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
        public static void DeployLinuxDoubleTake(MRPTaskType _task, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            dt_server_type server_type = dt_server_type.source;

            try
            {
                MRPWorkloadType _working_workload = new MRPWorkloadType();
                int _counter = 0;
                while (true)
                {
                    switch (server_type)
                    {
                        case dt_server_type.source:
                            _working_workload = _source_workload;
                            _counter = 1;
                            break;
                        case dt_server_type.target:
                            _working_workload = _target_workload;
                            _counter = 50;
                            break;
                    }

                    _task.progress(String.Format("Starting DT deploying process on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 1));
                    string _contactable_ip = _working_workload.working_ipaddress(true);
                    _password = _working_workload.get_credential.decrypted_password;

                    KeyboardInteractiveAuthenticationMethod _keyboard_authentication = new KeyboardInteractiveAuthenticationMethod(_working_workload.get_credential.username);
                    _keyboard_authentication.AuthenticationPrompt += new EventHandler<AuthenticationPromptEventArgs>(HandleKeyEvent);
                    PasswordAuthenticationMethod _password_authentication = new PasswordAuthenticationMethod(_working_workload.get_credential.username, _password);
                    ConnectionInfo ConnNfo = new ConnectionInfo(_contactable_ip, 22, _working_workload.get_credential.username, new AuthenticationMethod[] { _keyboard_authentication, _password_authentication });

                    string localbinarypath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Double-Take", "Linux");
                    String localfilepath = null;
                    if (_target_workload.osedition.Contains("CENTOS") || _target_workload.osedition.Contains("REDHAT"))
                    {
                        localfilepath = Directory.GetFiles(localbinarypath, "*.rpm").First();
                    }
                    else if (_target_workload.osedition.Contains("SUSE") || _target_workload.osedition.Contains("UBUNTU"))
                    {
                        localfilepath = Directory.GetFiles(localbinarypath, "*.deb").First();
                    }

                    string remotetemppath = (String.IsNullOrEmpty(_working_workload.deploymentpolicy.dt_linux_temppath) ? "/dt_tmp" : _working_workload.deploymentpolicy.dt_linux_temppath);

                    if (!File.Exists(localfilepath))
                    {
                        throw new Exception(String.Format("Couldn't locate required installation file(s) {0}", localfilepath));
                    }

                    String _dt_installed_version = "";
                    String _dt_available_version = RemoveExtraText(Path.GetFileNameWithoutExtension(localfilepath));

                    using (var sshclient = new SshClient(ConnNfo))
                    {
                        sshclient.Connect();
                        var _version_cmd = sshclient.RunCommand("/usr/bin/DT -v");
                        if (_version_cmd.ExitStatus != 0)
                        {
                            _task.progress(string.Format("It's a fresh install; no Double-Take version found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 5));
                        }
                        else
                        {
                            _dt_installed_version = RemoveExtraText(_version_cmd.Result).Trim();
                            int versionCompare = Versions.Compare(_dt_available_version, _dt_installed_version);
                            if (versionCompare <= 0)
                            {
                                _task.progress(String.Format("Product version being PushInstalled is same or less than the version ({0}) installed on {1}", _dt_installed_version, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 6));
                                ProductVersionModel _installed_dt_version;
                                using (Doubletake _dt = new Doubletake(null, _working_workload))
                                {
                                    try
                                    {
                                        _installed_dt_version = (_dt.management().GetProductInfo()).ManagementServiceVersion;
                                        _task.progress(String.Format("Double-Take installed and running on {0} with version {1}.{2}.{3}", _working_workload.hostname, _installed_dt_version.Major, _installed_dt_version.Minor, _installed_dt_version.Build), ReportProgress.Progress(_start_progress, _end_progress, _counter + 7));
                                    }
                                    catch (Exception ex)
                                    {
                                        _task.progress(String.Format("Double-Take installed on {0} but cannot be contacted: {1} {2}", _working_workload.hostname, ex.Message, ex.InnerException.InnerException.InnerException.Message), ReportProgress.Progress(_start_progress, _end_progress, _counter + 7));
                                    }
                                }
                                if (server_type == dt_server_type.target)
                                {
                                    break;
                                }
                                else if (server_type == dt_server_type.source)
                                {
                                    server_type = dt_server_type.target;
                                    continue;
                                }
                            }
                            else
                            {
                                _task.progress(String.Format("Upgrading Double-Take version {0} on {1} to version {2}", _dt_installed_version, _working_workload.hostname, _dt_available_version), ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));
                                var _lsb_install = sshclient.CreateCommand("/etc/init.d/DT halt");
                            }
                        }
                        _task.progress("Copy Linux binary to workload", ReportProgress.Progress(_start_progress, _end_progress, _counter + 11));

                        using (var sftp = new SftpClient(ConnNfo))
                        {
                            sftp.Connect();
                            try
                            {
                                sftp.ChangeDirectory(remotetemppath);
                            }
                            catch (SftpPathNotFoundException)
                            {
                                sftp.CreateDirectory(remotetemppath);
                                sftp.ChangeDirectory(remotetemppath);
                            }
                            if (!sftp.Exists(remotetemppath + "/" + Path.GetFileName(localfilepath)))
                            {
                                var fileStream = new FileStream(localfilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                if (fileStream != null)
                                {
                                    sftp.UploadFile(fileStream, Path.GetFileName(localfilepath), null);
                                    fileStream.Close();
                                    _task.progress(String.Format("Complete binaries copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 12));
                                }
                            }
                        }
                        //test and disable SELinux
                        bool _workload_rebooted = false;
                        _task.progress(String.Format("Testing if SELinux is enabled on workload {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 13));

                        var _selinux_test_command = sshclient.RunCommand("selinuxenabled");
                        if (_selinux_test_command.ExitStatus == 0)
                        {
                            _task.progress(String.Format("SELinux is enabled on workload {0}, disabling it", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 14));

                            var _selinux_disable_command = sshclient.RunCommand("sed -i 's/SELINUX=enforcing/SELINUX=disabled/g' /etc/selinux/config");
                            if (_selinux_test_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("SELinux disabling task failed on {0} : {1}", _working_workload.hostname, _selinux_disable_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 15));
                            }
                            else
                            {
                                _workload_rebooted = true;
                                _task.progress(String.Format("SELinux disabling task successed on {0}. Rebooting workload for change to take affect", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                                //init 6 results in a dropped ssh connection. catch will make sure the code completes as expected.
                                try
                                {
                                    var _reboot_command = sshclient.RunCommand("init 6");
                                    _task.progress(String.Format("Reboot failed on {0} : {1}. Please reboot workload", _working_workload.hostname, _reboot_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 17));
                                    throw new Exception(String.Format("Reboot failed on {0} : {1}. Please reboot workload", _working_workload.hostname, _reboot_command.Result));
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        //if we had to reboot the workload because we disabled SELinux
                        if (_workload_rebooted)
                        {
                            _task.progress(String.Format("Waiting for {0}to become available after reboot", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 18));
                            int _connection_retries = 6;
                            while (true)
                            {
                                try
                                {
                                    sshclient.Connect();

                                    _task.progress(String.Format("{0} is now available after reboot", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 19));

                                    _task.progress(String.Format("Testing if SELinux is disabled on workload {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 20));

                                    sshclient.Connect();
                                    _selinux_test_command = sshclient.RunCommand("selinuxenabled");
                                    if (_selinux_test_command.ExitStatus == 1)
                                    {
                                        _task.progress(String.Format("SELinux is disabled on workload {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 21));
                                    }
                                    else
                                    {
                                        _task.progress(String.Format("SELinux is still enabled on {0}. Please disable by manually", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 22));
                                        throw new Exception(String.Format("SELinux is still enabled on {0}. Please disable by manually", _working_workload.hostname));
                                    }
                                    break;
                                }
                                catch (Exception)
                                {
                                    if (_connection_retries-- == 0)
                                    {
                                        _task.progress(String.Format("Cannot connect to {0} after reboot.", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 23));
                                        throw new Exception(String.Format("Cannot connect to {0} after reboot.", _working_workload.hostname));
                                    }
                                    Thread.Sleep(new TimeSpan(0, 0, 10));
                                }
                            }

                            _task.progress(String.Format("Checking if LSB is installed on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 24));

                            var _lsb_command = sshclient.RunCommand("lsb_release -a");
                            if (_lsb_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("LSB not installed on {0}, installing it", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 25));

                                string _lsb_install_command = "";
                                if (_target_workload.osedition.Contains("CENTOS") || _target_workload.osedition.Contains("REDHAT"))
                                {
                                    _lsb_install_command = "yum -y install redhat-lsb";
                                }
                                else if (_target_workload.osedition.Contains("SUSE"))
                                {
                                    _lsb_install_command = "zypper in lsb-base";
                                }
                                else if (_target_workload.osedition.Contains("UBUNTU"))
                                {
                                    _lsb_install_command = "apt-get --yes install lsb-base";
                                }
                                var _lsb_install = sshclient.RunCommand(_lsb_install_command);

                                if (_lsb_install.ExitStatus != 0)
                                {
                                    _task.progress(String.Format("LSB installation failed {0}", _lsb_install.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));
                                    throw new Exception(String.Format("LSB installation failed on {0}", _working_workload.hostname));
                                }
                                else
                                {
                                    _task.progress(String.Format("LSB installation succeeded"), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));
                                }
                            }
                            else
                            {
                                _task.progress(String.Format("LSB in installed on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 27));
                            }
                            _task.progress(String.Format("Checking if dmidecode is installed on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 28));

                            var _dmidecode_command = sshclient.RunCommand("dmidecode");
                            if (_dmidecode_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("Dmidecode not installed on {0}, installing it", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 29));

                                string _dmidecode_install_command = "";
                                if (_target_workload.osedition.Contains("CENTOS") || _target_workload.osedition.Contains("REDHAT"))
                                {
                                    _dmidecode_install_command = "yum -y install dmidecode";
                                }
                                else if (_target_workload.osedition.Contains("SUSE"))
                                {
                                    _dmidecode_install_command = "rpm install dmidecode";
                                }
                                else if (_target_workload.osedition.Contains("UBUNTU"))
                                {
                                    _dmidecode_install_command = "apt-get -y install dmidecode";
                                }
                                var _dmidecode_install = sshclient.RunCommand(_dmidecode_install_command);

                                if (_dmidecode_install.ExitStatus != 0)
                                {
                                    _task.progress(String.Format("Dmidecode installation failed {0}", _dmidecode_install.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 30));
                                    throw new Exception(String.Format("Dmidecode installation failed on {0}", _working_workload.hostname));
                                }
                                else
                                {
                                    _task.progress(String.Format("Dmidecode installation succeeded"), ReportProgress.Progress(_start_progress, _end_progress, _counter + 31));
                                }
                            }
                            else
                            {
                                _task.progress(String.Format("Dmidecode is installed on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 32));
                            }

                            _task.progress(String.Format("Complete copy and requirements process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 33));


                            _task.progress(String.Format("Starting Double-Take installer on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 34));

                            String install_file = remotetemppath + "/" + Path.GetFileName(localfilepath);
                            string _install_cmd = "";
                            if (_target_workload.osedition.Contains("CENTOS") || _target_workload.osedition.Contains("REDHAT"))
                            {
                                _install_cmd = String.Format("yum -y install {0}", install_file);
                            }
                            else if (_target_workload.osedition.Contains("SUSE") || _target_workload.osedition.Contains("UBUNTU"))
                            {
                                _install_cmd = String.Format("dpkg -i {0}", install_file);
                            }
                            var _dt_install_command = sshclient.RunCommand(_install_cmd);
                            if (_dt_install_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("Error installing Double-Take on {0} : {1}", _working_workload.hostname, _dt_install_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 35));
                                throw new Exception(String.Format("Error installing Double-Take on {0} : {1}", _working_workload.hostname, _dt_install_command.Result));
                            }
                            else
                            {
                                _task.progress(String.Format("Successfully installed Double-Take on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 35));
                            }
                            _task.progress(String.Format("Accepting Double-Take license agreement on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 36));

                            var _dt_accept_command = sshclient.RunCommand("DTSetup -E YES");
                            if (_dt_accept_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("Error accepting license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_accept_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 37));
                                throw new Exception(String.Format("Error accepting license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_accept_command.Result));
                            }
                            else
                            {
                                _task.progress(String.Format("Successfully accepted license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_accept_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 37));
                            }
                            _task.progress(String.Format("Starting Double-Take services on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 40));

                            var _dt_start_command = sshclient.RunCommand("/etc/init.d/DT start");
                            if (_dt_start_command.ExitStatus != 0)
                            {
                                _task.progress(String.Format("Error accepting license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_start_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 38));
                                throw new Exception(String.Format("Error accepting license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_start_command.Result));
                            }
                            else
                            {
                                _task.progress(String.Format("Successfully accepted license agreement for Double-Take on {0} : {1}", _working_workload.hostname, _dt_start_command.Result), ReportProgress.Progress(_start_progress, _end_progress, _counter + 38));
                            }
                            sshclient.Disconnect();
                        }

                        //Verify DT Installation
                        _task.progress(String.Format("Verify DT connectivity on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress,  _counter + 39));

                        //Verify if the management service of Double-Take is running
                        // to determine that the software is installed properly
                        ProductVersionModel _dt_version = new ProductVersionModel();
                        using (Doubletake _dt = new Doubletake(null, _working_workload))
                        {
                            try
                            {
                                _dt_version = (_dt.management().GetProductInfo()).ManagementServiceVersion;
                                if (_dt_version == null)
                                {
                                    _task.progress(String.Format("Double-Take installed on {0} but cannot be contacted", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 40));
                                    server_type = dt_server_type.target;
                                }
                                _task.progress(String.Format("Double-Take version {0}.{1}.{2} has successfully installed on workload {3} ", _dt_version.Major, _dt_version.Minor, _dt_version.Build, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 41));
                            }
                            catch (Exception ex)
                            {
                                _task.progress(String.Format("Double-Take installed on {0} but cannot be contacted: {1} {2}", _working_workload.hostname, ex.Message, ex.InnerException.InnerException.InnerException.Message), ReportProgress.Progress(_start_progress, _end_progress, _counter + 42));
                                server_type = dt_server_type.target;
                            }
                        }

                        //if we done with the target, then we done with the job...
                        if (server_type == dt_server_type.target)
                        {
                            break;
                        }
                        //once we have the source deployed, move to the target
                        server_type = dt_server_type.target;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(string.Format("Double-Take installation failed: {0}", ex.ToString()), Logger.Severity.Error);
                throw new Exception(String.Format("Double-Take installation failed: {0}", ex.Message));

            }
            _task.progress("Completed Double-Take deployment", ReportProgress.Progress(_start_progress, _end_progress, 99));

        }
        static private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
