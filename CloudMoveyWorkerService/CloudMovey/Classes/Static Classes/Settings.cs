using CloudMoveyWorkerService.CloudMovey.Types;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            MoveyWorkerType worker = new MoveyWorkerType();
            worker.hostname = Environment.MachineName;
            worker.worker_version = Global.versionNumber;
            worker.ipaddress = String.Join(",", System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Select(x => x.ToString()).Where(x => x.ToString().Contains(".")));
            worker.id = Global.agentId;
            CloudMovey CloudMovey = new CloudMovey();
            if (CloudMovey.worker().confirm_worker(worker))
            {
                Global.eventLog.WriteEntry("Hostname: " + worker.hostname);
            }
            else
            {
                Global.eventLog.WriteEntry("Worker not registered, registering worker with CloudMovey portal");
                if (CloudMovey.worker().register_worker(worker))
                {
                    if (CloudMovey.worker().confirm_worker(worker))
                    {
                        Global.eventLog.WriteEntry("Worker Registered: " + worker.hostname);
                    }
                    else
                    {
                        Global.eventLog.WriteEntry("Registration Failed", EventLogEntryType.Error);
                    }
                }
            }
        }
        static public void SetupAgent()
        {
            //Define global version number
            String _registry = @"SOFTWARE\CloudMovey Worker Service";
            Global.versionNumber = "0.0.1";
            Global.eventLog.WriteEntry("Starting CloudMovey Worker Service");
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
                rkSubKey.SetValue("agentVersion", Global.versionNumber, RegistryValueKind.String);
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
            if (Global.Debug) {Global.eventLog.WriteEntry("CloudMovey Worker Agent ID:" + Global.agentId);};

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
                    if (Global.Debug) { Global.eventLog.WriteEntry("CloudMovey Portal URL:" + Global.apiBase); };
                }
                else
                {
                    Global.eventLog.WriteEntry("incorrect base url format", EventLogEntryType.Error);
                }

            }
        }
    }
}
