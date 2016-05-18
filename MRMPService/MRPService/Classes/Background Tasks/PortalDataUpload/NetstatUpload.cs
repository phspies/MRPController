using MRMPService.MRMPAPI.Types.API;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.Utilities;

namespace MRMPService.MRMPService.Classes.Background_Classes
{

    class NetstatUpload
    {
        public void Start()
        {
            try
            {
                Logger.log("Starting Netstat Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                using (MRMPAPI.MRMP_ApiClient _cloud_movey = new MRMPAPI.MRMP_ApiClient())
                {
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        List<MRPNetworkStatCRUDType> _netstat_list = new List<MRPNetworkStatCRUDType>();
                        IList<Netstat> _db_netstats = db.Netstat.AsEnumerable().ToList();

                        foreach (Netstat _db_netstat_record in _db_netstats)
                        {
                            MRPNetworkStatCRUDType _netstatcrud = new MRPNetworkStatCRUDType();
                            Objects.Copy(_db_netstat_record, _netstatcrud);
                            _netstat_list.Add(_netstatcrud);
                            if (_netstat_list.Count > Global.portal_upload_netstat_page_size)
                            {
                                _cloud_movey.netstat().create_bulk(_netstat_list);
                                _netstat_list.Clear();
                            }
                        }
                        if (_netstat_list.Count > 0)
                        {
                            _cloud_movey.netstat().create_bulk(_netstat_list);
                        }

                        //remove all processed records from from local database
                        Logger.log(String.Format("Deleting uploaded Netstat records: {0}", _db_netstats.Count()), Logger.Severity.Debug);
                        Stopwatch _sw_delete = Stopwatch.StartNew();
                        db.Netstat.RemoveRange(_db_netstats);
                        db.SaveChanges();
                        _sw_delete.Stop();
                        Logger.log(String.Format("Took {0} to delete {1} performance records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _db_netstats.Count()), Logger.Severity.Debug);

                    }
                }
                _sw.Stop();
                Logger.log(String.Format("Completed Nestat Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading netstat information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
