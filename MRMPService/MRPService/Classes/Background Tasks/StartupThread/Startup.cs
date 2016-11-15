using MRMPService.MRMPAPI.Classes;
using MRMPService.DTPollerCollection;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.PerformanceCollection;
using MRMPService.PlatformInventory;
using MRMPService.TaskExecutioner;
using MRMPService.Utilities;
using System;
using System.Data.SqlServerCe;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using MRMPService.WCF;
using MRMPService.DTEventPollerCollection;
using MRMPService.NetstatCollection;
using MRMPService.NetflowCollection;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    public class Startup
    {
        Thread scheduler_thread, mirror_thread, _performance_thread, _netflow_thread, _osinventody_thread, _osnetstat_thread, _dt_job_thread, _dt_event_thread;

        public void Start()
        {
            //String _connection_string = MRPDatabase.GetConnection().ConnectionString;
            //Logger.log(String.Format("Data Connection String: {0}",_connection_string), Logger.Severity.Info);

            //SqlCeEngine _engine = new SqlCeEngine(_connection_string);
            //_engine.LocalConnectionString = _connection_string;
            //try
            //{
            //    Logger.log(String.Format("Verifying database"), Logger.Severity.Info);
            //    if (false == _engine.Verify(VerifyOption.Enhanced))
            //    {
            //        int corrupt = 3;
            //        while (corrupt > 0)
            //        {
            //            Logger.log(String.Format("Database seems to contain corruption, trying to repair"), Logger.Severity.Error);
            //            try
            //            {
            //                _engine.Repair(null, RepairOption.RecoverAllPossibleRows);
            //                Logger.log(String.Format("Database repair success, trying verify"), Logger.Severity.Error);
            //                _engine.Verify();
            //                Logger.log(String.Format("Database verify success"), Logger.Severity.Error);
            //                break;
            //            }
            //            catch (Exception ex)
            //            {
            //                Logger.log(String.Format("Database repair failed, trying again: {1}", ex.Message), Logger.Severity.Error);
            //            }
            //            corrupt--;
            //        }
            //        if (corrupt == 0)
            //        {
            //            Logger.log(String.Format("Could not repair database, exiting!"), Logger.Severity.Error);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        try
            //        {
            //            Logger.log(String.Format("Compacting database"), Logger.Severity.Info);
            //            _engine.Compact(_connection_string);
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.log(String.Format("Error compacting database: {0}", ex.Message), Logger.Severity.Error);
            //            return;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.log(String.Format("Error verifying database: {0}", ex.Message), Logger.Severity.Error);
            //}
            
            //int workerThreads;
            //int portThreads;
            //ThreadPool.GetMaxThreads(out workerThreads, out portThreads);

            //Logger.log(String.Format("Maximum allowed threads on this workload: [worker threads: {0}] [port threads: {1}]", workerThreads, portThreads), Logger.Severity.Info);

            //int count = 0;
            //using (MRPDatabase _mrmp_database = new MRPDatabase())
            //{
            //    while (true)
            //    {
            //        Test _test = new Test() { id = Objects.RamdomGuid() };

            //        try
            //        {
            //            _mrmp_database.Test.Add(_test);
            //            _mrmp_database.SaveChanges();
            //            count++;
            //            Console.WriteLine(count);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }

            //}
            //using (MRPDatabase db = new MRPDatabase())
            //{
            //    if (Global.debug)
            //    {
            //        Logger.log(String.Format("Performance Counters: {0}, Network Flows: {1}, Netstat Flows: {2}, Events: {3}",
            //            db.Performance.ToList().Count,
            //            db.NetworkFlows.ToList().Count,
            //            db.Netstat.ToList().Count,
            //            db.Events.ToList().Count
            //            ), Logger.Severity.Debug);
            //    }
            //}

            // Start WCF Service
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

            ServiceHost serviceHost = new ServiceHost(typeof(MRPWCFService));
            ServiceMetadataBehavior wcfsmb = new ServiceMetadataBehavior();
            wcfsmb.HttpGetEnabled = true;
            wcfsmb.HttpGetUrl = wcfbaseAddress;
            serviceHost.Description.Behaviors.Add(wcfsmb);
            serviceHost.AddServiceEndpoint(typeof(IMRPWCFService), basicHttpBinding, wcfbaseAddress);
            serviceHost.Open();

            Settings.SetupController();
            Settings.ConfirmController();

            Logger.log(String.Format("organization id: {0}", Global.organization_id), Logger.Severity.Debug);

            //Fill counters that needs to be collected from workloads
            Global._available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% Idle Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% User Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "Processor", counter = "% Processor Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "Processor", counter = "Interrupts/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Available Bytes" });
            Global._available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Faults/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Reads/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Memory", counter = "Page Writes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Free Space" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Disk Time" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Write Disk Time" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "% Read Disk Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Reads/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Writes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Read Bytes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Write Bytes/sec" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Disk Bytes/sec" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Split IO/sec" });
            //Global._available_counters.Add(new CollectionCounter() { category = "LogicalDisk", counter = "Current Disk Queue Length" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Disk Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Write Disk Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "% Read Disk Time" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Reads/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Writes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Read Bytes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Write Bytes/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Disk Bytes/sec" });
            //Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Split IO/sec" });
            //Global._available_counters.Add(new CollectionCounter() { category = "PhysicalDisk", counter = "Current Disk Queue Length" });
            Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Received/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Bytes Sent/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Current Bandwidth" });
            //Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Output Queue Length" });
            Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Recieved/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Network Interface", counter = "Packets Sent/sec" });
            Global._available_counters.Add(new CollectionCounter() { category = "Double-Take Connection", counter = "*" });
            Global._available_counters.Add(new CollectionCounter() { category = "Double-Take Kernel", counter = "*" });
            Global._available_counters.Add(new CollectionCounter() { category = "Double-Take Source", counter = "*" });
            Global._available_counters.Add(new CollectionCounter() { category = "Double-Take Target", counter = "*" });

            TaskWorker _scheduler = new TaskWorker();
            if (Global.debug) { Logger.log("Starting Scheduler Thread", Logger.Severity.Debug); };
            scheduler_thread = new Thread(new ThreadStart(_scheduler.Start));
            scheduler_thread.Priority = ThreadPriority.AboveNormal;
            scheduler_thread.Start();

            PlatformInventoryThread _mirror = new PlatformInventoryThread();
            if (Global.debug) { Logger.log("Starting Mirror Thread", Logger.Severity.Debug); };
            mirror_thread = new Thread(new ThreadStart(_mirror.Start));
            mirror_thread.Priority = ThreadPriority.AboveNormal;
            mirror_thread.Start();

            WorkloadPerformanceThread _performance = new WorkloadPerformanceThread();
            if (Global.debug) { Logger.log("Starting Performance Collection Thread", Logger.Severity.Debug); };
            _performance_thread = new Thread(new ThreadStart(_performance.Start));
            _performance_thread.Priority = ThreadPriority.AboveNormal;
            _performance_thread.Start();

            NetflowWorker _netflow = new NetflowWorker();
            if (Global.debug) { Logger.log("Starting Netflow Collection Thread", Logger.Severity.Debug); };
            _netflow_thread = new Thread(new ThreadStart(_netflow.Start));
            _netflow_thread.Priority = ThreadPriority.AboveNormal;
            _netflow_thread.Start();

            if (Global.debug) { Logger.log("Starting Data Upload Worker", Logger.Severity.Debug); };
            PortalDataUploadWorker _upload = new PortalDataUploadWorker();
            Thread _upload_thread = new Thread(new ThreadStart(_upload.Start));
            _upload_thread.Priority = ThreadPriority.AboveNormal;
            _upload_thread.Start();

            WorkloadInventoryThread _osinventory = new WorkloadInventoryThread();
            if (Global.debug) { Logger.log("Starting OS Inventory Thread", Logger.Severity.Debug); };
            _osinventody_thread = new Thread(new ThreadStart(_osinventory.Start));
            _osinventody_thread.Priority = ThreadPriority.AboveNormal;
            _osinventody_thread.Start();

            WorkloadNetstatThread _osnetstat = new WorkloadNetstatThread();
            if (Global.debug) { Logger.log("Starting OS Netstat Thread", Logger.Severity.Debug); };
            _osnetstat_thread = new Thread(new ThreadStart(_osnetstat.Start));
            _osnetstat_thread.Priority = ThreadPriority.AboveNormal;
            _osnetstat_thread.Start();

            DTJobPollerThread _dt_job_polling = new DTJobPollerThread();
            if (Global.debug) { Logger.log("Starting DT Job Polling Thread", Logger.Severity.Debug); };
            _dt_job_thread = new Thread(new ThreadStart(_dt_job_polling.Start));
            _dt_job_thread.Priority = ThreadPriority.AboveNormal;
            _dt_job_thread.Start();

            DTEventPollerThread _dt_event_polling = new DTEventPollerThread();
            if (Global.debug) { Logger.log("Starting DT Event Polling Thread", Logger.Severity.Debug); };
            _dt_event_thread = new Thread(new ThreadStart(_dt_event_polling.Start));
            _dt_event_thread.Priority = ThreadPriority.AboveNormal;
            _dt_event_thread.Start();
        }
    }
}
