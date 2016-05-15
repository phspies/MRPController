using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using MRMPService.Utilities;
using MRMPService.API.Types.API;
using DoubleTake.Web.Models;
using MRMPService.API;
using System.Collections.Generic;

namespace MRMPService.DTEventPollerCollection
{
    class DTEventPoller
    {
        public static void PollerDo(MRPWorkloadType _workload)
        {
            //check for credentials

            MRPCredentialType _credential = _workload.credential;
            if (_credential == null)
            {
                throw new ArgumentException(String.Format("Double-Take Event: Error finding credentials for workload {0} {1}", _workload.id, _workload.hostname));
            }

            //check for working IP
            string workload_ip = null;
            using (Connection _connection = new Connection())
            {
                workload_ip = _connection.FindConnection(_workload.iplist, true);
            }
            if (workload_ip == null)
            {
                throw new ArgumentException(String.Format("Double-Take Event: Error finding contactable IP for workload {0}", _workload.hostname));
            }
            Logger.log(String.Format("Double-Take Event: Start Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _workload))
                {
                    ProductInfoModel _dt_info = _dt.management().GetProductInfo().Result;
                }
            }
            catch (Exception ex)
            {
                //Mark the job as being unavailable because the system can't be reached
                using (MRP_ApiClient _mrmp = new MRP_ApiClient())
                {
                    _mrmp.job().updatejob(new MRPJobType()
                    {
                        id = _workload.id,
                        internal_state = "unavailable",
                    });
                }

                Logger.log(String.Format("Double-Take Event: Error contacting Double-Take Management Service for {0} using {1} : {2}", _workload, workload_ip, ex.ToString()), Logger.Severity.Info);
                throw new ArgumentException(String.Format("Double-Take Event: Error contacting Double-Take management service for {0} using {1}", _workload, workload_ip));
            }
            //now collect job information from target workload
            IEnumerable<EventLogModel> _dt_events;
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _workload))
                {
                    _dt_events = _dt.events().GetDoubleTakeEntries(_workload.last_dt_event_id).Result;
                }
                using (MRP_ApiClient _mrmp = new MRP_ApiClient())
                {
                    foreach (EventLogModel _event in _dt_events)
                    {
                       
                    }
                }
            }
            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Event: Error collecting event information from {1} : {2}", _workload.hostname, ex.ToString()), Logger.Severity.Info);
                using (MRP_ApiClient _mrmp = new MRP_ApiClient())
                {
                    _mrmp.job().updatejob(new MRPJobType()
                    {
                        id = _workload.id,
                        internal_state = "deleted",
                    });
                }
                return;
            }

            Logger.log(String.Format("Double-Take Event: Completed Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

