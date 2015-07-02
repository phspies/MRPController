using CladesWorkerService.CaaS;
using DoubleTake.Common.Contract;
using DoubleTake.Common.Tasks;
using DoubleTake.Communication;
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
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades.Controllers
{
    class DT
    {
        static String server = "";
        static string username = "";
        static string password = "";
        static string domain = "";
        static string remoteInstallFiles = @"C:\DTSetup";
        static private DateTime installEndTime;
        static public int InstallWaitTimeoutInSeconds { get { return _installWaitTimeoutInSeconds; } set { _installWaitTimeoutInSeconds = value; } }
        static private int _installWaitTimeoutInSeconds = 2700;
        static CladesWorkerService.Clades.Clades clades = null;
        static TasksObject tasks = null;
        static dynamic _payload = null;


        public static void dt_deploy(dynamic payload)
        {
            _payload = payload;
            clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            tasks = new TasksObject(clades);

            username = payload.payload.dt.username;
            password = payload.payload.dt.password;
            domain = payload.payload.dt.domain;
            remoteInstallFiles = payload.payload.dt.deploymentpolicy.dt_temppath;

            server = find_working_ip(payload,0);
            if (string.IsNullOrEmpty(server))
            {
                clades.task().failcomplete(payload, "None of the IP's responded");
                return;
            }

            string remoteInstallPath = Path.Combine(server, remoteInstallFiles);

            try
            {
                clades.task().progress(payload, "Remote Get Architecture", 30);

                //Determine if the setup to be installed is 32 bit or 64 bit
                string systemArchitecture = GetRemoteSystemArchitecture();

                clades.task().progress(payload, "Remote Architecture Check", 40);

                //In case of an upgrade scenario, check if the version being install is same as the one on remote machine
                //In case the two versions are the same, throw an error
                CheckFileVersion(server, systemArchitecture);

                clades.task().progress(payload, "Copy files to remote server: " + remoteInstallPath, 50);
                //Copy install options in configuration file and setup files for 32 bit and 64 bit to remote machine
                bool success = CreateCopyDirectory(@"X64")
                && CreateCopyDirectory(@"i386")
                && CopyRequiredInstallationFiles(@"X64")
                && CopyRequiredInstallationFiles(@"i386")
                && CopyConfigFile();

                if (!success)
                {
                    clades.task().failcomplete(payload, "Could not copy installation and/or configuration files to remote machine");
                    return;
                }

                clades.task().progress(payload, "Starting installer on remote server", 60);
                //Invoke install process on the remote machine
                int processId = StartInstallerProcess("Win32_Process", "Create", systemArchitecture);

                if (processId == 0)
                {
                    clades.task().failcomplete(payload, "Process ID not found");
                    return;
                }

                //Wait for the process to complete
                clades.task().progress(payload, "Wait for remote installer to complete", 70);

                bool processComplete = WaitForInstallToFinish(processId);

                if (!processComplete)
                {
                    clades.task().failcomplete(payload, "Install process timed out \n");
                    return;
                }

                //Verify if the management service of Double-Take is running
                // to determine that the software is installed properly
                var version = ValidateManagementServiceRunning();

                if (version == null)
                {
                    clades.task().failcomplete(payload, "Cannot determine installed version of Double-Take");
                    return;
                }
                clades.task().successcomplete(payload, String.Format("Double-Take version {0}.{1}.{2} has successfully installed on server {3} ", version.Major, version.Minor, version.Build, server));


            }
            catch (SecurityException ex)
            {
                clades.task().failcomplete(payload, string.Format("Permission denied;Cannot access install process on remote machine; {0}", ex.Message));
            }
            catch (Exception ex)
            {
                clades.task().failcomplete(payload, ex.Message);
            }

        }
        public static void dt_getproductinformation(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            clades.task().progress(payload, "DT Connection", 50);
            String url = BuildUrl(payload, "/DoubleTake/Common/Contract/ManagementService");
            ChannelFactory<IManagementService> MgtServiceFactory = 
                new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService", new EndpointAddress(url));
            MgtServiceFactory.Credentials.Windows.ClientCredential = GetCredentials(payload,2);
            MgtServiceFactory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            try
            {
                clades.task().progress(payload, "DT Data Gathering", 50);
                clades.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetServerInfo()));
            }
            catch (Exception e)
            {
                clades.task().failcomplete(payload, e.ToString());
            }
        }
        public static void dt_getimages(dynamic payload)
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            clades.task().progress(payload, "DT Connection", 50);
            ChannelFactory<IManagementService> MgtServiceFactory =
                new ChannelFactory<IManagementService>("DefaultBinding_IManagementService_IManagementService",
                    new EndpointAddress(BuildUrl(payload, "/DoubleTake/Common/Contract/ManagementService")));
            TasksObject tasks = new TasksObject(clades);
            IManagementService iMgrSrc = MgtServiceFactory.CreateChannel();
            try
            {
                clades.task().progress(payload, "DT Data Gathering", 50);
                clades.task().successcomplete(payload, JsonConvert.SerializeObject(iMgrSrc.GetImages(null)));
            }
            catch (Exception e)
            {
                clades.task().failcomplete(payload, e.ToString());
            }
        }

        private static string find_working_ip(dynamic payload, int type)
        {
            ConnectionOptions connection = new ConnectionOptions();

            String ipaddresslist = null;
            if (type==0)
            {
                ipaddresslist = payload.payload.dt.ipaddress;
                connection.Username = payload.payload.dt.username;
                connection.Password = payload.payload.dt.password;
                connection.Authority = "ntlmdomain:" + payload.payload.dt.domain;
            } else if (type==1)
            {
                ipaddresslist = payload.payload.dt.source.ipaddress;
                connection.Username = payload.payload.dt.source.username;
                connection.Password = payload.payload.dt.source.password;
                connection.Authority = "ntlmdomain:" + payload.payload.dt.source.domain;
            }
            else if (type==2)
            {
                ipaddresslist = payload.payload.dt.target.ipaddress;
                connection.Username = payload.payload.dt.target.username;
                connection.Password = payload.payload.dt.target.password;
                connection.Authority = "ntlmdomain:" + payload.payload.dt.target.domain;
            }
            String workingip = null;
            ManagementScope scope = new ManagementScope();
            Exception error = new Exception();
            foreach (string ip in ipaddresslist.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {

                try
                {
                    scope = new ManagementScope("\\\\" + ip.Trim() + "\\root\\CIMV2", connection);
                    scope.Connect();
                    workingip = ip;
                    break;
                }
                catch (Exception e)
                {
                    error = e;
                }
            }
            if (!scope.IsConnected)
            {
                workingip = null;
            }
            return workingip;
        }

        private static DoubleTake.Core.Contract.ProductVersion ValidateManagementServiceRunning()
        {
            DoubleTake.Core.Contract.ProductVersion version = null;
            var endTime = DateTime.Now.AddMinutes(2);
            while (DateTime.Now < endTime)
            {
                try
                {
                    string uri = @"http://" + server + @":6325/DoubleTake/Common/Contract/ManagementService";
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
                        process = Process.GetProcessById(processId, server);
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

        protected static int StartInstallerProcess(string Class, string MethodName, string systemArchitecture)
        {
            remoteInstallFiles = remoteInstallFiles.Replace('$', ':');
            var setupPath = Path.Combine(remoteInstallFiles, systemArchitecture); // @"C:\DTSetup\X64";

            //Create connection object to remote machine
            ConnectionOptions connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.Authentication = AuthenticationLevel.Default;
            connOptions.EnablePrivileges = true;
            connOptions.Username = username;
            connOptions.Password = password;

            //var configPath = @"C:\DTSetup";
            ManagementScope scope = new ManagementScope(@"\\" + server + @"\root\CIMV2", connOptions);
            scope.Connect();

            string installCmd = @"cmd.exe /c " + setupPath + "\\setup.exe /s /v\"DTSETUPINI=\\\"" + remoteInstallFiles + "\\" + "DTSetup.ini\\\" /qn /l*v+ " + setupPath + "\\Repinst.log";

            Dictionary<string, string> installCmdParams = new Dictionary<string, string>();
            installCmdParams["CommandLine"] = installCmd;
            installCmdParams["CurrentDirectory"] = setupPath;

            Dictionary<string, object> returnValues = new Dictionary<string, object>();
            ManagementPath wmiObjectPath = new ManagementPath(Class);
            ObjectGetOptions ogo = new ObjectGetOptions();
            ManagementBaseObject returnValue;
            using (ManagementClass mc = new ManagementClass(scope, wmiObjectPath, ogo))
            {
                ManagementBaseObject inparams = mc.GetMethodParameters(MethodName);

                if (installCmdParams != null)
                {
                    foreach (var p in installCmdParams)
                    {
                        inparams[p.Key] = p.Value;
                    }
                }

                returnValue = mc.InvokeMethod(MethodName, inparams, null);

            }

            int processId = 0;
            if (returnValues != null)
            {
                processId = Convert.ToInt32(returnValue.Properties["ProcessId"].Value);
            }

            return processId;

        }

        private static bool CreateCopyDirectory(string selection)
        {
            var success = true;
            Exception lastException = null;

            remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
            string serverPath = Path.Combine(server, remoteInstallFiles);
            serverPath = @"\\" + serverPath + @"\" + selection;
            clades.task().progress(_payload, "Copy files to remote server to " + serverPath, 50);
            var endTime = DateTime.Now.AddMinutes(1);
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {
                while (DateTime.Now < endTime)
                {
                    try
                    {
                        if (Directory.Exists(serverPath))
                        {
                            clades.task().progress(_payload, "Copy files to remote server to " + serverPath, 50);
                            break;
                        }
                        else
                        {
                            Directory.CreateDirectory(serverPath);
                            if (Directory.Exists(serverPath))
                            {
                                clades.task().progress(_payload, String.Format("Successfully created directories {0} to copy install file to on {1} \n", serverPath, server), 50);

                                break;
                            }
                        }
                    }
                    catch (IOException e)
                    {

                        clades.task().progress(_payload, String.Format("Failed to create directory on {0}. Retrying... {1}", server, e.ToString()), 50);
                        lastException = e;
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                }
                if (endTime <= DateTime.Now)
                {
                    clades.task().progress(_payload, String.Format("Timed out while waiting for directory to be created on {0}.", server), 50);

                    if (lastException != null)
                    {
                        clades.task().progress(_payload, String.Format("Problem creating directory on {0}. Error: {1} \n ", server, lastException), 50);
                        success = false;
                    }
                }

                return success;
            }
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
                string serverPath = Path.Combine(server, remoteInstallFiles);
                serverPath = @"\\" + serverPath + @"\" + selection;
                string setupFileOnServer = serverPath + @"\setup.exe";

                if (File.Exists(setupFileOnServer))
                    File.SetAttributes(setupFileOnServer, FileAttributes.Normal);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                File.Copy(localFilePath, setupFileOnServer, true);

                Console.Write("Setup file copied successfully {0}  \n", setupFileOnServer);
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
                    Console.Error.Write("Couldn't locate required configuration file(s) {0} \n", localConfigFilePath);
                    return false;
                }
                remoteInstallFiles = remoteInstallFiles.Replace(':', '$');
                string serverPath = Path.Combine(server, remoteInstallFiles);
                string configFileOnServer = @"\\" + serverPath + @"\DTSetup.ini";

                if (File.Exists(configFileOnServer))
                    File.SetAttributes(configFileOnServer, FileAttributes.Normal);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                File.Copy(localConfigFilePath, configFileOnServer, true);

                Console.Write("Configuration file copied successfully {0}  \n", configFileOnServer);

                return true;
            }
        }

        private static void CheckFileVersion(string remoteServer, string systemArchitecture)
        {
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {

                string RemoteFilePath = @"\\" + remoteServer + @"\C$\Program Files\Vision Solutions\Double-Take\" + systemArchitecture + @"\setup.exe";

                FileVersionInfo remoteFileVersion;
                if (File.Exists(RemoteFilePath))
                {
                    remoteFileVersion = FileVersionInfo.GetVersionInfo(RemoteFilePath);
                    Console.Write("Double-Take version on remote server; {0} \n", remoteFileVersion);
                }
                else
                {
                    Console.Write("It's a fresh install; no Double-Take version found on remote server \n");
                    return;
                }

                string localConfigFilePath = @"C:\Program Files\Vision Solutions\Double-Take\" + systemArchitecture;
                string LocalPath = Path.Combine(localConfigFilePath, @"setup.exe");

                FileVersionInfo localFileVersion;
                if (File.Exists(LocalPath))
                {
                    localFileVersion = FileVersionInfo.GetVersionInfo(LocalPath);
                    Console.Write("Double-Take version being installed; {0} \n", localFileVersion);
                }
                else
                {
                    Console.Error.Write("Couldn't locate required install file(s) {0} or {1} \n", LocalPath);
                    throw new FileNotFoundException("Install files not found ");
                }

                int versionCompare = CompareVersions(localFileVersion.ProductVersion, remoteFileVersion.ProductVersion);
                if (versionCompare <= 0)
                {
                    throw new NotSupportedException("Install failed; Product version being PushInstalled is same or less than the version installed on the remote server");
                }
            }
        }

        /// <remarks>
        /// DT Versions have 5 parts with an optional ".s" on the end.
        /// This method compares two DT versions and returns 0 for equal, -1 for sa '&lt;' sb, and 1 for sa '&gt;' sb
        /// </remarks>
        static int CompareVersions(string sa, string sb)
        {
            Func<string, int?> parse = s => { int ret; return int.TryParse(s, out ret) ? (int?)ret : null; };

            Func<string, IEnumerable<int>> f = s => s.Split('.').Select(t => int.Parse(t));

            var diff = f(sa).Zip(f(sb), (a, b) => new { a, b }).FirstOrDefault(x => x.a != x.b);

            return diff == null ? 0 : diff.a < diff.b ? -1 : 1;
        }

        private static string GetRemoteSystemArchitecture()
        {
            string keyString = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            string OS;
            using (Impersonation.LogonUser(domain, username, password, LogonType.Batch))
            {
                RegistryKey rk = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, server);
                RegistryKey key = rk.OpenSubKey(keyString);
                if (key != null)
                {
                    string architecture = (string)key.GetValue("PROCESSOR_ARCHITECTURE");
                    if (architecture.Contains("64"))
                        OS = "X64";
                    else
                        OS = "i386";
                }
                else
                {
                    throw new InvalidOperationException("Cannot determine the operating system architecture");
                }
            }
            return OS;

        }
        private static NetworkCredential GetCredentials(dynamic payload, int type)
        {
            NetworkCredential credentials = null;
            switch (type)
            {
                case 0:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.username, Password = payload.payload.dt.password, Domain = payload.payload.dt.domain };
                    break;
                case 1:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.source.username, Password = payload.payload.dt.source.password, Domain = payload.payload.dt.source.domain };
                    break;
                case 2:
                    credentials = new NetworkCredential() { UserName = payload.payload.dt.target.username, Password = payload.payload.dt.target.password, Domain = payload.payload.dt.target.domain };
                    break;
            }
            return credentials;
        }
        private static String BuildUrl(dynamic request, String method)
        {
            int portNumber = 6325;
            string bindingScheme = "http://";
            String url = null;

            if (request.payload.dt.hostname != null)
            {
                url = new UriBuilder(bindingScheme, find_working_ip(request,0) ,portNumber, method).ToString();
            }
            else
            {
                if (request.payload.dt.source != null)
                {
                    url = new UriBuilder(bindingScheme, find_working_ip(request, 1), portNumber, method).ToString();

                }
                if (request.payload.dt.target != null)
                {
                    url = new UriBuilder(bindingScheme, find_working_ip(request, 2), portNumber, method).ToString();
                }
            }
            return url;
        }
    }
}
