using MRMPService.MRMPService.Log;
using Microsoft.Win32;
using System;
using MRMPService.MRMPAPI;

namespace MRMPService.Utilities
{
    class Settings
    {
        static public void ConfirmController()
        {
            MRMP_ApiClient MRP = new MRMP_ApiClient();
            MRP.manager().confirm_controller();
        }

        static public void SetupController()
        {
            Global.debug = Convert.ToBoolean((int)MRPRegistry.RegAccess("debug", 1, RegistryValueKind.DWord));
            if (Global.debug)
            {
                Logger.log("Debug Enabled!", Logger.Severity.Info);
            }
            else
            {
                Logger.log("Debug Disabled!", Logger.Severity.Info);
            }
            MRPRegistry.RegAccess("manager_version", Global.manager_version, RegistryValueKind.String);


            //check if agent Id exists
            String _agentId = MRPRegistry.RegAccess("manager_id") as String;
            if (String.IsNullOrEmpty(_agentId))
            {
                Global.manager_id = Guid.NewGuid().ToString().Replace("-", "");
                MRPRegistry.RegAccess("manager_id", Global.manager_id, RegistryValueKind.String);
            }
            else
            {
                Global.manager_id = _agentId.ToString();
            }

            if (Global.debug) { Logger.log("MRP Manager ID:" + Global.manager_id, Logger.Severity.Info); };

        }
    }
}
