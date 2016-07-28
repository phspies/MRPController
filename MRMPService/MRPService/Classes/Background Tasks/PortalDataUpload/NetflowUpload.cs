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
                    using (MRPDatabase db = new MRPDatabase())
                    {
                        List<NetworkFlow> _db_flows = db.NetworkFlows.ToList();
                        using (NetworkFlowSet _netstat = new NetworkFlowSet())
                        {
                            _db_flows = _netstat.ModelRepository.Get();
                        }

                        //process netflows information
                        List<MRPNetworkFlowCRUDType> _networkflow_list = new List<MRPNetworkFlowCRUDType>();
                        foreach (NetworkFlow _db_flow in _db_flows)
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

                            //add record to list
                            _networkflow_list.Add(_mrp_crud);

                            //process batch
                            if (_networkflow_list.Count() > Global.portal_upload_netflow_page_size)
                            {
                                _cloud_movey.netflow().createnetworkflow(_networkflow_list);
                                _networkflow_list.Clear();
                            }
                        }
                        //process any remaining records
                        if (_networkflow_list.Count() > 0)
                        {
                            _cloud_movey.netflow().createnetworkflow(_networkflow_list);
                        }
                        //remove all processed records from from local database
                        Stopwatch _sw_delete = Stopwatch.StartNew();
                        db.NetworkFlows.RemoveRange(_db_flows);
                        db.SaveChanges();
                        _sw_delete.Stop();
                        Logger.log(String.Format("Took {0} to delete {1} netflow records", TimeSpan.FromMilliseconds(_sw_delete.Elapsed.TotalMilliseconds), _db_flows.Count()), Logger.Severity.Debug);

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
