using MRMPService.API.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;
using System.Threading.Tasks;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class PortalDataUploadWorker
    {

        public void Start()
        {
            API.MRP_ApiClient _cloud_movey = new API.MRP_ApiClient();
            while (true)
            {
                DateTime _next_upload_run = DateTime.Now.AddMinutes(Global.portal_upload_interval);

                try
                {
                    Logger.log("Staring data upload process", Logger.Severity.Info);

                    Stopwatch sw = Stopwatch.StartNew();

                    PerformanceUpload _performance = new PerformanceUpload();
                    Logger.log("Starting Performance Upload Thread", Logger.Severity.Debug);
                    Thread performance_thread = new Thread(new ThreadStart(_performance.Start));
                    performance_thread.Start();

                    NetstatUpload _netstat = new NetstatUpload();
                    Logger.log("Starting Netstat Upload Thread", Logger.Severity.Debug);
                    Thread netstat_thread = new Thread(new ThreadStart(_netstat.Start));
                    netstat_thread.Start();

                    NetflowUpload _netflow = new NetflowUpload();
                    Logger.log("Starting NetFlow Upload Thread", Logger.Severity.Debug);
                    Thread netflow_thread = new Thread(new ThreadStart(_netflow.Start));
                    netflow_thread.Start();

                    performance_thread.Join();
                    netstat_thread.Join();
                    netflow_thread.Join();


                    sw.Stop();

                    Logger.log(
                        String.Format("Completed data upload process. Total Elapsed Time: {0}", TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ), Logger.Severity.Info);
                }
                catch (Exception ex)
                {
                    Logger.log(String.Format("Error in data upload task: {0}", ex.ToString()), Logger.Severity.Error);
                }
                //Wait for next run
                while (_next_upload_run > DateTime.Now)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}
