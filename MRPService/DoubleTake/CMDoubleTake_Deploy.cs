using DoubleTake.Core.Contract;
using Microsoft.Win32;
using MRPService.CloudMRP.Classes.Static_Classes;
using MRPService.DoubleTake;
using MRPService.LocalDatabase;
using MRPService.MRPService.Types.API;
using MRPService.Portal;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRPService.DoubleTake
{
    class CMDoubleTake_Deploy
    {
        public static void dt_deploy(MRPTaskType payload)
        {
            CloudMRPPortal _mrp_portal = new CloudMRPPortal();

            try
            {
                LocalDB _db = new LocalDB();
                MRPTaskWorkloadType _source_workload = payload.submitpayload.source;
                MRPTaskWorkloadType _target_workload = payload.submitpayload.target;

                int _counter = 10;
                foreach (MRPTaskWorkloadType _working_workload in (new List<MRPTaskWorkloadType> { _source_workload, _target_workload }))
                {
                    string remoteTempLocation = _working_workload.deploymentpolicy.dt_installpath;
                    Credential _workload_credentials = _db.Credentials.FirstOrDefault(x => x.id == _source_workload.credential_id);
                    string _contactable_ip = Connection.find_working_ip(_source_workload.iplist, true);
                    if (_contactable_ip == null)
                    {
                        _mrp_portal.task().failcomplete(payload, String.Format("Cannot contant workload {0}", _working_workload.hostname));
                        return;
                    }

                    _mrp_portal.task().progress(payload, "Get remote architecture", _counter + 10);

                    string systemArchitecture = null;
                    //Determine if the setup to be installed is 32 bit or 64 bit
                    string keyString = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
                    using (Impersonation.LogonUser((String.IsNullOrWhiteSpace(_workload_credentials.domain) ? "." : _workload_credentials.domain), _workload_credentials.username, _workload_credentials.password, LogonType.NewCredentials))
                    {
                        #region Detect Target Architecture
                        RegistryKey rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _contactable_ip);
                        RegistryKey key = rk.OpenSubKey(keyString);
                        if (key != null)
                        {
                            string architecture = (string)key.GetValue("PROCESSOR_ARCHITECTURE");
                            if (architecture.Contains("64"))
                                systemArchitecture = "X64";
                            else
                                systemArchitecture = "i386";
                        }
                        else
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Cannot determine remote achitecture for {0}", _working_workload.hostname));
                        }
                        _mrp_portal.task().progress(payload, String.Format("{0} is of type {1} architecture", _working_workload.hostname, systemArchitecture), _counter + 15);

                        //In case of an upgrade scenario, check if the version being install is same as the one on remote machine
                        //In case the two versions are the same, throw an error

                        string RemoteFilePath = @"\\" + _contactable_ip + @"\C$\Program Files\Vision Solutions\Double-Take\" + systemArchitecture + @"\setup.exe";

                        FileVersionInfo remoteFileVersion = null;
                        if (File.Exists(RemoteFilePath))
                        {
                            remoteFileVersion = FileVersionInfo.GetVersionInfo(RemoteFilePath);
                            _mrp_portal.task().progress(payload, String.Format("Double-Take version on {0} : {1}", _working_workload.hostname, remoteFileVersion), _counter + 20);
                        }
                        else
                        {
                            _mrp_portal.task().progress(payload, string.Format("It's a fresh install; no Double-Take version found on {0}", _working_workload.hostname), _counter + 25);
                        }

                        string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\" + systemArchitecture;
                        string LocalPath = Path.Combine(localConfigFilePath, @"setup.exe");

                        FileVersionInfo localFileVersion;
                        if (File.Exists(LocalPath))
                        {
                            localFileVersion = FileVersionInfo.GetVersionInfo(LocalPath);
                            _mrp_portal.task().progress(payload, String.Format("Double-Take {0} being installed on {1}", localFileVersion, _working_workload.hostname), _counter + 30);
                        }
                        else
                        {
                            _mrp_portal.task().failcomplete(payload, String.Format("Couldn't locate required install file(s) {0}", LocalPath));
                            return;
                        }
                        if (remoteFileVersion != null)
                        {
                            int versionCompare = CompareVersions(localFileVersion.ProductVersion, remoteFileVersion.ProductVersion);
                            if (versionCompare <= 0)
                            {
                                _mrp_portal.task().progress(payload, String.Format("Product version being PushInstalled is same or less than the version ({0}) installed on {1}", localFileVersion, _working_workload.hostname), _counter + 35);
                                _mrp_portal.task().successcomplete(payload);
                                return;
                            }
                        }
                        #endregion

                        #region Copy files process
                        //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
                        _mrp_portal.task().progress(payload, String.Format("Copy files to {0} on {1} ({2})", remoteTempLocation, _working_workload.hostname, systemArchitecture), _counter + 36);
                        remoteTempLocation = remoteTempLocation.Replace(':', '$');
                        string _target_workload_temp_path = Path.Combine(_contactable_ip, remoteTempLocation);
                        _target_workload_temp_path = @"\\" + _target_workload_temp_path + @"\" + systemArchitecture;
                        var endTime = DateTime.Now.AddMinutes(1);
                        if (Directory.Exists(_target_workload_temp_path))
                        {
                            break;
                        }
                        else
                        {
                            Directory.CreateDirectory(_target_workload_temp_path);
                            if (Directory.Exists(_target_workload_temp_path))
                            {
                                break;
                            }
                        }
                        string path = @"C:\Program Files\Vision Solutions\Double-Take";

                        string localFilePath = Path.Combine(path, systemArchitecture);
                        localFilePath = Path.Combine(localFilePath, @"setup.exe");
                        if (!File.Exists(localFilePath))
                        {
                            Console.Error.Write("Couldn't locate required installation file(s) {0} \n", localFilePath);
                        }

                        remoteTempLocation = remoteTempLocation.Replace(':', '$');
                        string setupFileOnWorkload = _target_workload_temp_path + @"\setup.exe";

                        if (File.Exists(setupFileOnWorkload))
                            File.SetAttributes(setupFileOnWorkload, FileAttributes.Normal);

                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        File.Copy(localFilePath, setupFileOnWorkload, true);

                        string configFileOnWorkload = @"\\" + _target_workload_temp_path + @"\DTSetup.ini";
                        File.WriteAllLines(configFileOnWorkload, BuildINIFile(_working_workload.deploymentpolicy).ConvertAll(Convert.ToString));

                        #endregion

                        #region Start Remote Installer
                        _mrp_portal.task().progress(payload, "Starting installer on remote workload", 80);


                        //Invoke install process on the remote machine
                        remoteTempLocation = remoteTempLocation.Replace('$', ':');
                        var setupPath = Path.Combine(remoteTempLocation, systemArchitecture);

                        //Create connection object to remote machine
                        ConnectionOptions connOptions = new ConnectionOptions();
                        connOptions.Impersonation = ImpersonationLevel.Impersonate;
                        connOptions.Authentication = AuthenticationLevel.Default;
                        connOptions.EnablePrivileges = true;
                        connOptions.Username = (_workload_credentials.domain == null ? "." : _workload_credentials.domain) + @"\" + _workload_credentials.username;
                        connOptions.Password = _workload_credentials.password;

                        //var configPath = @"C:\DTSetup";
                        ManagementScope scope = new ManagementScope(@"\\" + _contactable_ip + @"\root\CIMV2", connOptions);
                        scope.Connect();

                        string installCmd = @"cmd.exe /c " + setupPath + "\\setup.exe /s /v\"DTSETUPINI=\\\"" + remoteTempLocation + "\\" + "DTSetup.ini\\\" /qn /l*v+ " + setupPath + "\\Repinst.log";

                        Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
                        installCmdParams["CommandLine"] = installCmd;
                        installCmdParams["CurrentDirectory"] = setupPath;

                        Dictionary<string, object> returnValues = new Dictionary<string, object>();
                        ManagementPath wmiObjectPath = new ManagementPath("Win32_Process");
                        ObjectGetOptions ogo = new ObjectGetOptions();
                        ManagementBaseObject returnValue;
                        using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
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
                        if (returnValues != null)
                        {
                            processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
                        }

                        if (processId == 0)
                        {
                            _mrp_portal.task().failcomplete(payload, "Process ID not found");
                            return;
                        }
                        //Wait for the process to complete
                        _mrp_portal.task().progress(payload, "Wait for remote installer to complete", 90);

                        DateTime installEndTime = DateTime.Now.AddSeconds(2700);
                        Process process;

                        while (DateTime.Now < installEndTime)
                        {
                            try
                            {
                                process = Process.GetProcessById(processId, _contactable_ip);
                            }
                            catch (Exception)
                            {
                                Console.Write("Install process completed \n");
                                break;
                            }
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                        if (installEndTime <= DateTime.Now)
                        {
                            Console.Error.Write("Timed out while waiting for install \n");
                        }

                        #endregion


                        //Verify if the management service of Double-Take is running
                        // to determine that the software is installed properly
                        MRPDoubleTake _dt = new MRPDoubleTake(null, _working_workload.id);
                        ProductVersion _dt_version = _dt.common().ManagementService(Types.CMWorkloadType.Target).GetProductInfo().ManagementServiceVersion;

                        if (_dt_version == null)
                        {
                            _mrp_portal.task().failcomplete(payload, "Cannot determine installed version of Double-Take");
                            return;
                        }
                        _mrp_portal.task().successcomplete(payload, String.Format("Double-Take version {0}.{1}.{2} has successfully installed on workload {3} ", _dt_version.Major, _dt_version.Minor, _dt_version.Build, _working_workload.hostname));


                    }
                }
            }
            catch (SecurityException ex)
            {
                _mrp_portal.task().failcomplete(payload, string.Format("Permission denied;Cannot access install process on remote machine; {0}", ex.Message));
            }
            catch (Exception ex)
            {
                _mrp_portal.task().failcomplete(payload, ex.Message);
            }
        }

        private static ProductVersion ValidateManagementServiceRunning()
        {
            ProductVersion version = null;
            var endTime = DateTime.Now.AddMinutes(2);
            while (DateTime.Now < endTime)
            {
                try
                {
                    string uri = @"http://" + workload + @":6325/DoubleTake/Common/Contract/ManagementService";
                    ChannelFactory<IManagementService> mgmtServiceFactory = new ChannelFactory<IManagementService>(
                        "DefaultBinding_IManagementService_IManagementService",
                        new EndpointAddress(new Uri(uri))
                        );
                    mgmtServiceFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential { UserName = username, Password = password, Domain = domain };
                    mgmtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

                    IManagementService mgmtServiceMgr = mgmtServiceFactory.CreateChannel();
                    version = mgmtServiceMgr.GetProductInfo().ManagementServiceVersion;

                    if (version != null)
                    {
                        break;
                    }
                }
                catch (EndpointNotFoundException ex)
                {
                    Console.Error.Write("Could not contact management service: {0} \n", ex.Message);

                }
                Thread.Sleep(1000);
            }
            return version;
        }
        private static bool WaitForInstallToFinish(int processId)
        {
            bool completed = false;

            installEndTime = DateTime.Now.AddSeconds(InstallWaitTimeoutInSeconds);
            Process process;
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {
                while (DateTime.Now < installEndTime)
                {
                    try
                    {
                        process = Process.GetProcessById(processId, workload);
                    }
                    catch (Exception)
                    {
                        Console.Write("Install process completed \n");
                        completed = true;
                        break;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
                if (installEndTime <= DateTime.Now)
                {
                    Console.Error.Write("Timed out while waiting for install \n");
                }

                return completed;
            }
        }

        private static List<String> BuildINIFile(MRPTaskDeploymentpolicyType _deployment_policy)
        {
            List<String> _setup_file = new List<string>();
            _setup_file.Add("[Config]");
            _setup_file.Add("DTSETUPTYPE=DTSO");
            _setup_file.Add("DTACTIVATIONCODE="+ _deployment_policy.activation_code);
            _setup_file.Add("DOUBLETAKEFOLDER=" + '"' + _deployment_policy.dt_installpath + '"');
            _setup_file.Add("QMEMORYBUFFERMAX=" + _deployment_policy.dt_max_memory);
            _setup_file.Add("DISKQUEUEFOLDER=" + '"' + _deployment_policy.dt_queue_folder +'"');
            switch(_deployment_policy.dt_queue_scheme)
            {
                case 0:
                    _setup_file.Add("DISKQUEUEMAXSIZE=0");
                    break;
                case 1:
                    _setup_file.Add("DISKQUEUEMAXSIZE=UNLIMITED");
                    break;
                case 2:
                    _setup_file.Add("DISKQUEUEMAXSIZE=" + _deployment_policy.dt_queue_limit_disk_size.ToString());
                    break;
            }
            _setup_file.Add("DISKFREESPACEMIN=" + _deployment_policy.dt_queue_min_disk_free_size);
            _setup_file.Add("DTSERVICESTARTUP=1");
            _setup_file.Add("PORT=6320");
            _setup_file.Add("WINFW_CONFIG_OPTION=ALL");
            _setup_file.Add("LICENSE_ACTIVATION_OPTION=1");

            return _setup_file;
        }
    private static bool CopyRequiredInstallationFiles(string selection)
        {
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {

                string path = @"C:\Program Files\Vision Solutions\Double-Take";

                string localFilePath = Path.Combine(path, selection);
                localFilePath = Path.Combine(localFilePath, @"setup.exe");
                if (!File.Exists(localFilePath))
                {
                    Console.Error.Write("Couldn't locate required installation file(s) {0} \n", localFilePath);
                    return false;
                }

                remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                string workloadPath = Path.Combine(workload, remoteInstallFiles);
                workloadPath = @"\\" + workloadPath + @"\" + selection;
                string setupFileOnWorkload = workloadPath + @"\setup.exe";

                if (File.Exists(setupFileOnWorkload))
                    File.SetAttributes(setupFileOnWorkload, FileAttributes.Normal);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                CloudMRP.task().progress(_payload, String.Format("Copy installation files to {0} on {1}", workloadPath, workload), 57);

                File.Copy(localFilePath, setupFileOnWorkload, true);
                CloudMRP.task().progress(_payload, String.Format("Setup file copied successfully {0}", workload), 58);

                return true;
            }
        }
        private static bool CopyConfigFile()
        {
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {
                string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\DTSetup.ini";
                if (!File.Exists(localConfigFilePath))
                {
                    CloudMRP.task().failcomplete(_payload, String.Format("Couldn't locate required configuration file(s) {0}", localConfigFilePath));
                    return false;
                }
                remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                string workloadPath = Path.Combine(workload, remoteInstallFiles);
                string configFileOnWorkload = @"\\" + workloadPath + @"\DTSetup.ini";

                if (File.Exists(configFileOnWorkload))
                    File.SetAttributes(configFileOnWorkload, FileAttributes.Normal);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                File.Copy(localConfigFilePath, configFileOnWorkload, true);

                CloudMRP.task().progress(_payload, String.Format("Configuration file copied successfully {0}", configFileOnWorkload));

                return true;
            }
        }
        static int CompareVersions(string sa, string sb)
        {
            Func<string, int?> parse = s => { int ret; return int.TryParse(s, out ret) ? (int?)ret : null; };

            Func<string, IEnumerable<int>> f = s => s.Split('.').Select(t => int.Parse(t));

            var diff = f(sa).Zip(f(sb), (a, b) => new { a, b }).FirstOrDefault(x => x.a != x.b);

            return diff == null ? 0 : diff.a < diff.b ? -1 : 1;
        }

    }
}
