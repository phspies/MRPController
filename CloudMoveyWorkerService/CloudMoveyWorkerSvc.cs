using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Controllers;
using System.Threading;
using CloudMoveyWorkerService.WCF;
using System.Data.Services;
using CloudMoveyWorkerService.Portal.Classes;
using CloudMoveyWorkerService.Portal.Classes.Static_Classes.Background_Classes;
using CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes;

namespace CloudMoveyWorkerService
{
  
    public partial class CloudMoveyWorkerSvc : ServiceBase
    {
        Thread scheduler_thread, mirror_thread, _performance_thread, _netflow_thread;
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
            if (Global.debug) { Global.event_log.WriteEntry("Starting WCF Service"); };

            Uri wcfbaseAddress = new Uri("http://localhost:8734/CloudMoveyWCFService");
            serviceHost = new ServiceHost(typeof(CloudMoveyService), wcfbaseAddress);
            ServiceMetadataBehavior wcfsmb = new ServiceMetadataBehavior();
            wcfsmb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(wcfsmb);
            serviceHost.Open();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Global.event_log = CloudMoveyWorkerLog1;
            Settings.SetupAgent();
            Settings.RegisterAgent();
            Global.event_log.WriteEntry(String.Format("organization id: {0}", Global.organization_id));

            TaskWorker _scheduler = new TaskWorker();
            if (Global.debug) { Global.event_log.WriteEntry("Starting Scheduler Thread"); };
            scheduler_thread = new Thread(new ThreadStart(_scheduler.Start));
            scheduler_thread.Start();


            PlatformInventoryWorker _mirror = new PlatformInventoryWorker();
            if (Global.debug) { Global.event_log.WriteEntry("Starting Mirror Thread"); };
            mirror_thread = new Thread(new ThreadStart(_mirror.Start));
            mirror_thread.Start();

            PerformanceWorker _performance = new PerformanceWorker();
            if (Global.debug) { Global.event_log.WriteEntry("Starting Performance Collection Thread"); };
            _performance_thread = new Thread(new ThreadStart(_performance.Start));
            _performance_thread.Start();

            NetflowWorker _netflow = new NetflowWorker();
            if (Global.debug) { Global.event_log.WriteEntry("Starting Netflow v5 Collection Thread"); };
            _netflow_thread = new Thread(new ThreadStart(_netflow.Start));
            _netflow_thread.Start();

            Thread.Yield();
            //Thread.Sleep(20000);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        static void HandleException(Exception e)
        {
            Global.event_log.WriteEntry(e.ToString(), EventLogEntryType.Error);
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
