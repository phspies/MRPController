using CloudMoveyWorkerService.CloudMoveyWorkerService.Log;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace CloudMoveyWorkerService.Portal
{
    class Settings 
    {
        static public string DBLocation()
        {
            String _registry = @"SOFTWARE\CloudMovey Worker Service";
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            String _dblocation = rkSubKey.GetValue("DBLocation", null) as String;
            return _dblocation;
        }
        static public void RegisterAgent()
        {

            CloudMoveyPortal CloudMovey = new CloudMoveyPortal();
            if (!CloudMovey.worker().confirm_worker())
            {
                Logger.log("Worker not registered, registering worker with CloudMovey portal", Logger.Severity.Warn);
                if (CloudMovey.worker().register_worker())
                {
                    if (CloudMovey.worker().confirm_worker())
                    {
                        Logger.log("Worker Registered", Logger.Severity.Warn);
                    }
                    else
                    {
                        Logger.log("Registration Failed", Logger.Severity.Error);
                    }
                }
            }
        }
        static public void SetupAgent()
        {
            //Define global version number
            String _registry = @"SOFTWARE\CloudMovey Worker Service";
            Global.version_number = "0.0.1";
            Logger.log("Starting CloudMovey Worker Service", Logger.Severity.Info);
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            if (rkSubKey == null)
            {
                Logger.log("Creating Registry Hive", Logger.Severity.Info);
                RegistryKey key = Registry.LocalMachine.CreateSubKey(_registry);
                key.SetValue("Debug", false);
                Global.debug = false;
                key.Close();
                rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            }
            else
            {
                rkSubKey.SetValue("agentVersion", Global.version_number, RegistryValueKind.String);
                Global.debug = Convert.ToBoolean(rkSubKey.GetValue("debug"));
                if (Global.debug)
                {
                    Logger.log("Debug Enabled!", Logger.Severity.Info);
                }
                else
                {
                    Logger.log("Debug Disabled!", Logger.Severity.Info);
                }
            }
            //check if agent Id exists
            String _agentId = rkSubKey.GetValue("agentId", false) as String;
            if (String.IsNullOrEmpty(_agentId))
            {
                Global.agent_id = Guid.NewGuid().ToString().Replace("-", "");
                rkSubKey.SetValue("agentId", Global.agent_id, RegistryValueKind.String);
            }
            else
            {
                Global.agent_id = _agentId.ToString();
            }
            if (Global.debug) {Logger.log("CloudMovey Worker Agent ID:" + Global.agent_id, Logger.Severity.Info);};

            //load portal API base url
            String _apiBase = rkSubKey.GetValue("apiBase", null) as String;
            if (String.IsNullOrEmpty(_apiBase))
            {
                rkSubKey.SetValue("apiBase", "<missing url base>", RegistryValueKind.String);
                Logger.log("Missing base URL", Logger.Severity.Error);
            }
            else
            {
                Global.api_base = _apiBase.ToString();
                if (Uri.IsWellFormedUriString(Global.api_base, UriKind.Absolute))
                {
                    if (Global.debug) { Logger.log("CloudMovey Portal URL:" + Global.api_base, Logger.Severity.Info); };
                }
                else
                {
                    Logger.log("incorrect base url format", Logger.Severity.Error);
                }

            }
        }
    }
}
