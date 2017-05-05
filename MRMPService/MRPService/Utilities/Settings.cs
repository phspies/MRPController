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
            MRMPServiceBase._mrmp_api.manager().confirm_controller();
        }

        static public void SetupController()
        {
            MRMPServiceBase.debug = Convert.ToBoolean((int)MRPRegistry.RegAccess("debug", 1, RegistryValueKind.DWord));
            if (MRMPServiceBase.debug)
            {
                Logger.log("Debug Enabled!", Logger.Severity.Info);
            }
            else
            {
                Logger.log("Debug Disabled!", Logger.Severity.Info);
            }
            MRPRegistry.RegAccess("manager_version", MRMPServiceBase.manager_version, RegistryValueKind.String);


            //check if agent Id exists
            String _agentId = MRPRegistry.RegAccess("manager_id") as String;
            if (String.IsNullOrEmpty(_agentId))
            {
                MRMPServiceBase.manager_id = Guid.NewGuid().ToString().Replace("-", "");
                MRPRegistry.RegAccess("manager_id", MRMPServiceBase.manager_id, RegistryValueKind.String);
            }
            else
            {
                MRMPServiceBase.manager_id = _agentId.ToString();
            }

            if (MRMPServiceBase.debug) { Logger.log("MRP Manager ID:" + MRMPServiceBase.manager_id, Logger.Severity.Info); };

        }
    }
}
