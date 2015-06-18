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
using CladesWorkerService.Clades;
using CladesWorkerService.Clades.Types;
using System.Net;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using CladesWorkerService.Clades.Controllers;
using System.Threading;

namespace CladesWorkerService
{
  
    public partial class CladesWorkerSvc : ServiceBase
    {
        Thread t;
        public CladesWorkerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            Global.eventLog = CladesWorkerLog1;
            Settings.SetupAgent();
            Settings.RegisterAgent();

            Scheduler _scheduler = new Scheduler();
            if (Global.Debug) { Global.eventLog.WriteEntry("Starting Scheduler"); };
            t = new Thread(new ThreadStart(_scheduler.Start));

            // Start ThreadProc.  Note that on a uniprocessor, the new  
            // thread does not get any processor time until the main thread  
            // is preempted or yields.  Uncomment the Thread.Sleep that  
            // follows t.Start() to see the difference.
            t.Start();
  
            
        }

        protected override void OnStop()
        {
            //CladesWorkerLog1.WriteEntry("In onStop.");
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
 
    }
}
