using MRMPService.MRMPService.Log;
using MRMPService.LocalDatabase;
using System;
using MRMPService.Utilities;
using MRMPService.MRMPAPI.Types.API;
using DoubleTake.Web.Models;
using MRMPService.MRMPAPI;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MRMPService.DTEventPollerCollection
{
    class DTEventPollerDo
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
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _api.workload().DoubleTakeUpdateStatus(_workload, "Success", true);
                }
            }
            catch (Exception ex)
            {
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _api.workload().DoubleTakeUpdateStatus(_workload, ex.Message, false);
                }

                Logger.log(String.Format("Double-Take Event: Error contacting Double-Take Management Service for {0} using {1} : {2}", _workload, workload_ip, ex.ToString()), Logger.Severity.Info);
                throw new ArgumentException(String.Format("Double-Take Event: Error contacting Double-Take management service for {0} using {1}", _workload, workload_ip));
            }
            //now collect job information from target workload
            IEnumerable<EventLogModel> _dt_events;
            List<MRPInternalEventType> _internal_events = (new MRPInternalEvents()).internal_events;
            try
            {
                using (DoubleTake.Doubletake _dt = new DoubleTake.Doubletake(null, _workload))
                {
                    _dt_events = _dt.events().GetDoubleTakeEntries(_workload.last_dt_event_id).Result;
                }

                using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
                {
                    int? last_event = null;
                    foreach (EventLogModel _event in _dt_events)
                    {
                        MRPInternalEventType _internal_event;

                        string event_hex = _event.InstanceId.ToString("X");
                        int _event_id = Int16.Parse(event_hex.Substring(event_hex.Length - 4, 4).ToString());
                        if (_internal_events.Exists(x => x.id == _event_id))
                        {
                            _internal_event = _internal_events.FirstOrDefault(x => x.id == _event_id);
                            _mrmp.@event().create(new MRMPEventType()
                            {
                                event_id = _event.Id,
                                source_subsystem = MRMPEventTypes.DT,
                                message = _event.Message,
                                object_id = _workload.id,
                                timestamp = _event.TimeWritten.UtcDateTime,
                                response = _internal_event.response,
                                severity = _internal_event.severity
                            });
                            last_event = _event.Id;
                        }
                        else
                        {
                            Logger.log(String.Format("Double-Take Event: Can't find event in internal event list: {0}", JsonConvert.SerializeObject(_event)), Logger.Severity.Error);
                            continue;
                        }
                    }
                    //update workload with last event id
                    if (last_event != null)
                    {
                        _mrmp.workload().updateworkload(new MRPWorkloadType()
                        {
                            id = _workload.id,
                            last_dt_event_id = last_event
                        });
                    }
                }
            }
            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Event: Error collecting event information from {0} : {1}", _workload.hostname, ex.ToString()), Logger.Severity.Info);
                using (MRMP_ApiClient _api = new MRMP_ApiClient())
                {
                    _api.workload().DoubleTakeUpdateStatus(_workload, ex.Message, false);
                }
                return;
            }

            Logger.log(String.Format("Double-Take Event: Completed Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

