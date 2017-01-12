using MRMPService.Modules.MRMPPortal.Contracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;
using MRMPService.MRMPAPI;
using System.Threading;

namespace MRMPService.Scheduler.PortalDataUpload
{
    class NetflowUpload
    {
        public void Start()
        {
            try
            {
                Logger.log("Starting Netflow Upload Thread", Logger.Severity.Debug);
                Stopwatch _sw = Stopwatch.StartNew();
                while (true)
                {
                    IEnumerable<NetworkFlow> _increment_records;
                    using (MRPDatabase _db = new MRPDatabase())
                    {
                        _increment_records = _db.NetworkFlows.Take(500).AsEnumerable();
                    }
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

                            if (_netflow_list.Count > MRMPServiceBase.portal_upload_netflow_page_size)
                            {
                                MRMPServiceBase._mrmp_api.netflow().createnetworkflow(_netflow_list).Wait();
                                _netflow_list.Clear();
                            }
                        }
                        //upload last remaining records
                        if (_netflow_list.Count > 0)
                        {
                            MRMPServiceBase._mrmp_api.netflow().createnetworkflow(_netflow_list).Wait();
                        }

                        //remove all processed records from from local database
                        Stopwatch _sw_delete = Stopwatch.StartNew();
                        using (MRPDatabase _ctx = new MRPDatabase())
                        {
                            var _primary_keys = _increment_records.Select(x => x.id).ToList();
                            var _db_records = _ctx.NetworkFlows.Where(r => _primary_keys.Contains(r.id));
                            _ctx.NetworkFlows.RemoveRange(_db_records);
                        }
                        _sw_delete.Stop();
                        Logger.log(String.Format("Took {0} to delete {1} netflow records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalSeconds), _increment_records.Count()), Logger.Severity.Debug);
                    }
                    else
                    {
                        break;
                    }
                }

                _sw.Stop();
                Logger.log(String.Format("Completed Netflow Upload Thread in {0}", TimeSpan.FromMilliseconds(_sw.Elapsed.TotalSeconds)), Logger.Severity.Debug);
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error uploading Netflow information to portal {0}", ex.ToString()), Logger.Severity.Error);
            }
        }
    }
}
