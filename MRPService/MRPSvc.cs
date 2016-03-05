using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using MRPService.WCF;
using MRPService.API.Classes;
using MRPService.API.Classes.Static_Classes.Background_Classes;
using MRPService.MRPService.Classes.Background_Classes;
using System.Linq;
using MRPService.LocalDatabase;
using MRPService.MRPService.Log;
using MRPService.TaskExecutioner;
using MRPService.Utilities;

namespace MRPService
{

    public partial class MRPSvc : ServiceBase
    {
        Thread scheduler_thread, mirror_thread, _performance_thread, _netflow_thread, _dataupload_thread, _osinventody_thread;
        ServiceHost serviceHost;
        public MRPSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {


            Global.event_log = MRPLog1;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            MRPDatabase db = new MRPDatabase();


            Settings.SetupAgent();
            Settings.RegisterAgent();

            // Start WCF Service
            if (Global.debug) {
                Logger.log(String.Format("Platforms: {0}, Workloads: {1}, Credentials: {2}, Performance Counters: {3}, Network Flows: {4}",
                    db.Platforms.ToList().Count,
                    db.Workloads.ToList().Count,
                    db.Credentials.ToList().Count,
                    db.Performance.ToList().Count,
                    db.NetworkFlows.ToList().Count
                    ), Logger.Severity.Debug);
            };

            Logger.log(String.Format("Starting WCF Service"), Logger.Severity.Debug);
            Uri wcfbaseAddress = new Uri("http://localhost:8734/MRPWCFService");
            serviceHost = new ServiceHost(typeof(MRPWCFService), wcfbaseAddress);
            ServiceMetadataBehavior wcfsmb = new ServiceMetadataBehavior();
            wcfsmb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(wcfsmb);
            serviceHost.Open();


            Logger.log(String.Format("organization id: {0}", Global.organization_id), Logger.Severity.Debug);

            TaskWorker _scheduler = new TaskWorker();
            if (Global.debug) { Logger.log("Starting Scheduler Thread", Logger.Severity.Debug); };
            scheduler_thread = new Thread(new ThreadStart(_scheduler.Start));
            scheduler_thread.Start();


            PlatformInventoryThread _mirror = new PlatformInventoryThread();
            if (Global.debug) { Logger.log("Starting Mirror Thread", Logger.Severity.Debug); };
            mirror_thread = new Thread(new ThreadStart(_mirror.Start));
            mirror_thread.Start();

            WorkloadPerformanceThread _performance = new WorkloadPerformanceThread();
            if (Global.debug) { Logger.log("Starting Performance Collection Thread", Logger.Severity.Debug); };
            _performance_thread = new Thread(new ThreadStart(_performance.Start));
            _performance_thread.Start();

            NetflowV5Worker _netflow = new NetflowV5Worker();
            if (Global.debug) { Logger.log("Starting Netflow v5 Collection Thread", Logger.Severity.Debug); };
            _netflow_thread = new Thread(new ThreadStart(_netflow.Start));
            _netflow_thread.Start();

            PortalDataUploadWorker _dataupload = new PortalDataUploadWorker();
            if (Global.debug) { Logger.log("Starting Data Upload Thread", Logger.Severity.Debug); };
            _dataupload_thread = new Thread(new ThreadStart(_dataupload.Start));
            _dataupload_thread.Start();


            OSInventoryThread _osinventody = new OSInventoryThread();
            if (Global.debug) { Logger.log("Starting OS Inventory Thread", Logger.Severity.Debug); };
            _osinventody_thread = new Thread(new ThreadStart(_osinventody.Start));
            _osinventody_thread.Start();

            Thread.Yield();
            //Thread.Sleep(20000);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        static void HandleException(Exception e)
        {
            Logger.log(e.ToString(), Logger.Severity.Error);
        }
        protected override void OnStop()
        {
            //MRPLog1.WriteEntry("In onStop.");
            if (serviceHost != null)
                serviceHost.Close();
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
