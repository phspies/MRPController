using Microsoft.Win32;
using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security;
using System.Threading;
using MRMPService.Utilities;
using DoubleTake.Web.Models;
using MRMPService.MRMPDoubleTake;
using MRMPService.MRMPAPI.Types.API;
using System.Linq;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace MRMPService.Tasks.DoubleTake
{
    partial class Deploy
    {
        public static async void DeployLinuxDoubleTake(string _task_id, MRPWorkloadType _source_workload, MRPWorkloadType _target_workload, float _start_progress, float _end_progress)
        {
            dt_server_type server_type = dt_server_type.source;

            MRMPAPI.MRMP_ApiClient _mrp_portal = new MRMPAPI.MRMP_ApiClient();

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

                    _mrp_portal.task().progress(_task_id, String.Format("Starting DT deploying process on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 5));

                    string _contactable_ip = null;
                    using (Connection _connection = new Connection())
                    {
                        _contactable_ip = _connection.FindConnection(_working_workload.iplist, true);
                    }
                    if (_contactable_ip == null)
                    {
                        _mrp_portal.task().failcomplete(_task_id, String.Format("Cannot contant workload {0}", _working_workload.hostname));
                        server_type = dt_server_type.target;
                        continue;
                    }

                    _mrp_portal.task().progress(_task_id, "Copy Linux binary to workload", ReportProgress.Progress(_start_progress, _end_progress, _counter + 10));

                    ConnectionInfo ConnNfo = new ConnectionInfo(_contactable_ip, 22, _working_workload.credential.username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(_working_workload.credential.username, _working_workload.credential.encrypted_password) });

                    string localbinarypath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Double-Take", "Linux");
                    String localfilepath = Directory.GetFiles(localbinarypath, "*.deb").First();

                    if (!File.Exists(localfilepath))
                    {
                        _mrp_portal.task().failcomplete(_task_id, String.Format("Couldn't locate required installation file(s) {0}", localfilepath));
                        return;
                    }
                    using (var sftp = new SftpClient(ConnNfo))
                    {
                        sftp.Connect();
                        try
                        {
                            sftp.ChangeDirectory(_working_workload.deploymentpolicy.dt_linux_temppath);
                        }
                        catch (SftpPathNotFoundException)
                        {
                            sftp.CreateDirectory(_working_workload.deploymentpolicy.dt_linux_temppath);
                            sftp.ChangeDirectory(_working_workload.deploymentpolicy.dt_linux_temppath);
                        }

                        var fileStream = new FileStream(localfilepath, FileMode.Open);
                        if (fileStream != null)
                        {
                            sftp.UploadFile(fileStream, Path.GetFileName(localfilepath), null);
                        }
                    }
                    String _dt_installed_version = "";
                    String _dt_available_version = RemoveExtraText(Path.GetFileNameWithoutExtension(localfilepath));

                    using (var sshclient = new SshClient(ConnNfo))
                    {
                        sshclient.Connect();
                        var _version_cmd = sshclient.RunCommand("/usr/bin/DT -v");
                        if (_version_cmd.ExitStatus != 0)
                        {
                            _mrp_portal.task().progress(_task_id, string.Format("It's a fresh install; no Double-Take version found on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                        }
                        else
                        {
                            _dt_installed_version = RemoveExtraText(_version_cmd.Result).Trim();
                            int versionCompare = Versions.Compare(_dt_available_version, _dt_installed_version);

                            _mrp_portal.task().progress(_task_id, String.Format("Double-Take found on {0} : {1}", _working_workload.hostname, _dt_installed_version), ReportProgress.Progress(_start_progress, _end_progress, _counter + 16));
                            continue;
                        }

                        var cmd = sshclient.RunCommand(Path.Combine(_working_workload.deploymentpolicy.dt_linux_temppath, "mrmp_setup.sh"));
                        

                        sshclient.Disconnect();
                    }


                    _mrp_portal.task().progress(_task_id, String.Format("Complete binaries copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 21));

                    //_mrp_portal.task().progress(_task_id, String.Format("Copy configuration file to {0} on {1})", remoteTempLocation, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 25));
                    string configFileOnWorkload = @"\\" + @"\DTSetup.ini";
                    File.WriteAllLines(configFileOnWorkload, BuildINI.BuildINIFile(_working_workload.deploymentpolicy, server_type).ConvertAll(Convert.ToString));
                    _mrp_portal.task().progress(_task_id, String.Format("Complete configuration copy process for {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 26));

                    _mrp_portal.task().progress(_task_id, String.Format("Starting installer on {0}", _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 30));

                    //Wait for the process to complete
                    _mrp_portal.task().progress(_task_id, "Wait for remote installer to complete", ReportProgress.Progress(_start_progress, _end_progress, _counter + 31));

                    #region Verify DT Installation
                    _mrp_portal.task().progress(_task_id, String.Format("Verify DT connectivity on {0}", _working_workload.hostname), _counter + 40);

                    //Verify if the management service of Double-Take is running
                    // to determine that the software is installed properly
                    ProductVersionModel _dt_version;
                    using (Doubletake _dt = new Doubletake(null, _working_workload))
                    {
                        _dt_version = (await _dt.management().GetProductInfo()).ManagementServiceVersion;
                        if (_dt_version == null)
                        {
                            _mrp_portal.task().failcomplete(_task_id, "Cannot determine installed version of Double-Take");
                            server_type = dt_server_type.target;
                            continue;
                        }
                    }
                    _mrp_portal.task().progress(_task_id, String.Format("Double-Take version {0}.{1}.{2} has successfully installed on workload {3} ", _dt_version.Major, _dt_version.Minor, _dt_version.Build, _working_workload.hostname), ReportProgress.Progress(_start_progress, _end_progress, _counter + 45));

                    #endregion

                    //if we done with the target, then we done with the job...
                    if (server_type == dt_server_type.target)
                    {
                        break;
                    }
                    //once we have the source deployed, move to the target
                    server_type = dt_server_type.target;
                }
                _mrp_portal.task().successcomplete(_task_id, "Completed Double-Take deployment");
            }
            catch (Exception ex)
            {
                _mrp_portal.task().failcomplete(_task_id, ex.Message);
                Logger.log(string.Format("Cannot access install process on remote machine; {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
        static private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
