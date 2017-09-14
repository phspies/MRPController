using MRMPService.MRMPService.Log;
using System;
using MRMPService.Utilities;
using MRMPService.Modules.MRMPPortal.Contracts;
using DoubleTake.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MRMPService.MRMPAPI;
using System.Text.RegularExpressions;

namespace MRMPService.Scheduler.DTEventPollerCollection
{
    class DTEventPollerDo
    {
        public static void PollerDo(MRMPWorkloadBaseType _workload)
        {
            //check for working IP
            string workload_ip = _workload.GetContactibleIP(true);
            Logger.log(String.Format("Double-Take Event: Start Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
            try
            {
                using (MRMPDoubleTake.Doubletake _dt = new MRMPDoubleTake.Doubletake(null, _workload))
                {
                    ProductInfoModel _dt_info = _dt.management().GetProductInfo();
                }
                MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(_workload, "Success", true);
            }
            catch (Exception ex)
            {
                MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(_workload, ex.Message, false);
                Logger.log(String.Format("Double-Take Event: Error contacting Double-Take Management Service for {0} using {1} : {2}", _workload.hostname, workload_ip, ex.ToString()), Logger.Severity.Info);
                throw new ArgumentException(String.Format("Double-Take Event: Error contacting Double-Take management service for {0} using {1}", _workload.hostname, workload_ip));
            }
            //now collect job information from target workload
            IEnumerable<EventLogModel> _dt_events;
            List<MRPInternalEventType> _internal_events = (new MRPInternalEvents()).internal_events;
            try
            {
                using (MRMPDoubleTake.Doubletake _dt = new MRMPDoubleTake.Doubletake(null, _workload))
                {
                    _dt_events = _dt.events().GetDoubleTakeEntries(_workload.last_dt_event_id).Result;
                }

                foreach (EventLogModel _event in _dt_events)
                {
                    string event_hex = _event.InstanceId.ToString("X");
                    int _event_id = Convert.ToInt32(event_hex.Substring(event_hex.Length - 4, 4).ToString(), 16);
                    MRPInternalEventType _internal_event = _internal_events.FirstOrDefault(x => x.id == _event_id);
                    string _friendlyname = null;
                    if (_internal_event != null)
                    {
                        _friendlyname = String.Join(" ", _event.Source, new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(String.Join(" ", _internal_event.name.Split('_').Skip(1)).ToString()));
                    }

                    MRMPServiceBase._mrmp_api.@event().create(new MRMPEventType()
                    {
                        elementevent_id = _event.Id,
                        source_subsystem = String.Join(" ", _event.Source, _event.Category),
                        source_datamover = "Double-Take",
                        message = _event.Message,
                        object_id = _workload.id,
                        timestamp = _event.TimeWritten.UtcDateTime,
                        response = (_internal_event == null) ? null : _internal_event.response,
                        severity = _event.EntryType.ToString(),
                        eventname = (_internal_event == null) ? Regex.Replace(_event.Message.ToLower().Replace(" ","_"), @"[^0-9a-zA-Z\.]", string.Empty) : _internal_event.name,
                        eventnamefriendly = (_friendlyname == null) ? null : _friendlyname
                    });
                }
            }

            //When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            catch (Exception ex)
            {
                Logger.log(String.Format("Double-Take Event: Error collecting event information from {0} : {1}", _workload.hostname, ex.ToString()), Logger.Severity.Info);

                MRMPServiceBase._mrmp_api.workload().DoubleTakeUpdateStatus(_workload, ex.Message, false);
                return;
            }

            Logger.log(String.Format("Double-Take Event: Completed Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);

        }
    }
}

