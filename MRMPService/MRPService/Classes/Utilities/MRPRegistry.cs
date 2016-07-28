using System;
using MRMPService.MRMPService.Log;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRMPService.Utilities
{
    class MRPRegistry
    {
        static public object RegAccess(String key, object defaultvalue = null, object regkind = null)
        {
            object return_value = new object();
            try
            {
                String _registry = @"SOFTWARE\MRMP Manager Service";
                RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
                if (rkSubKey == null)
                {
                    Logger.log("Creating Registry Hive", Logger.Severity.Info);
                    RegistryKey regkey = Registry.LocalMachine.CreateSubKey(_registry);
                    Global.debug = false;
                    regkey.Close();
                    rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
                }

                //if default value is set but it does not exist in registry
                if (rkSubKey.GetValue(key) == null)
                {
                    rkSubKey.SetValue(key, defaultvalue, (RegistryValueKind)regkind);
                }

                //get value from registry
                return_value = rkSubKey.GetValue(key);

            }
            catch (Exception ex)
            {

            }
            return return_value;
        }
    }
}
