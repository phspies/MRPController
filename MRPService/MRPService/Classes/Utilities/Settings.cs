using MRPService.MRPService.Log;
using Microsoft.Win32;
using System;
using MRPService.API;
using System.Threading;

namespace MRPService.Utilities
{
    class Settings
    {
        static public void ConfirmController()
        {
            MRP_ApiClient MRP = new MRP_ApiClient();
            MRP.worker().confirm_controller();
        }

        static public void SetupController()
        {
            Global.debug = Convert.ToBoolean(MRPRegistry.RegAccess("debug"));
            if (Global.debug)
            {
                Logger.log("Debug Enabled!", Logger.Severity.Info);
            }
            else
            {
                Logger.log("Debug Disabled!", Logger.Severity.Info);
            }

            //check if agent Id exists
            String _agentId = MRPRegistry.RegAccess("agentId") as String;
            if (String.IsNullOrEmpty(_agentId))
            {
                Global.agent_id = Guid.NewGuid().ToString().Replace("-", "");
                MRPRegistry.RegAccess("agentId", Global.agent_id, RegistryValueKind.String);
            }
            else
            {
                Global.agent_id = _agentId.ToString();
            }

            if (Global.debug) { Logger.log("MRP Controller ID:" + Global.agent_id, Logger.Severity.Info); };

        }
    }
}
