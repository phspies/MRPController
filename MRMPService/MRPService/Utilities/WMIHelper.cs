using MRMPService.MRMPService.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.Utilities
{
    class WMIHelper
    {
        /// <summary>
        /// Construct ConnectionOptions
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public static ConnectionOptions ProcessConnectionOptions(string _username, string _password)
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Packet;
            options.EnablePrivileges = true;
            options.Username = _username;
            options.Password = _password;
            return options;
        }
        /// <summary>
        /// Construct ManagementScope
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="ConnectionOptions (optional)"></param>
        /// <returns></returns>
        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2");
            connectScope.Options = options;
            int _connect_retries = 3;
            while (true)
            {
                try
                {
                    connectScope.Connect();
                    break;
                }
                catch (Exception ex)
                {
                    if (--_connect_retries == 0)
                    {
                        Logger.log(String.Format("Error creating a WMI connection to workload {0}: {1}", machineName, ex.GetBaseException().Message), Logger.Severity.Error);
                        throw new ArgumentException(String.Format("Error creating a WMI connection to workload {0}: {1}", machineName, ex.GetBaseException().Message));
                    }
                    else Thread.Sleep(10000);
                }
            }
            return connectScope;
        }
    }
}
