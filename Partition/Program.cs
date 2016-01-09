using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Partition
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionOptions options = ProcessConnectionOptions();

           // options.Username = "Administrator";
           // options.Password = "c0mp2q";

            ManagementScope connectionScope = ConnectionScope("127.0.0.1", options);

            //process network interfaces
            SelectQuery wmiNetInterfaces = new SelectQuery("select * from Win32_NetworkAdapterConfiguration where IPEnabled = 'True'");
            ManagementObjectSearcher searchNetInterfacesConfig = new ManagementObjectSearcher(connectionScope, wmiNetInterfaces);
            foreach (ManagementObject searchNetInterfaceConfig in searchNetInterfacesConfig.Get())
            {
                foreach (ManagementObject searchNetInterface in searchNetInterfaceConfig.GetRelated("Win32_NetworkAdapter"))
                {
                    foreach (PropertyData prop in searchNetInterface.Properties)
                    {
                        Console.WriteLine("{0} = {1}", prop.Name, prop.Value);
                    }
                }
            }
        }
        private static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }

        private static ManagementScope ConnectionScope(string machineName, ConnectionOptions options)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2");
            connectScope.Options = options;

            try
            {
                connectScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message.ToString());
            }
            return connectScope;
        }

    }
}
