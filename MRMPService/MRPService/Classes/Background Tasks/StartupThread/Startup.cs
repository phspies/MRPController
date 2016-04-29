using MRMPService.API.Classes;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.PerformanceCollection;
using MRMPService.PlatformInventory;
using MRMPService.TaskExecutioner;
using MRMPService.Utilities;
using MRMPService.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class Startup
    {
        Thread scheduler_thread, mirror_thread, _performance_thread, _netflow_thread, _dataupload_thread, _osinventody_thread, _osnetstat_thread;
        ServiceHost serviceHost;

        public void Start()
        {
            MRPDatabase db = new MRPDatabase();

            // Start WCF Service
            if (Global.debug)
            {
                Logger.log(String.Format("Platforms: {0}, Workloads: {1}, Credentials: {2}, Performance Counters: {3}, Network Flows: {4}, Netstat Flows: {5}",
                    db.Platforms.ToList().Count,
                    db.Workloads.ToList().Count,
                    db.Credentials.ToList().Count,
                    db.Performance.ToList().Count,
                    db.NetworkFlows.ToList().Count,
                    db.Netstat.ToList().Count
                    ), Logger.Severity.Debug);
            };

            Logger.log(String.Format("Starting WCF Service"), Logger.Severity.Debug);

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.MaxBufferPoolSize = int.MaxValue;
            basicHttpBinding.MaxBufferSize = int.MaxValue;
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            basicHttpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            basicHttpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            basicHttpBinding.ReaderQuotas.MaxDepth = int.MaxValue;
            basicHttpBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            Uri wcfbaseAddress = new Uri("http://localhost:8734/MRMPWCFService");

            var serviceHost = new ServiceHost(typeof(MRPWCFService));
            ServiceMetadataBehavior wcfsmb = new ServiceMetadataBehavior();
            wcfsmb.HttpGetEnabled = true;
            wcfsmb.HttpGetUrl = wcfbaseAddress;
            serviceHost.Description.Behaviors.Add(wcfsmb);
            serviceHost.AddServiceEndpoint(typeof(IMRPWCFService), basicHttpBinding, wcfbaseAddress);
            serviceHost.Open();


            Settings.SetupController();
            Settings.ConfirmController();

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

            WorkloadInventoryThread _osinventody = new WorkloadInventoryThread();
            if (Global.debug) { Logger.log("Starting OS Inventory Thread", Logger.Severity.Debug); };
            _osinventody_thread = new Thread(new ThreadStart(_osinventody.Start));
            _osinventody_thread.Start();

            WorkloadNetstatThread _osnetstat = new WorkloadNetstatThread();
            if (Global.debug) { Logger.log("Starting OS Netstat Thread", Logger.Severity.Debug); };
            _osnetstat_thread = new Thread(new ThreadStart(_osnetstat.Start));
            _osnetstat_thread.Start();
        }
    }
}
