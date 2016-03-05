using System;
using MRPService.MRPService.Log;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRPService.Utilities
{
    class MRPRegistry
    {
        static public object RegAccess(String key, object value = null, object regkind = null)
        {
            String _registry = @"SOFTWARE\MRP Controller Service";
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            if (rkSubKey == null)
            {
                Logger.log("Creating Registry Hive", Logger.Severity.Info);
                RegistryKey regkey = Registry.LocalMachine.CreateSubKey(_registry);
                Global.debug = false;
                regkey.Close();
                rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            }
            object return_value = new object();
            if (value == null)
            {
                return_value = rkSubKey.GetValue(key);
            }
            else
            {
                rkSubKey.SetValue(key, value, (RegistryValueKind)regkind);
            }
            return return_value;
        }
    }
}
