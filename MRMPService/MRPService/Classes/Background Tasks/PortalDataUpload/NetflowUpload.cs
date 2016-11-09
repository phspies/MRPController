using MRMPService.MRMPAPI.Contracts;
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
    class NetflowUpload
    {
        public void Start()
        {
            try
            {
                Logger.log("Starting Netflow Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                using (MRMPAPI.MRMP_ApiClient _cloud_movey = new MRMPAPI.MRMP_ApiClient())
                {
                    while (true)
                    {
                        IEnumerable<NetworkFlow> _increment_records;
                        using (MRPDatabase _db = new MRPDatabase())
                        {
                            _increment_records = _db.NetworkFlows.Take(500).AsEnumerable();
                            if (_increment_records.Count() > 0)
                            {
                                List<MRPNetworkFlowCRUDType> _netflow_list = new List<MRPNetworkFlowCRUDType>();
                                foreach (NetworkFlow _db_flow in _increment_records)
                                {
                                    MRPNetworkFlowCRUDType _mrp_crud = new MRPNetworkFlowCRUDType();
                                    _mrp_crud.kbyte = _db_flow.kbyte;
                                    _mrp_crud.packets = _db_flow.packets;
                                    _mrp_crud.protocol = _db_flow.protocol;
                                    _mrp_crud.source_address = _db_flow.source_address;
                                    _mrp_crud.source_port = _db_flow.source_port;
                                    _mrp_crud.start_timestamp = _db_flow.start_timestamp;
                                    _mrp_crud.stop_timestamp = _db_flow.stop_timestamp;
                                    _mrp_crud.target_address = _db_flow.target_address;
                                    _mrp_crud.target_port = _db_flow.target_port;
                                    _mrp_crud.timestamp = _db_flow.timestamp;

                                    _netflow_list.Add(_mrp_crud);

                                    if (_netflow_list.Count > Global.portal_upload_netflow_page_size)
                                    {
                                        _cloud_movey.netflow().createnetworkflow(_netflow_list);
                                        _netflow_list.Clear();
                                    }
                                }
                                //upload last remaining records
                                if (_netflow_list.Count > 0)
                                {
                                    _cloud_movey.netflow().createnetworkflow(_netflow_list);
                                }

                                //remove all processed records from from local database
                                Stopwatch _sw_delete = Stopwatch.StartNew();
                                _db.NetworkFlows.RemoveRange(_increment_records);
                                _db.SaveChanges();
                                _sw_delete.Stop();
                                Logger.log(String.Format("Took {0} to delete {1} netflow records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _increment_records.Count()), Logger.Severity.Debug);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                _sw.Stop();
                Logger.log(String.Format("Completed Netflow Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalMilliseconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading Netflow information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
