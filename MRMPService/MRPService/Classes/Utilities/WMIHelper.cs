using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
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
            options.Authentication = AuthenticationLevel.Default;
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
            connectScope.Connect();
            return connectScope;
        }
    }
}
