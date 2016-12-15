using System;
using System.Diagnostics;
using System.Threading;
using MRMPService.MRMPService.Log;

namespace MRMPService.Scheduler.PortalDataUpload
{
    class PortalDataUploadWorker
    {

        public void Start()
        {
            while (true)
            {                                                                                   
                DateTime _next_upload_run = DateTime.UtcNow.AddMinutes(MRMPServiceBase.portal_upload_interval);
                try
                {
                    Logger.log("Staring data upload process", Logger.Severity.Info);
                    Stopwatch sw = Stopwatch.StartNew();

                    WorkloadPerformanceUpload _performance = new WorkloadPerformanceUpload();
                    Thread _performance_thread = new Thread(new ThreadStart(_performance.Start));
                    _performance_thread.Name = "Performance Upload Thread";
                    _performance_thread.Start();
                    _performance_thread.Priority = ThreadPriority.AboveNormal;

                    NetflowUpload _netflow = new NetflowUpload();
                    Thread netflow_thread = new Thread(new ThreadStart(_netflow.Start));
                    netflow_thread.Name = "Netflow Upload Thread";
                    netflow_thread.Start();
                    netflow_thread.Priority = ThreadPriority.AboveNormal;

                    ManagerEventUpload _managerevents = new ManagerEventUpload();
                    Thread managerevents_thread = new Thread(new ThreadStart(_managerevents.Start));
                    managerevents_thread.Name = "Manager Event Upload Thread";
                    managerevents_thread.Start();
                    managerevents_thread.Priority = ThreadPriority.AboveNormal;

                    _performance_thread.Join();
                    netflow_thread.Join();
                    managerevents_thread.Join();

                    sw.Stop();

                    Logger.log(String.Format("Completed data upload process. Total Elapsed Time: {0}", TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)), Logger.Severity.Info);
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
