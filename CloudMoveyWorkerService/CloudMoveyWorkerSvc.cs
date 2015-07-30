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
using CloudMoveyWorkerService.CloudMovey;
using CloudMoveyWorkerService.CloudMovey.Types;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CloudMoveyWorkerService.CloudMovey.Controllers;
using System.Threading;
using CloudMoveyWorkerService.WCF;

namespace CloudMoveyWorkerService
{
  
    public partial class CloudMoveyWorkerSvc : ServiceBase
    {
        Thread t;
        ServiceHost host;

        public CloudMoveyWorkerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Start WCF Service
            if (Global.Debug) { Global.eventLog.WriteEntry("Starting WCF Service"); };
            Uri baseAddress = new Uri("http://localhost:8733/CloudMoveyWorkerService.WCF/CloudMovey/");
            ServiceHost serviceHost = new ServiceHost(typeof(CloudMoveyService), baseAddress);
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            serviceHost.Description.Behaviors.Add(smb);
            serviceHost.Open();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Global.eventLog = CloudMoveyWorkerLog1;
            Settings.SetupAgent();
            Settings.RegisterAgent();

            Scheduler _scheduler = new Scheduler();
            if (Global.Debug) { Global.eventLog.WriteEntry("Starting Scheduler"); };
            t = new Thread(new ThreadStart(_scheduler.Start));

            t.Start();
            Thread.Yield();

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        static void HandleException(Exception e)
        {
            Global.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
        }
        protected override void OnStop()
        {
            //CloudMoveyWorkerLog1.WriteEntry("In onStop.");
            if (host != null)
                host.Close();
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
