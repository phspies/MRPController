using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace CloudMoveyWorkerService.CloudMovey
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

            CloudMovey CloudMovey = new CloudMovey();
            if (!CloudMovey.worker().confirm_worker())
            {
                Global.event_log.WriteEntry("Worker not registered, registering worker with CloudMovey portal");
                if (CloudMovey.worker().register_worker())
                {
                    if (CloudMovey.worker().confirm_worker())
                    {
                        Global.event_log.WriteEntry("Worker Registered");
                    }
                    else
                    {
                        Global.event_log.WriteEntry("Registration Failed", EventLogEntryType.Error);
                    }
                }
            }
        }
        static public void SetupAgent()
        {
            //Define global version number
            String _registry = @"SOFTWARE\CloudMovey Worker Service";
            Global.version_number = "0.0.1";
            Global.event_log.WriteEntry("Starting CloudMovey Worker Service");
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            if (rkSubKey == null)
            {
                Global.event_log.WriteEntry("Creating Registry Hive");
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
                    Global.event_log.WriteEntry("Debug Enabled!");
                }
                else
                {
                    Global.event_log.WriteEntry("Debug Disabled!");
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
            if (Global.debug) {Global.event_log.WriteEntry("CloudMovey Worker Agent ID:" + Global.agent_id);};

            //load portal API base url
            String _apiBase = rkSubKey.GetValue("apiBase", null) as String;
            if (String.IsNullOrEmpty(_apiBase))
            {
                rkSubKey.SetValue("apiBase", "<missing url base>", RegistryValueKind.String);
                Global.event_log.WriteEntry("Missing base URL", EventLogEntryType.Error);
            }
            else
            {
                Global.api_base = _apiBase.ToString();
                if (Uri.IsWellFormedUriString(Global.api_base, UriKind.Absolute))
                {
                    if (Global.debug) { Global.event_log.WriteEntry("CloudMovey Portal URL:" + Global.api_base); };
                }
                else
                {
                    Global.event_log.WriteEntry("incorrect base url format", EventLogEntryType.Error);
                }

            }
        }
    }
}
