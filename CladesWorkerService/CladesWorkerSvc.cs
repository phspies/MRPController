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
using DoubleTakeManagementServiceRestProxy;
using DoubleTakeJobManagerRestProxy;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel.Web;
using Microsoft.Win32;
using DoubleTakeRestProxy;

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
            }
            else
            {
                Global.Debug = Convert.ToBoolean(rkSubKey.GetValue("Debug"));
                if (Global.Debug)
                {
                    CladesWorkerLog1.WriteEntry("Debug Enabled!");
                }
                else
                {
                    CladesWorkerLog1.WriteEntry("Debug Disabled!");
                }
            }

            Uri ManagementServicebaseAddress = new Uri("http://0.0.0.0:8080/CladesWorker/ManagementService");
            Uri JobManagerbaseAddress = new Uri("http://0.0.0.0:8080/CladesWorker/JobManager");

            ServiceHost hostManagementService = new ServiceHost(typeof(DoubleTakeManagementService), ManagementServicebaseAddress);
            ServiceHost hostJobManager = new ServiceHost(typeof(DoubleTakeJobManager), JobManagerbaseAddress);
            WebHttpBinding https_binding = new WebHttpBinding();
            //https_binding.Security.Mode = WebHttpSecurityMode.Transport;

            ServiceEndpoint managementEndpoint = hostManagementService.AddServiceEndpoint(typeof(IDoubleTakeManagementService), https_binding, "");
            ServiceEndpoint jobEndpoint = hostJobManager.AddServiceEndpoint(typeof(IDoubleTakeJobManager), https_binding, "");
            managementEndpoint.Behaviors.Add(new WebHttpBehavior());
            jobEndpoint.Behaviors.Add(new WebHttpBehavior());
            
            hostManagementService.Open();
            hostJobManager.Open();
            
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(config);
            foreach (ServiceEndpoint endpoint in hostManagementService.Description.Endpoints)
            {
                CladesWorkerLog1.WriteEntry("Listening on: " + endpoint.ListenUri.AbsoluteUri);
            }
            foreach (ServiceEndpoint endpoint in hostJobManager.Description.Endpoints)
            {
                CladesWorkerLog1.WriteEntry("Listening on: " + endpoint.ListenUri.AbsoluteUri);
            }

            CladesWorkerLog1.WriteEntry("Accepting Connections");


            
        }

        protected override void OnStop()
        {
            CladesWorkerLog1.WriteEntry("In onStop.");
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
