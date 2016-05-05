using MRMPService.API.Classes;
using MRMPService.DTPollerCollection;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.PerformanceCollection;
using MRMPService.PlatformInventory;
using MRMPService.TaskExecutioner;
using MRMPService.Utilities;
using MRMPService.WCF;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class Startup
    {
        Thread scheduler_thread, mirror_thread, _performance_thread, _netflow_thread, _dataupload_thread, _osinventody_thread, _osnetstat_thread, _dt_thread;

        public void Start()
        {
            String _connection_string = MRPDatabase.GetConnection().ConnectionString;
            SqlCeEngine _engine = new SqlCeEngine(_connection_string);
            try
            {
                Logger.log(String.Format("Verifying database"), Logger.Severity.Info);
                if (false == _engine.Verify())
                {
                    int corrupt = 3;
                    while (corrupt > 0)
                    {
                        Logger.log(String.Format("Database seems to contain corruption, trying to repair"), Logger.Severity.Error);
                        try
                        {
                            _engine.Repair(null, RepairOption.RecoverAllPossibleRows);
                            Logger.log(String.Format("Database repair success, trying verify"), Logger.Severity.Error);
                            _engine.Verify();
                            Logger.log(String.Format("Database verify success"), Logger.Severity.Error);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Logger.log(String.Format("Database repair failed, trying again: {1}", ex.Message), Logger.Severity.Error);
                        }
                        corrupt--;
                    }
                    if (corrupt == 0)
                    {
                        Logger.log(String.Format("Could not repair database, exiting!"), Logger.Severity.Error);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        Logger.log(String.Format("Compacting database"), Logger.Severity.Info);
                        _engine.Compact(_connection_string);
                    }
                    catch (Exception ex)
                    {
                        Logger.log(String.Format("Error compacting database: {0}", ex.Message), Logger.Severity.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error verifying database: {0}", ex.Message), Logger.Severity.Error);
            }

            using (MRPDatabase db = new MRPDatabase())
            {
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
                }
            }

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

            //WorkloadNetstatThread _osnetstat = new WorkloadNetstatThread();
            //if (Global.debug) { Logger.log("Starting OS Netstat Thread", Logger.Severity.Debug); };
            //_osnetstat_thread = new Thread(new ThreadStart(_osnetstat.Start));
            //_osnetstat_thread.Start();

            DTPollerThread _dt_polling = new DTPollerThread();
            if (Global.debug) { Logger.log("Starting DT Polling Thread", Logger.Severity.Debug); };
            _dt_thread = new Thread(new ThreadStart(_dt_polling.Start));
            _dt_thread.Start();

        }

    }
}
