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
			RegistryKey _remote_base, _computerName = null;
			_remote_base = _computerName = null;

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

                _remote_base = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _remote_address);
                _computerName = _remote_base.OpenSubKey(RegLocComputerName);
                if (_computerName == null)
                {
                    throw new Exception("Registry location '" + RegLocComputerName + "' is not readable.");
                }
                if (((String)_computerName.GetValue("ComputerName")) != _new_hostname)
                {
                    throw new Exception("The computer name was set by WMI but was not updated in the registry location: '" + RegLocComputerName + "'");
                }
            }
            catch (Exception)
            {
                return false;
            }
			finally
			{
				_remote_base?.Close();
				_computerName?.Close();
				_remote_base?.Dispose();
				_computerName?.Dispose();
			}
            return true;
        }
    }
}
