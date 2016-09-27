using MRMPService.MRMPService.Log;
using System;
using MRMPService.MRMPAPI.Types.API;
using MRMPService.MRMPAPI;
using MRMPService.RP4VMTypes;
using MRMPService.RP4VMAPI;

namespace MRMPService.RPEventPollerCollection
{
    class RPEventPollerDo
    {
        public static void PollerDo(MRPManagementobjectType _mrp_managementobject)
        {
            //refresh managementobject from portal
            using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
            {
                _mrp_managementobject = _mrmp.managementobject().getmanagementobject_id(_mrp_managementobject.id);
            }

            //check for credentials
            MRPPlatformType _target_platform = _mrp_managementobject.target_platform;
            MRPCredentialType _platform_credential = _target_platform.credential;
            if (_platform_credential == null)
            {
                throw new ArgumentException(String.Format("RP4VM Group: Error finding credentials for appliance {0}", _target_platform.rp4vm_url));
            }

            //check for working IP

            Logger.log(String.Format("RP4VM Group: Start RP4vM Event collection for {0}", _target_platform.rp4vm_url), Logger.Severity.Info);

            //first try to connect to the target server to make sure we can connect
            eventsPage _group_events = new eventsPage();

            //first try to connect to the target server to make sure we can connect
            try
            {
                String username = String.Concat((String.IsNullOrEmpty(_platform_credential.domain) ? "" : (_platform_credential.domain + @"\")), _platform_credential.username);

                using (RP4VM_ApiClient _rp4vm = new RP4VM_ApiClient(_target_platform.rp4vm_url, username, _platform_credential.encrypted_password))
                {
                    ManagementSettings _info = _rp4vm.settings().getManagementSettings_Method();

                    //now we try to get the snapshot information we have. IF we can't, then we know the group has been deleted...
                    //_group_stats = _rp4vm.events().get(Int64.Parse(_mrp_managementobject.moid));
                    _group_events = _rp4vm.events().getEventLogsByFilter_Method(
                        new UserEventLogsFilter() {
                            topics = new System.Collections.Generic.List<eventLogTopic>() {
                                eventLogTopic.CONSISTENCY_GROUP
                            }
                            
                        });

                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("RP4VM Group: Error collecting information from {0} : {1}", _target_platform.rp4vm_url, ex.ToString()), Logger.Severity.Info);
            }
            //now collect job information from target workload
            //IEnumerable<EventLogModel> _dt_events;
            //List<MRPInternalEventType> _internal_events = (new MRPInternalEvents()).internal_events;
            //try
            //{
            //    _group_snapshot_list.
            //    using (MRMP_ApiClient _mrmp = new MRMP_ApiClient())
            //    {
            //        foreach (EventLogModel _event in _dt_events)
            //        {
            //            string event_hex = _event.InstanceId.ToString("X");
            //            int _event_id = Convert.ToInt32(event_hex.Substring(event_hex.Length - 4, 4).ToString(), 16);
            //            MRPInternalEventType _internal_event = _internal_events.FirstOrDefault(x => x.id == _event_id);
            //            string _friendlyname = null;
            //            if (_internal_event != null)
            //            {
            //                _friendlyname = String.Join(" ", _event.Source, new System.Globalization.CultureInfo("en-US", false).TextInfo.ToTitleCase(String.Join(" ", _internal_event.name.Split('_').Skip(1)).ToString()));
            //            }

            //            _mrmp.@event().create(new MRMPEventType()
            //            {
            //                event_id = _event.Id,
            //                source_subsystem = String.Join(" ",_event.Source, _event.Category),
            //                source_datamover = "Double-Take",
            //                message = _event.Message,
            //                object_id = _workload.id,
            //                timestamp = _event.TimeWritten.UtcDateTime,
            //                response = (_internal_event == null) ? null : _internal_event.response,
            //                severity = _event.EntryType.ToString(),
            //                eventname = (_internal_event == null) ? null : _internal_event.name,
            //                eventnamefriendly = (_friendlyname == null) ? null : _friendlyname
            //            });
            //        }
            //    }
            //}
            ////When we get an exception from collecting the job informationwe assume the job no longer exists and needs to be marked as being deleted on the portal
            //catch (Exception ex)
            //{
            //    Logger.log(String.Format("Double-Take Event: Error collecting event information from {0} : {1}", _workload.hostname, ex.ToString()), Logger.Severity.Info);

            //    return;
            //}

            //Logger.log(String.Format("Double-Take Event: Completed Double-Take collection for {0} using {1}", _workload.hostname, workload_ip), Logger.Severity.Info);
        }
    }
}

