using MRPService.Portal.Types.API;
using Microsoft.Win32;
using Newtonsoft.Json;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
using System.ServiceModel;
using System.Threading;
using MRPService.Portal;
using DoubleTake.Core.Contract;
using MRPService.CMDoubleTake.Types;
using MRPService.MRPService.Types.API;

namespace MRPService.CMDoubleTake
{
    class CMDoubleTake_Common : CMDoubleTake_Core
    {
        public CMDoubleTake_Common(CMDoubleTake cmdoubletake) : base(cmdoubletake) { }

        static String workload = "";
        static string username = "";
        static string password = "";
        static string domain = "";
        static string remoteInstallFiles = @"C:\DTSetup";
        static private DateTime installEndTime;
        static public int InstallWaitTimeoutInSeconds { get { return _installWaitTimeoutInSeconds; } set { _installWaitTimeoutInSeconds = value; } }
        static private int _installWaitTimeoutInSeconds = 2700;
        static CloudMRPPortal CloudMRP = new CloudMRPPortal();
        static MRPTask tasks = null;
        static dynamic _payload = null;
//        public static void dt_getproductinformation(MRPTaskType payload)
//        {
//            try
//            {
//                CloudMRP.task().progress(payload, "Creating ManagementService process", 9);
//                IManagementService iMgrSrc = ManagementService(CMWorkloadType.Target).CreateChannel();

//                CloudMRP.task().progress(payload, "DT Data Gathering", 50);
//                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetWorkloadInfo()));
//            }
//            catch (Exception e)
//            {
//                CloudMRP.task().failcomplete(payload, e.ToString());
//            }
//        }

//        public static void dt_getimages(MRPTaskType payload)
//        {
//            CloudMRP.task().progress(payload, "DT Connection", 50);
//            ChannelFactory<IManagementService> MgtServiceFactory =
//                new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService",
//                    new EndpointAddress(BuildUrl(payload, "/DoubleTake/Common/Contract/ManagementService", 0)));
//            MRPTask tasks = new MRPTask(CloudMRP);
//            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
//            try
//            {
//                CloudMRP.task().progress(payload, "DT Data Gathering", 50);
//                CloudMRP.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetImages(null)));
//            }
//            catch (Exception e)
//            {
//                CloudMRP.task().failcomplete(payload, e.ToString());
//            }
//        }
//        public static void dt_deploy(MRPTaskType payload)
//        {
//            _payload = payload;
//            tasks = new MRPTask(CloudMRP);

//            username = payload.submitpayload.dt.username;
//            password = payload.submitpayload.dt.password;
//            domain = payload.submitpayload.dt.domain;
//            remoteInstallFiles = @"C:\Temp";

//            workload = find_working_ip(payload, 0);
//            if (string.IsNullOrEmpty(workload))
//            {
//                CloudMRP.task().failcomplete(payload, "None of the IP's responded");
//                return;
//            }
//            string remoteInstallPath;
//            IPAddress address = IPAddress.Parse(workload);
//            if (address.AddressFamily.ToString() == AddressFamily.InterNetworkV6.ToString())
//            {
//                String _workload = workload;
//                _workload = _workload.Replace(":", "-");
//                _workload = _workload.Replace("%", "s");
//                _workload = _workload + ".ipv6-literal.net";
//                remoteInstallPath = Path.Combine(_workload, remoteInstallFiles);
//                workload = _workload;
//            }
//            else
//            {
//                remoteInstallPath = Path.Combine(workload, remoteInstallFiles);
//            }

//            try
//            {
//                CloudMRP.task().progress(payload, "Remote Get Architecture", 30);

//                string systemArchitecture = null;
//                //Determine if the setup to be installed is 32 bit or 64 bit
//                string keyString = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
//                using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//                {
//                    RegistryKey rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, workload);
//                    RegistryKey key = rk.OpenSubKey(keyString);
//                    if (key != null)
//                    {
//                        string architecture = (string)key.GetValue("PROCESSOR_ARCHITECTURE");
//                        if (architecture.Contains("64"))
//                            systemArchitecture = "X64";
//                        else
//                            systemArchitecture = "i386";
//                    }
//                    else
//                    {
//                        CloudMRP.task().failcomplete(payload, String.Format("Cannot determine remote achitecture for {0}", workload));
//                    }
//                }

//                CloudMRP.task().progress(payload, String.Format("{0} is of type {1} architecture", workload, systemArchitecture), 40);

//                //In case of an upgrade scenario, check if the version being install is same as the one on remote machine
//                //In case the two versions are the same, throw an error
//                using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//                {

//                    string RemoteFilePath = @"\\" + workload + @"\C$\Program Files\Vision Solutions\Double-Take\" + systemArchitecture + @"\setup.exe";

//                    FileVersionInfo remoteFileVersion = null;
//                    if (File.Exists(RemoteFilePath))
//                    {
//                        remoteFileVersion = FileVersionInfo.GetVersionInfo(RemoteFilePath);
//                        CloudMRP.task().progress(payload, String.Format("Double-Take version on {0} : {1}", workload, remoteFileVersion), 50);
//                    }
//                    else
//                    {
//                        CloudMRP.task().progress(payload, string.Format("It's a fresh install; no Double-Take version found on {0}", workload), 50);
//                    }

//                    string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\" + systemArchitecture;
//                    string LocalPath = Path.Combine(localConfigFilePath, @"setup.exe");

//                    FileVersionInfo localFileVersion;
//                    if (File.Exists(LocalPath))
//                    {
//                        localFileVersion = FileVersionInfo.GetVersionInfo(LocalPath);
//                        CloudMRP.task().progress(payload, String.Format("Double-Take {0} being installed on {1}", localFileVersion, workload), 60);
//                    }
//                    else
//                    {
//                        CloudMRP.task().failcomplete(payload, String.Format("Couldn't locate required install file(s) {0}", LocalPath));
//                        return;
//                    }
//                    if (remoteFileVersion != null)
//                    {
//                        int versionCompare = CompareVersions(localFileVersion.ProductVersion, remoteFileVersion.ProductVersion);
//                        if (versionCompare <= 0)
//                        {
//                            CloudMRP.task().progress(payload, String.Format("Product version being PushInstalled is same or less than the version ({0}) installed on {1}", localFileVersion, workload), 60);
//                            CloudMRP.task().successcomplete(payload);
//                            return;
//                        }
//                    }
//                }

//                CloudMRP.task().progress(payload, String.Format("Copy files to {0} on {1} ({2})", remoteInstallPath, workload, systemArchitecture), 51);
//                //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
//                bool success = false;
//                switch (systemArchitecture)
//                {
//                    case "i386":
//                        {
//                            success = CreateCopyDirectory(@"i386") && CopyRequiredInstallationFiles(@"i386") && CopyConfigFile();
//                            break;
//                        }
//                    case "X64":
//                        success = CreateCopyDirectory(@"X64") && CopyRequiredInstallationFiles(@"X64") && CopyConfigFile();
//                        break;

//                }
//                if (!success)
//                {
//                    CloudMRP.task().failcomplete(payload, "Could not copy installation and/or configuration files to remote machine");
//                    return;
//                }

//                CloudMRP.task().progress(payload, "Starting installer on remote workload", 80);
//                //Invoke install process on the remote machine
//                int processId = StartInstallerProcess("Win32_Process", "Create", systemArchitecture);

//                if (processId == 0)
//                {
//                    CloudMRP.task().failcomplete(payload, "Process ID not found");
//                    return;
//                }

//                //Wait for the process to complete
//                CloudMRP.task().progress(payload, "Wait for remote installer to complete", 90);

//                bool processComplete = WaitForInstallToFinish(processId);

//                if (!processComplete)
//                {
//                    CloudMRP.task().failcomplete(payload, "Install process timed out");
//                    return;
//                }

//                //Verify if the management service of Double-Take is running
//                // to determine that the software is installed properly
//                var version = ValidateManagementServiceRunning();

//                if (version == null)
//                {
//                    CloudMRP.task().failcomplete(payload, "Cannot determine installed version of Double-Take");
//                    return;
//                }
//                CloudMRP.task().successcomplete(payload, String.Format("Double-Take version {0}.{1}.{2} has successfully installed on workload {3} ", version.Major, version.Minor, version.Build, workload));


//            }
//            catch (SecurityException ex)
//            {
//                CloudMRP.task().failcomplete(payload, string.Format("Permission denied;Cannot access install process on remote machine; {0}", ex.Message));
//            }
//            catch (Exception ex)
//            {
//                CloudMRP.task().failcomplete(payload, ex.Message);
//            }

//        }

//        private static ProductVersion ValidateManagementServiceRunning()
//        {
//            ProductVersion version = null;
//            var endTime = DateTime.Now.AddMinutes(2);
//            while (DateTime.Now < endTime)
//            {
//                try
//                {
//                    string uri = @"http://" + workload + @":6325/DoubleTake/Common/Contract/ManagementService";
//                    ChannelFactory<IManagementService> mgmtServiceFactory = new ChannelFactory<IManagementService>(
//                        "DefaultBinding_IManagementService_IManagementService",
//                        new EndpointAddress(new Uri(uri))
//                        );
//                    mgmtServiceFactory.Credentials.Windows.ClientCredential = new System.Net.NetworkCredential { UserName = username, Password = password, Domain = domain };
//                    mgmtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

//                    IManagementService mgmtServiceMgr = mgmtServiceFactory.CreateChannel();
//                    version = mgmtServiceMgr.GetProductInfo().ManagementServiceVersion;

//                    if (version != null)
//                    {
//                        break;
//                    }
//                }
//                catch (EndpointNotFoundException ex)
//                {
//                    Console.Error.Write("Could not contact management service: {0} \n", ex.Message);

//                }
//                Thread.Sleep(1000);
//            }
//            return version;
//        }
//        private static bool WaitForInstallToFinish(int processId)
//        {
//            bool completed = false;

//            installEndTime = DateTime.Now.AddSeconds(InstallWaitTimeoutInSeconds);
//            Process process;
//            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//            {
//                while (DateTime.Now < installEndTime)
//                {
//                    try
//                    {
//                        process = Process.GetProcessById(processId, workload);
//                    }
//                    catch (Exception)
//                    {
//                        Console.Write("Install process completed \n");
//                        completed = true;
//                        break;
//                    }
//                    Thread.Sleep(TimeSpan.FromSeconds(10));
//                }
//                if (installEndTime <= DateTime.Now)
//                {
//                    Console.Error.Write("Timed out while waiting for install \n");
//                }

//                return completed;
//            }
//        }
//        protected static int StartInstallerProcess(string Class, string MethodName, string systemArchitecture)
//        {
//            remoteInstallFiles = remoteInstallFiles.Replace('$', ':');
//            var setupPath = Path.Combine(remoteInstallFiles, systemArchitecture); // @"C:\DTSetup\X64";

//            //Create connection object to remote machine
//            ConnectionOptions connOptions = new ConnectionOptions();
//            connOptions.Impersonation = ImpersonationLevel.Impersonate;
//            connOptions.Authentication = AuthenticationLevel.Default;
//            connOptions.EnablePrivileges = true;
//            connOptions.Username = username;
//            connOptions.Password = password;

//            //var configPath = @"C:\DTSetup";
//            ManagementScope scope = new ManagementScope(@"\\" + workload + @"\root\CIMV2", connOptions);
//            scope.Connect();

//            string installCmd = @"cmd.exe /c " + setupPath + "\\setup.exe /s /v\"DTSETUPINI=\\\"" + remoteInstallFiles + "\\" + "DTSetup.ini\\\" /qn /l*v+ " + setupPath + "\\Repinst.log";

//            Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
//            installCmdParams["CommandLine"] = installCmd;
//            installCmdParams["CurrentDirectory"] = setupPath;

//            Dictionary<string, object> returnValues = new Dictionary<string, object>();
//            ManagementPath wmiObjectPath = new ManagementPath(Class);
//            ObjectGetOptions ogo = new ObjectGetOptions();
//            ManagementBaseObject returnValue;
//            using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
//            {
//                ManagementBaseObject inparams = mc.GetMethodParameters(MethodName);

//                if (installCmdParams != null)
//                {
//                    foreach (var p in installCmdParams)
//                    {
//                        inparams[p.Key] = p.Value;
//                    }
//                }

//                returnValue = mc.InvokeMethod(MethodName, inparams, null);

//            }

//            int processId = 0;
//            if (returnValues != null)
//            {
//                processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
//            }

//            return processId;

//        }
//        private static bool CreateCopyDirectory(string selection)
//        {
//            var success = true;
//            Exception lastException = null;

//            remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
//            string workloadPath = Path.Combine(workload, remoteInstallFiles);
//            workloadPath = @"\\" + workloadPath + @"\" + selection;
//            CloudMRP.task().progress(_payload, "Copy files to remote workload to " + workloadPath, 51);
//            var endTime = DateTime.Now.AddMinutes(1);
//            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//            {
//                while (DateTime.Now < endTime)
//                {
//                    try
//                    {
//                        if (Directory.Exists(workloadPath))
//                        {
//                            CloudMRP.task().progress(_payload, "Copy files to remote workload to " + workloadPath, 52);
//                            break;
//                        }
//                        else
//                        {
//                            Directory.CreateDirectory(workloadPath);
//                            if (Directory.Exists(workloadPath))
//                            {
//                                CloudMRP.task().progress(_payload, String.Format("Successfully created directories {0} to copy install file to on {1} \n", workloadPath, workload), 53);

//                                break;
//                            }
//                        }
//                    }
//                    catch (IOException e)
//                    {

//                        CloudMRP.task().progress(_payload, String.Format("Failed to create directory on {0}. Retrying... {1}", workload, e.ToString()), 54);
//                        lastException = e;
//                        Thread.Sleep(TimeSpan.FromSeconds(10));
//                    }
//                }
//                if (endTime <= DateTime.Now)
//                {
//                    CloudMRP.task().progress(_payload, String.Format("Timed out while waiting for directory to be created on {0}.", workload), 55);

//                    if (lastException != null)
//                    {
//                        CloudMRP.task().progress(_payload, String.Format("Problem creating directory on {0}. Error: {1} \n ", workload, lastException), 56);
//                        success = false;
//                    }
//                }

//                return success;
//            }
//        }
//        private static bool CopyRequiredInstallationFiles(string selection)
//        {
//            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//            {

//                string path = @"C:\Program Files\Vision Solutions\Double-Take";

//                string localFilePath = Path.Combine(path, selection);
//                localFilePath = Path.Combine(localFilePath, @"setup.exe");
//                if (!File.Exists(localFilePath))
//                {
//                    Console.Error.Write("Couldn't locate required installation file(s) {0} \n", localFilePath);
//                    return false;
//                }

//                remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
//                string workloadPath = Path.Combine(workload, remoteInstallFiles);
//                workloadPath = @"\\" + workloadPath + @"\" + selection;
//                string setupFileOnWorkload = workloadPath + @"\setup.exe";

//                if (File.Exists(setupFileOnWorkload))
//                    File.SetAttributes(setupFileOnWorkload, FileAttributes.Normal);

//                Thread.Sleep(TimeSpan.FromSeconds(1));
//                CloudMRP.task().progress(_payload, String.Format("Copy installation files to {0} on {1}", workloadPath, workload), 57);

//                File.Copy(localFilePath, setupFileOnWorkload, true);
//                CloudMRP.task().progress(_payload, String.Format("Setup file copied successfully {0}", workload), 58);

//                return true;
//            }
//        }
//        private static bool CopyConfigFile()
//        {
//            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
//            {
//                string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\DTSetup.ini";
//                if (!File.Exists(localConfigFilePath))
//                {
//                    CloudMRP.task().failcomplete(_payload, String.Format("Couldn't locate required configuration file(s) {0}", localConfigFilePath));
//                    return false;
//                }
//                remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
//                string workloadPath = Path.Combine(workload, remoteInstallFiles);
//                string configFileOnWorkload = @"\\" + workloadPath + @"\DTSetup.ini";

//                if (File.Exists(configFileOnWorkload))
//                    File.SetAttributes(configFileOnWorkload, FileAttributes.Normal);

//                Thread.Sleep(TimeSpan.FromSeconds(1));
//                File.Copy(localConfigFilePath, configFileOnWorkload, true);

//                CloudMRP.task().progress(_payload, String.Format("Configuration file copied successfully {0}", configFileOnWorkload));

//                return true;
//            }
//        }
//        static int CompareVersions(string sa, string sb)
//        {
//            Func<string, int?> parse = s => { int ret; return int.TryParse(s, out ret) ? (int?)ret : null; };

//            Func<string, IEnumerable<int>> f = s => s.Split('.').Select(t => int.Parse(t));

//            var diff = f(sa).Zip(f(sb), (a, b) => new { a, b }).FirstOrDefault(x => x.a != x.b);

//            return diff == null ? 0 : diff.a < diff.b ? -1 : 1;
//        }
//        private static NetworkCredential GetCredentials(dynamic payload, int type)
//        {
//            NetworkCredential credentials = null;
//            switch (type)
//            {
//                case 0:
//                    credentials = new NetworkCredential() { UserName = payload.payload.dt.username, Password = payload.payload.dt.password, Domain = payload.payload.dt.domain };
//                    break;
//                case 1:
//                    credentials = new NetworkCredential() { UserName = payload.payload.dt.source.username, Password = payload.payload.dt.source.password, Domain = payload.payload.dt.source.domain };
//                    break;
//                case 2:
//                    credentials = new NetworkCredential() { UserName = payload.payload.dt.target.username, Password = payload.payload.dt.target.password, Domain = payload.payload.dt.target.domain };
//                    break;
//            }
//            return credentials;
//        }
//        private static string find_working_ip(MRPTaskType request, int type)
//        {
//            ConnectionOptions connection = new ConnectionOptions();
//            MRPTaskSubmitpayloadType _request = request.submitpayload;
//            String ipaddresslist = null;
//            if (type == 0)
//            {
//                ipaddresslist = _request.dt.ipaddress;
//            }
//            else if (type == 1)
//            {
//                ipaddresslist = _request.dt.source.ipaddress;
//            }
//            else if (type == 2)
//            {
//                ipaddresslist = _request.dt.target.ipaddress;
//            }
//            String workingip = null;
//            Ping testPing = new Ping();
//            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
//            {
//                PingReply reply = testPing.Send(ip, 1000);
//                if (reply != null)
//                {
//                    workingip = ip;
//                    break;
//                }
//            }
//            testPing.Dispose();
//            //check for IPv6 address
//            IPAddress _check_ip = IPAddress.Parse(workingip);
//#pragma warning disable CS0436 // Type conflicts with imported type
//            if (_check_ip.AddressFamily.ToString() == AddressFamily.InterNetworkV6.ToString())
//#pragma warning restore CS0436 // Type conflicts with imported type
//            {
//                String _workingip = workingip;
//                _workingip = _workingip.Replace(":", "-");
//                _workingip = _workingip.Replace("%", "s");
//                _workingip = _workingip + ".ipv6-literal.net";
//                workingip = _workingip;
//            }
//            return workingip;
//        }
//        private static String BuildUrl(MRPTaskType request, String method, int type)
//        {
//            int portNumber = 6325;
//            string bindingScheme = "http://";
//            return new UriBuilder(bindingScheme, find_working_ip(request, type), portNumber, method).ToString();
//        }

    }
}
