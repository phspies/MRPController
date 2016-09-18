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

                    while (true)
                    {
                        IEnumerable<Netstat> _increment_records;
                        using (MRPDatabase _db = new MRPDatabase())
                        {
                            _increment_records = _db.Netstat.Take(500).AsEnumerable();
                            if (_increment_records.Count() > 0)
                            {

                                //process performancecounters
                                List<MRPNetworkStatCRUDType> _netstat_list = new List<MRPNetworkStatCRUDType>();
                                foreach (Netstat _db_netstat_record in _increment_records)
                                {
                                    MRPNetworkStatCRUDType _netstatcrud = new MRPNetworkStatCRUDType();
                                    _netstatcrud.pid = _db_netstat_record.pid;
                                    _netstatcrud.process = _db_netstat_record.process;
                                    _netstatcrud.proto = _db_netstat_record.proto;
                                    _netstatcrud.source_ip = _db_netstat_record.source_ip;
                                    _netstatcrud.source_port = _db_netstat_record.source_port;
                                    _netstatcrud.state = _db_netstat_record.state;
                                    _netstatcrud.target_ip = _db_netstat_record.target_ip;
                                    _netstatcrud.target_port = _db_netstat_record.target_port;
                                    _netstatcrud.workload_id = _db_netstat_record.workload_id;

                                    _netstat_list.Add(_netstatcrud);

                                    if (_netstat_list.Count > Global.portal_upload_performanceounter_page_size + 100)
                                    {
                                        _cloud_movey.netstat().create_bulk(_netstat_list);
                                        _netstat_list.Clear();

                                    }
                                }
                                //upload last remaining records
                                if (_netstat_list.Count > 0)
                                {
                                    _cloud_movey.netstat().create_bulk(_netstat_list);
                                }

                                //remove all processed records from from local database
                                Stopwatch _sw_delete = Stopwatch.StartNew();
                                _db.Netstat.RemoveRange(_increment_records);
                                _db.SaveChanges();
                                _sw_delete.Stop();
                                Logger.log(String.Format("Took {0} to delete {1} netstat records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _increment_records.Count()), Logger.Severity.Debug);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                _sw.Stop();
                Logger.log(String.Format("Completed Netstat Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading netstat information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
