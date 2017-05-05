using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using System.Threading;

namespace MRMPService.Scheduler.PortalDataUpload
{
    class ManagerEventUpload
    {
        public void Start()
        {
            try
            {
                Logger.log("Starting Manager Event Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();

                while (true)
                {
                    List<ManagerEvent> _increment_records;
                    using (MRPDatabase _ctx = new MRPDatabase())
                    {
                        _increment_records = _ctx.ManagerEvents.Take(500).ToList();
                    }
                    if (_increment_records.Count() > 0)
                    {
                        List<MRPManagerEventType> _event_crud_list = new List<MRPManagerEventType>();
                        foreach (ManagerEvent _db_flow in _increment_records)
                        {
                            MRPManagerEventType _mrp_crud = new MRPManagerEventType();
                            _mrp_crud.message = _db_flow.message;
                            _mrp_crud.timestamp = _db_flow.timestamp;
                            _mrp_crud.manager_id = MRMPServiceBase.manager_id;

                            _event_crud_list.Add(_mrp_crud);

                            if (_event_crud_list.Count > MRMPServiceBase.portal_upload_managerevent_page_size)
                            {
                                MRMPServiceBase._mrmp_api.manager().update_managerevents(_event_crud_list);
                                _event_crud_list.Clear();
                            }
                        }
                        //upload last remaining records
                        if (_event_crud_list.Count > 0)
                        {
                            MRMPServiceBase._mrmp_api.manager().update_managerevents(_event_crud_list);
                            _event_crud_list.Clear();
                        }
                        //remove all processed records from from local database
                        Stopwatch _sw_delete = Stopwatch.StartNew();
                        using (MRPDatabase _ctx = new MRPDatabase())
                        {
                            var _primary_keys = _increment_records.Select(x => x.Id).ToList();
                            var _db_records = _ctx.ManagerEvents.Where(r => _primary_keys.Contains(r.Id));
                            _ctx.ManagerEvents.RemoveRange(_db_records);
                            _ctx.SaveChanges();
                        }
                        _sw_delete.Stop();
                        Logger.log(String.Format("Took {0} to delete {1} manager events records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalSeconds), _increment_records.Count()), Logger.Severity.Info);
                    }
                    else
                    {
                        break;
                    }
                }


                _sw.Stop();
                Logger.log(String.Format("Completed Manager Events Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalSeconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading Manager Events information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
