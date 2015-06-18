using CladesWorkerService.Clades.Types;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CladesWorkerService.Clades
{
    class Settings
    {
        static public void RegisterAgent()
        {
            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase, null, null);
            Worker worker = new Worker(clades);
            worker.hostname = Environment.MachineName;
            worker.worker_version = Global.verionNumber;
            worker.ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
            worker.id = Global.agentId;
            Global.eventLog.WriteEntry(JsonConvert.SerializeObject(worker));
            if (worker.confirm_worker())
            {
                Global.eventLog.WriteEntry("Hostname: " + worker.hostname);
            }
            else
            {
                Global.eventLog.WriteEntry("Worker not registered, registering worker with clades portal");
                if (worker.register_worker())
                {
                    if (worker.confirm_worker())
                    {
                        Global.eventLog.WriteEntry("Worker Registered: " + worker.hostname);
                    }
                    else
                    {
                        Global.eventLog.WriteEntry("Registration Failed");
                    }
                }
            }
        }
        static public void SetupAgent()
        {
            //Define global version number
            String _registry = @"SOFTWARE\Clades Worker Service";
            Global.verionNumber = "0.0.1";
            Global.eventLog.WriteEntry("Starting Clades Worker Service");
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            if (rkSubKey == null)
            {
                Global.eventLog.WriteEntry("Creating Registry Hive");
                RegistryKey key = Registry.LocalMachine.CreateSubKey(_registry);
                key.SetValue("Debug", false);
                Global.Debug = false;
                key.Close();
                rkSubKey = Registry.LocalMachine.OpenSubKey(_registry, true);
            }
            else
            {
                Global.Debug = Convert.ToBoolean(rkSubKey.GetValue("debug"));
                if (Global.Debug)
                {
                    Global.eventLog.WriteEntry("Debug Enabled!");
                }
                else
                {
                    Global.eventLog.WriteEntry("Debug Disabled!");
                }
            }
            //check if agent Id exists
            String _agentId = rkSubKey.GetValue("agentId", false) as String;
            if (String.IsNullOrEmpty(_agentId))
            {
                Global.agentId = Guid.NewGuid().ToString().Replace("-", "");
                rkSubKey.SetValue("agentId", Global.agentId, RegistryValueKind.String);
            }
            else
            {
                Global.agentId = _agentId.ToString();
            }
            if (Global.Debug) {Global.eventLog.WriteEntry("Clades Worker Agent ID:" + Global.agentId);};

            //load portal API base url
            String _apiBase = rkSubKey.GetValue("apiBase", null) as String;
            if (String.IsNullOrEmpty(_apiBase))
            {
                rkSubKey.SetValue("apiBase", "<missing url base>", RegistryValueKind.String);
                Global.eventLog.WriteEntry("Missing base URL", EventLogEntryType.Error);
            }
            else
            {
                Global.apiBase = _apiBase.ToString();
                if (Uri.IsWellFormedUriString(Global.apiBase, UriKind.Absolute))
                {
                    if (Global.Debug) { Global.eventLog.WriteEntry("Clades Portal URL:" + Global.apiBase); };
                }
                else
                {
                    Global.eventLog.WriteEntry("incorrect base url format", EventLogEntryType.Error);
                }

            }
        }
    }
}
