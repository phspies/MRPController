using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using MRMPService.MRMPService.Classes.Background_Classes;
using MRMPService.MRMPService.Log;

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
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                //AppDomain.CurrentDomain.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                //MRMPService.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);


                MRMPServiceBase.event_log = MRPLog1;

                Startup _startup = new Startup();
                Logger.log("Starting Manager Service", Logger.Severity.Debug);
                startup_thread = new Thread(new ThreadStart(_startup.Start));
                startup_thread.IsBackground = true;
                startup_thread.Start();

                Thread.Yield();
                //Thread.Sleep(20000);
            }
            catch (Exception ex)
            {
                //something went wrong while starting up
                Logger.log(String.Format("Failed to start the service {0}", ex.ToString()), Logger.Severity.Error);
                System.Environment.Exit(1);
            }
        }
        protected override void OnStop()
        {
            Logger.log(String.Format("Shutdown initiated, waiting from threads to stops"), Logger.Severity.Info);
            while (startup_thread.IsAlive)
            {
                Thread.Sleep(1000);
            }
            Logger.log(String.Format("Shutdown complete!"), Logger.Severity.Info);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        static void HandleException(Exception e)
        {
            Logger.log(String.Format("Fatal Exeception: {0}", e.ToString()), Logger.Severity.Fatal);
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
