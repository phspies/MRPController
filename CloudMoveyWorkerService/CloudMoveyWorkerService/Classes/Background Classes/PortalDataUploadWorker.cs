using CloudMoveyWorkerService.Database;
using CloudMoveyWorkerService.Portal;
using CloudMoveyWorkerService.Portal.Types.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Utils;

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes
{
    class PortalDataUploadWorker
    {
        public void Start()
        {
            CloudMoveyPortal _cloud_movey = new CloudMoveyPortal();
            while (true)
            {
                try
                {
                    Global.event_log.WriteEntry("Staring data upload process");

                    Stopwatch sw = Stopwatch.StartNew();
                    int _new_networkflows, _new_performancecounters;
                    _new_networkflows = _new_performancecounters = 0;
                    foreach (NetworkFlow _flow in LocalData.search<NetworkFlow>())
                    {
                        MoveyNetworkFlowCRUDType _flowcrud = new MoveyNetworkFlowCRUDType();
                        Objects.MapObjects(_flow, _flowcrud);
                        _cloud_movey.netflow().createnetworkflow(_flowcrud);

                        //remove from local database
                        LocalData.delete<NetworkFlow>(_flow.id);

                        _new_networkflows += 1;
                    }
                    

                    //process performancecounters
                    foreach (Performance _performance in LocalData.search<Performance>())
                    {
                        MoveyPerformanceCRUDType _performancecrud = new MoveyPerformanceCRUDType();
                        Objects.MapObjects(_performance, _performancecrud);
                        _cloud_movey.performance().createnetworkflow(_performancecrud);

                        //remove from local database
                        LocalData.delete<Performance>(_performance.id);

                        _new_performancecounters += 1;
                    }


                    sw.Stop();

                    Global.event_log.WriteEntry(
                        String.Format("Completed data upload process.{2}{0} netflows.{2}{1} performancecounters.{2}{2} Total Execute Time: {3}",
                        _new_networkflows, _new_performancecounters,
                        Environment.NewLine, TimeSpan.FromMilliseconds(sw.Elapsed.TotalMilliseconds)
                        ));
                }
                catch (Exception ex)
                {
                    Global.event_log.WriteEntry(String.Format("Error in data upload task: {0}", ex.ToString()), EventLogEntryType.Error);
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }
    }
}
