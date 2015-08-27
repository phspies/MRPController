using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using CloudMoveyWorkerService.CloudMovey;
using CloudMoveyWorkerService.CloudMovey.Controllers;
using System.Threading;
using CloudMoveyWorkerService.WCF;
using System.Data.Services;

namespace CloudMoveyWorkerService
{
  
    public partial class CloudMoveyWorkerSvc : ServiceBase
    {
        Thread t;
        ServiceHost serviceHost;
        public CloudMoveyWorkerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Start DataService
            //if (Global.Debug) { Global.eventLog.WriteEntry("Starting Data Service"); };
            //Uri[] databaseAddress = new Uri[] { new Uri("http://localhost:8733/CloudMoveyDataService") };
            //DataServiceHost dataserviceHost = new DataServiceHost(typeof(CloudMoveyDataService), databaseAddress);
            //dataserviceHost.Open();

            // Start WCF Service
            if (Global.Debug) { Global.eventLog.WriteEntry("Starting WCF Service"); };

            Uri wcfbaseAddress = new Uri("http://localhost:8734/CloudMoveyWCFService");
            serviceHost = new ServiceHost(typeof(CloudMoveyService), wcfbaseAddress);
            ServiceMetadataBehavior wcfsmb = new ServiceMetadataBehavior();
            wcfsmb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(wcfsmb);
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
            if (serviceHost != null)
                serviceHost.Close();
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
