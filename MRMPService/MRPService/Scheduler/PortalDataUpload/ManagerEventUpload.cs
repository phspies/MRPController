using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;

namespace MRMPService.Scheduler.PortalDataUpload
{
    class ManagerEventUpload
    {
        public async void Start()
        {
            try
            {
                Logger.log("Starting Manager Event Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();

                while (true)
                {
                    IEnumerable<ManagerEvent> _increment_records;
                    using (MRPDatabase _db = new MRPDatabase())
                    {
                        _increment_records = _db.ManagerEvents.Take(500).AsEnumerable();
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
                                    await MRMPServiceBase._mrmp_api.manager().update_managerevents(_event_crud_list);
                                    _event_crud_list.Clear();
                                }
                            }
                            //upload last remaining records
                            if (_event_crud_list.Count > 0)
                            {
                                await MRMPServiceBase._mrmp_api.manager().update_managerevents(_event_crud_list);
                                _event_crud_list.Clear();
                            }

                            //remove all processed records from from local database
                            Stopwatch _sw_delete = Stopwatch.StartNew();
                            _db.ManagerEvents.RemoveRange(_increment_records);
                            _db.SaveChanges();
                            _sw_delete.Stop();
                            Logger.log(String.Format("Took {0} to delete {1} manager events records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _increment_records.Count()), Logger.Severity.Info);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                _sw.Stop();
                Logger.log(String.Format("Completed Manager Events Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading Manager Events information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
