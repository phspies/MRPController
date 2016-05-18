using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using MRMPService.MRMPAPI.Classes;
using MRMPService.MRMPService.Classes.Background_Classes;
using System.Linq;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.TaskExecutioner;
using MRMPService.Utilities;
using MRMPService.PerformanceCollection;
using MRMPService.PlatformInventory;

namespace MRMPService
{

    public partial class MRPSvc : ServiceBase
    {
        Thread startup_thread;
        public MRPSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Global.event_log = MRPLog1;

                Startup _startup = new Startup();
                Logger.log("Starting Manager Service", Logger.Severity.Debug);
                startup_thread = new Thread(new ThreadStart(_startup.Start));
                startup_thread.Start();

                Thread.Yield();
                //Thread.Sleep(20000);
            }
            catch (Exception ex)
            {
                //something went wrong while starting up
                Logger.log(String.Format("Failed to start the MRP Controller Service {0}", ex.ToString()), Logger.Severity.Error);
                System.Environment.Exit(1);
            }
        }
        protected override void OnStop()
        {


        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
