using System;
using System.Security;
using System.Threading;
using System.Management;

namespace MRMPService.Utilities
{


    /// <summary>Performs various WMI operations.</summary>
    public static class WmiOperations
    {
        /// <summary>Runs some command.</summary>
        /// <param name="command">The command to run.</param>
        /// <param name="commandline">The command arguments (default=none).</param>
        /// <param name="machine">The machine on which to run the command (default=current one).</param>
        /// <param name="domain">Domain for 'username' (default=current domain).</param>
        /// <param name="username">The user that runs the command (default=current user).</param>
        /// <param name="password">User's password (default=current user password)</param>
        /// <param name="securePassword">Crypted user's password (default=current user password)</param>
        /// <param name="wait">Command's timeout expiration.</param>
        /// <remarks>'wait==0' => Doesn't wait for process.</remarks>
        /// <remarks>'wait==+Inf' => Wait for process to end.</remarks>
        /// <remarks>'wait==]0;+Inf[' => Wait at most 'wait' seconds and then times-out.</remarks>
        /// <remarks>IMPORTANT: 'domain/login/password' have to be empty for local administration ('User credentials cannot be used for local connections')</remarks>
        /// <remarks>IMPORTANT: 'domain/login/password' must have correct priviledges on the CIMV2 path ('Access denied' => see http://msdn.microsoft.com/en-us/library/windows/desktop/aa393613%28v=vs.85%29.aspx )</remarks>
        /// TODO: ?? Replace 'wait' parameter with some cancel callback ??
        public static int Run(string command,
                                string machine = null,
                                string domain = null,
                                string username = null,
                                string password = null,
                                SecureString securePassword = null,
                                double wait = double.PositiveInfinity)
        {
            // We let internal functions check and make defaults
            if (double.IsNaN(wait) || (wait < 0.0)) { throw new ArgumentException("wait range is [0;+Inf]"); }

            // Process survey
            var processId = new[] { (uint)0 };
            var exitCode = 0;
            var mre = new ManualResetEvent(false);
            var w = (ManagementEventWatcher)null;
            var scope = (ManagementScope)null;
            var doWait = (wait > 0.0);

            try
            {
                // Connecting to WMI scope
                var span = TimeSpan.FromSeconds(0); // TODO: ?? relate to 'wait' or check cancel ??
                scope = connectToWmiScope(machine, domain, username, password, securePassword, span);

                // Begin process stop watcher
                Func<uint> getProcessId = () => processId[0];
                Action<int> setExitCode = (code) => exitCode = code;
                var q = new WqlEventQuery("Win32_ProcessStopTrace");
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += (sender, e) =>
                {
                    var hasStoppedId = (uint)e.NewEvent.Properties["ProcessId"].Value;
                    if (hasStoppedId != getProcessId()) { return; }

                    setExitCode((int)(uint)e.NewEvent.Properties["ExitStatus"].Value);
                    mre.Set();
                };
                w.Start();

                // Create the process
                processId[0] = createProcess(scope, command);

                // Wait process exit
                if (!doWait) { return 0; }
                waitRun(wait, mre);

                // Result
                return exitCode;
            }
            catch (Exception ex)
            {
                var msg = string.Format("Command execution failed");

                if (ex is TimeoutException)
                {
                    if ((scope != null) && (processId[0] != 0))
                    {
                        bool found;
                        tryKillProcess(scope, processId[0], out found);
                    }
                }

                throw new Exception(msg, ex);
            }
            finally
            {
                if (w != null)
                {
                    w.Stop();
                    w.Dispose();
                }
            }
        }

        private static void waitRun(double wait, ManualResetEvent mre)
        {
            var eventArrived = false;
            var doWaitForever = (double.IsPositiveInfinity(wait));

            if (doWaitForever)
            {
                eventArrived = mre.WaitOne();
            }
            else
            {
                var waitMs = (int)(1000.0 * wait);
                eventArrived = mre.WaitOne(waitMs, false);
            }

            if (!eventArrived) { throw new TimeoutException(); }
        }
       
        private static ManagementScope connectToWmiScope(string machine, string domain, string username, string password, SecureString securePassword, TimeSpan span)
        {
            var path = "ROOT\\CIMV2";

            if (domain != null) { username = domain + "\\" + username; }
            if (machine != null) { path = String.Format(@"\\{0}\{1}", machine, path); }

            var connectionOptions = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                EnablePrivileges = true,
                Username = username,
                Password = password,
                SecurePassword = securePassword,
                Timeout = span,
            };

            var managementScope = new ManagementScope(path, connectionOptions);
            managementScope.Connect();

            return managementScope;
        }
        private static uint createProcess(ManagementScope scope, string arguments)
        {
            var objectGetOptions = new ObjectGetOptions();
            var managementPath = new ManagementPath("Win32_Process");
            using (var processClass = new ManagementClass(scope, managementPath, objectGetOptions))
            {
                using (var inParams = processClass.GetMethodParameters("Create"))
                {
                    inParams["CommandLine"] = arguments;
                    using (var outParams = processClass.InvokeMethod("Create", inParams, null))
                    {
                        var err = (uint)outParams["returnValue"];
                        if (err != 0)
                        {
                            var info = "see http://msdn.microsoft.com/en-us/library/windows/desktop/aa389388(v=vs.85).aspx";
                            switch (err)
                            {
                                case 2: info = "Access Denied"; break;
                                case 3: info = "Insufficient Privilege"; break;
                                case 8: info = "Unknown failure"; break;
                                case 9: info = "Path Not Found"; break;
                                case 21: info = "Invalid Parameter"; break;
                            }

                            var msg = "Failed to create process, error = " + outParams["returnValue"] + " (" + info + ")";
                            throw new Exception(msg);
                        }

                        return (uint)outParams["processId"];
                    }
                }
            }
        }
        private static bool tryKillProcess(ManagementScope scope, uint processId, out bool found)
        {
            found = false;
            var stopped = true;

            var sq = new SelectQuery("Select * from Win32_Process Where ProcessId = " + processId);
            using (var searcher = new ManagementObjectSearcher(scope, sq))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var errcode = (uint)queryObj.InvokeMethod("Terminate", null);
                    queryObj.Dispose();

                    found = true;
                    stopped &= (errcode == 0);
                }
            }

            return (found && stopped);
        }
    }
}
