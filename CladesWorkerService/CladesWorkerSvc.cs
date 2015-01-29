using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel.Web;
using Microsoft.Win32;
using CladesWorkerService.Clades;
using CladesWorkerService.Clades.API;
using CladesWorkerService.Clades.Types;
using System.Net;

namespace CladesWorkerService
{
  
    public partial class CladesWorkerSvc : ServiceBase
    {
        public CladesWorkerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CladesWorkerLog1.WriteEntry("Starting Clades Worker Service");
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clades Worker Service", true);
            if (rkSubKey == null)
            {
                CladesWorkerLog1.WriteEntry("Creating Registry Hive");
                RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Clades Worker Service");
                key.SetValue("Debug", false);
                Global.Debug = false;
                key.Close();
                rkSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clades Worker Service", true);
            }
            else
            {
                Global.Debug = Convert.ToBoolean(rkSubKey.GetValue("debug"));
                if (Global.Debug)
                {
                    CladesWorkerLog1.WriteEntry("Debug Enabled!");
                }
                else
                {
                    CladesWorkerLog1.WriteEntry("Debug Disabled!");
                }
            }

            //check if agent Id exists
            String _agentId = rkSubKey.GetValue("agentId", false) as String;
            if (String.IsNullOrEmpty(_agentId)) 
            {
                Global.agentId = Guid.NewGuid().ToString().Replace("-","");
                rkSubKey.SetValue("agentId", Global.agentId, RegistryValueKind.String);
            } else {
                Global.agentId = _agentId.ToString();
            }
            CladesWorkerLog1.WriteEntry("Clades Worker Agent ID:" + Global.agentId);

            //load portal API base url
            String _apiBase = rkSubKey.GetValue("apiBase", null) as String;
            if (String.IsNullOrEmpty(_apiBase)) 
            {
                rkSubKey.SetValue("apiBase", "<missing url base>", RegistryValueKind.String);
                CladesWorkerLog1.WriteEntry("Missing base URL", EventLogEntryType.Error);
                this.Stop();
            } else {
                Global.apiBase = _apiBase.ToString();
                if (Uri.IsWellFormedUriString(Global.apiBase, UriKind.Absolute)) 
                {
                    CladesWorkerLog1.WriteEntry("Clades Portal URL:" + Global.apiBase);
                }
                else
                {
                    CladesWorkerLog1.WriteEntry("incorrect base url format",EventLogEntryType.Error);
                    this.Stop();
                }
                
            }

            CladesWorkerService.Clades.Clades clades = new CladesWorkerService.Clades.Clades(Global.apiBase,null,null);
            WorkerObject worker = clades.worker();
            if (worker.get_worker()) 
            {
                CladesWorkerLog1.WriteEntry("Hostname: " + worker.worker.hostname);    
            } else {
                CladesWorkerLog1.WriteEntry("Worker not registered, registering worker with clades portal");
                Worker newworker = new Worker();
                newworker.hostname = Environment.MachineName;
                newworker.ipaddress = Dns.GetHostAddresses(Environment.MachineName)[0].ToString();
                newworker.id = Global.agentId;
                if (worker.register_worker(newworker))
                {
                    if (worker.get_worker())
                    {
                        CladesWorkerLog1.WriteEntry("Worker Registered: " + worker.worker.hostname);
                    }
                    else
                    {
                        CladesWorkerLog1.WriteEntry("Registration Failed: " + worker.status.error);
                    }

                }
                
            }
            

            //Uri ManagementServicebaseAddress = new Uri("http://0.0.0.0:8080/CladesWorker/ManagementService");
            //Uri JobManagerbaseAddress = new Uri("http://0.0.0.0:8080/CladesWorker/JobManager");

            //ServiceHost hostManagementService = new ServiceHost(typeof(DoubleTakeManagementService), ManagementServicebaseAddress);
            //ServiceHost hostJobManager = new ServiceHost(typeof(DoubleTakeJobManager), JobManagerbaseAddress);
            //WebHttpBinding https_binding = new WebHttpBinding();
            ////https_binding.Security.Mode = WebHttpSecurityMode.Transport;

            //ServiceEndpoint managementEndpoint = hostManagementService.AddServiceEndpoint(typeof(IDoubleTakeManagementService), https_binding, "");
            //ServiceEndpoint jobEndpoint = hostJobManager.AddServiceEndpoint(typeof(IDoubleTakeJobManager), https_binding, "");
            //managementEndpoint.Behaviors.Add(new WebHttpBehavior());
            //jobEndpoint.Behaviors.Add(new WebHttpBehavior());
            
            //hostManagementService.Open();
            //hostJobManager.Open();
            
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var serviceModel = ServiceModelSectionGroup.GetSectionGroup(config);
            //foreach (ServiceEndpoint endpoint in hostManagementService.Description.Endpoints)
            //{
            //    CladesWorkerLog1.WriteEntry("Listening on: " + endpoint.ListenUri.AbsoluteUri);
            //}
            //foreach (ServiceEndpoint endpoint in hostJobManager.Description.Endpoints)
            //{
            //    CladesWorkerLog1.WriteEntry("Listening on: " + endpoint.ListenUri.AbsoluteUri);
            //}

            //CladesWorkerLog1.WriteEntry("Accepting Connections");


            
        }

        protected override void OnStop()
        {
            //CladesWorkerLog1.WriteEntry("In onStop.");
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
