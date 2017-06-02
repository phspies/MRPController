using Microsoft.Win32;
using System;
using System.Management;

namespace MRMPService.MRPService.Utilities
{
    static class RenameWindowsWorkload
    {
        static Boolean SetComputerName(String _new_hostname, string _username, string _password, string _remote_address)
        {
            String RegLocComputerName = @"SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName";
            try
            {
                string compPath = "Win32_ComputerSystem.Name='" + Environment.MachineName + "'";
                using (ManagementObject mo = new ManagementObject(new ManagementPath(compPath)))
                {
                    ManagementBaseObject inputArgs = mo.GetMethodParameters("Rename");
                    inputArgs["Name"] = _new_hostname;
                    ManagementBaseObject output = mo.InvokeMethod("Rename", inputArgs, null);
                    uint retValue = (uint)Convert.ChangeType(output.Properties["ReturnValue"].Value, typeof(uint));
                    if (retValue != 0)
                    {
                        throw new Exception("Computer could not be changed due to unknown reason.");
                    }
                }

                RegistryKey _remote_base = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _remote_address);
                RegistryKey ComputerName = _remote_base.OpenSubKey(RegLocComputerName);
                if (ComputerName == null)
                {
                    throw new Exception("Registry location '" + RegLocComputerName + "' is not readable.");
                }
                if (((String)ComputerName.GetValue("ComputerName")) != _new_hostname)
                {
                    throw new Exception("The computer name was set by WMI but was not updated in the registry location: '" + RegLocComputerName + "'");
                }
                ComputerName.Close();
                ComputerName.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
