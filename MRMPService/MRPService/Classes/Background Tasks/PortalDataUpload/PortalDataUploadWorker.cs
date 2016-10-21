using System;
using System.Diagnostics;
using System.Threading;
using MRMPService.MRMPService.Log;

namespace MRMPService.MRMPService.Classes.Background_Classes
{
    class PortalDataUploadWorker
    {

        public void Start()
        {
            while (true)
            {                                                                                   
                DateTime _next_upload_run = DateTime.UtcNow.AddMinutes(Global.portal_upload_interval);
                try
                {
                    Logger.log("Staring data upload process", Logger.Severity.Info);
                    Stopwatch sw = Stopwatch.StartNew();

                    PerformanceUpload _performance = new PerformanceUpload();
                    Thread performance_thread = new Thread(new ThreadStart(_performance.Start));
                    performance_thread.Name = "Performance Upload Thread";
                    performance_thread.Start();

                    NetflowUpload _netflow = new NetflowUpload();
                    Thread netflow_thread = new Thread(new ThreadStart(_netflow.Start));
                    netflow_thread.Name = "Netflow Upload Thread";
                    netflow_thread.Start();

                    performance_thread.Join();
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
                while (_next_upload_run > DateTime.UtcNow)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
            }
        }
    }
}
