using MRPService.MRPService.Log;
using Microsoft.Win32;
using System;
using MRPService.API;
using System.Threading;

namespace MRPService.Utilities
{
    class Settings
    {
        static public void RegisterAgent()
        {
            ApiClient MRP = new ApiClient();
            if (!MRP.worker().confirm_worker())
            {
                while (true)
                {
                    Logger.log("Worker not registered, registering worker with MRP portal", Logger.Severity.Warn);
                    if (MRP.worker().register_worker())
                    {
                        if (MRP.worker().confirm_worker())
                        {
                            Logger.log("Worker Registered", Logger.Severity.Warn);
                            break;
                        }
                        else
                        {
                            Logger.log("Registration Failed", Logger.Severity.Error);
                        }
                    }
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
            }
        }

        static public void SetupAgent()
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
